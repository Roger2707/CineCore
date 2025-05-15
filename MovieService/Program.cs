using Microsoft.EntityFrameworkCore;
using MovieService.Data;
using MovieService.Repositories;
using MovieService.Repositories.IRepositories;
using MovieService.Services;
using MovieService.Services.IService;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

#region Connect DB

builder.Services.AddDbContext<MovieDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

#endregion

#region Identity

#endregion

#region MassTransit

#endregion

#region Services

builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService.Services.MovieService>();

#endregion 

#region Grpc

builder.Services.AddGrpc();

#endregion

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcMovieService>();

app.Run();
