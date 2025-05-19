using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SearchService.Data;
using SearchService.DB;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

#region Using Http 2
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
#endregion

builder.Services.AddControllers();

#region MongoDB Context

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));


builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();

#endregion

#region Services

builder.Services.AddScoped<MovieSearchService>();

#endregion 

#region Grpc Clients

builder.Services.AddScoped<GrpcMovieClientService>();

#endregion

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
