using MassTransit;
using Microsoft.EntityFrameworkCore;
using P2.CinemaService.Consumers;
using P2.CinemaService.Data;
using P2.CinemaService.Repositories;
using P2.CinemaService.Repositories.IRepositories;
using P2.CinemaService.Services;
using P2.CinemaService.Services.IServices;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

#region Switch http2 (Dev env)

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

#endregion

builder.Services.AddControllers();

#region Connect DB

builder.Services.AddDbContext<CinemaDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

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
builder.Services.AddScoped<ICinemaService, P2.CinemaService.Services.CinemaService>();
builder.Services.AddScoped<ITheaterService, TheaterService>();
builder.Services.AddScoped<IScreeingService, ScreeningService>();

// gRPC
builder.Services.AddScoped<GrpcMovieClientService>();

#endregion 

#region Grpc

builder.Services.AddGrpc();

#endregion

#region Add Shared Authentication - Shared Project
builder.Services.AddSharedJwtAuthentication(builder.Configuration);
#endregion

var app = builder.Build();

#region Use Shared Authentication - Shared Project
app.UseSharedAuthentication();
#endregion
app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<GrpcCinemaService>();
app.MapGrpcService<GrpcScreeningService>();

app.Run();
