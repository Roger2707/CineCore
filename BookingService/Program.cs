using BookingService.Data;
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
    // ensure to add the outbox after savechange success
    x.AddEntityFrameworkOutbox<BookingDbContext>(o =>
    {
        // if process rabbitMQ server is downed, retry every 10 seconds
        o.QueryDelay = TimeSpan.FromSeconds(10);
        o.UseSqlServer();

        // when savechange success, no add to message queue immediately, add outbox first
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

#endregion

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
