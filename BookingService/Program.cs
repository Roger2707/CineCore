using BookingService.Consumers;
using BookingService.Data;
using BookingService.Repositories;
using BookingService.Repositories.IRepositories;
using BookingService.Services;
using BookingService.Services.IServices;
using MassTransit;
using Microsoft.EntityFrameworkCore;
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
    x.AddConsumersFromNamespaceContaining<BookingCreatedFaultConsumer>();
    x.AddEntityFrameworkOutbox<BookingDbContext>(o =>
    {
        o.UseSqlServer();
        o.UseBusOutbox();
        o.QueryDelay = TimeSpan.FromSeconds(10);
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("booking-failed", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<BookingCreatedFaultConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

#endregion

#region Services

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService.Services.BookingService>();

#endregion

#region Grpc Client

builder.Services.AddScoped<GrpcScreeningClientService>();

#endregion

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
