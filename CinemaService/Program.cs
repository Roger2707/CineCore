using CinemaService.Data;
using CinemaService.Repositories;
using CinemaService.Repositories.IRepositories;
using CinemaService.Services;
using CinemaService.Services.IServices;
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

#endregion

#region Services

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICinemaService, CinemaService.Services.CinemaService>();
builder.Services.AddScoped<IRoomService, RoomService>();

#endregion 

#region Grpc

builder.Services.AddGrpc();

#endregion

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<GrpcCinemaService>();
app.MapGrpcService<GrpcRoomService>();

app.Run();
