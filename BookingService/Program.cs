using MassTransit;
using Microsoft.EntityFrameworkCore;
using P4.BookingService.Consumers;
using P4.BookingService.Data;
using P4.BookingService.Repositories;
using P4.BookingService.Repositories.IRepositories;
using P4.BookingService.Services;
using P4.BookingService.Services.IServices;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

#region Switch http2 (Dev env)
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
#endregion

builder.Services.AddControllers();

#region Connect DB

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

#endregion

#region Redis

var redisConnectionString = builder.Configuration.GetValue<string>("Redis:ConnectionString");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

#endregion

#region MassTransit

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<BookingCreateCommandConsumer>(); 
    x.AddEntityFrameworkOutbox<BookingDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);
        o.UseSqlServer();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("booking-create-command", e =>
        {          
            e.UseMessageRetry(r =>
            {
                r.Exponential(
                    retryLimit: 5,
                    minInterval: TimeSpan.FromSeconds(10),
                    maxInterval: TimeSpan.FromMinutes(1),
                    intervalDelta: TimeSpan.FromSeconds(5)
                );
            });

            e.ConfigureConsumer<BookingCreateCommandConsumer>(context);
        });

        cfg.ReceiveEndpoint("processing-failed-saga", e =>
        {
            e.ConfigureConsumer<ProcessingFailedSagaConsumer>(context);
        });

        cfg.ReceiveEndpoint("release-seats-hold", e =>
        {
            e.ConfigureConsumer<ReleaseSeatHoldConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

#endregion

#region Services

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, P4.BookingService.Services.BookingService>();

#endregion

#region Grpc Client

builder.Services.AddScoped<GrpcScreeningClientService>();

#endregion

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
