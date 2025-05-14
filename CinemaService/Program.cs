using CinemaService.Data;
using CinemaService.Repositories;
using CinemaService.Repositories.IRepositories;
using CinemaService.Services.IServices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


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

#endregion 

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
