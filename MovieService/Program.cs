using MassTransit;
using Microsoft.EntityFrameworkCore;
using P1.MovieService.Data;
using P1.MovieService.Repositories;
using P1.MovieService.Repositories.IRepositories;
using P1.MovieService.Services;
using P1.MovieService.Services.IService;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

#region Switch http2 (Dev env)

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

#endregion

builder.Services.AddControllers();

#region Connect DB

builder.Services.AddDbContext<MovieDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

#endregion

#region MassTransit

builder.Services.AddMassTransit(x =>
{
    // ensure to add the outbox after savechange success
    x.AddEntityFrameworkOutbox<MovieDBContext>(o =>
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

#region Services

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, P1.MovieService.Services.MovieService>();

#endregion 

#region Grpc

builder.Services.AddGrpc();

#endregion

#region Authentication

builder.Services.AddSharedJwtAuthentication(builder.Configuration);

#endregion

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcMovieService>();

app.Run();
