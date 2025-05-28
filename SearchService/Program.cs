using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using P3.SearchService.Consumers;
using P3.SearchService.Data;
using P3.SearchService.Services;

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

#region Masstransit Consumer Settings

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<MovieCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        // retry : if stop mongodb 
        cfg.ReceiveEndpoint("search-movie-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<MovieCreatedConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});

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
