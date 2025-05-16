using Microsoft.EntityFrameworkCore;
using ScheduleService.Data;
using ScheduleService.Repositories;
using ScheduleService.Repositories.IRepositories;
using ScheduleService.Services;
using ScheduleService.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

#region Using Http 2
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
#endregion

builder.Services.AddControllers();

#region Connect DB

builder.Services.AddDbContext<ScheduleDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

#endregion


#region Services

builder.Services.AddScoped<IScreeningRepository, ScreeningRepository>();
builder.Services.AddScoped<IScreeningService, ScreeningService>();

#endregion

#region Grpc Clients

builder.Services.AddScoped<GrpcMovieClientService>();

#endregion

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
