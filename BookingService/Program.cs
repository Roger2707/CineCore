using BookingService.Consumers;
using BookingService.Data;
using BookingService.Repositories;
using BookingService.Repositories.IRepositories;
using BookingService.Services;
using BookingService.Services.IServices;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Switch http2 (Dev env)
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
#endregion

builder.Services.AddControllers();

#region Connect DB

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

#endregion

#region MassTransit

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<SeatHoldRequestedConsumer>();

    x.AddEntityFrameworkOutbox<BookingDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);
        o.UseSqlServer();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("seat-hold-requested", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<SeatHoldRequestedConsumer>(context);
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
