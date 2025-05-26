using CinemaService.Consumers;
using CinemaService.Data;
using CinemaService.Repositories;
using CinemaService.Repositories.IRepositories;
using CinemaService.Services;
using CinemaService.Services.IServices;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Switch http2 (Dev env)

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

#endregion

builder.Services.AddControllers();

#region Connect DB

builder.Services.AddDbContext<CinemaDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

#endregion

#region Identity

#endregion

#region MassTransit

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<UpdateSeatStatusConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("update-seat-status", e =>
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

            e.ConfigureConsumer<UpdateSeatStatusConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

#endregion

#region Services

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICinemaService, CinemaService.Services.CinemaService>();
builder.Services.AddScoped<ITheaterService, TheaterService>();
builder.Services.AddScoped<IScreeingService, ScreeningService>();

// gRPC
builder.Services.AddScoped<GrpcMovieClientService>();

#endregion 

#region Grpc

builder.Services.AddGrpc();

#endregion

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<GrpcCinemaService>();

app.Run();
