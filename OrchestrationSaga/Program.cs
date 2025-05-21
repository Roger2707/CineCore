using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrchestrationSaga;
using OrchestrationSaga.Data;
using OrchestrationSaga.StateMachine;
using OrchestrationSaga.TestConsumers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<OrchestratorDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<BookingCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("test", false));


    x.AddSagaStateMachine<BookingStateMachine, BookingState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
            r.AddDbContext<DbContext, OrchestratorDbContext>((provider, opt) =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("test-booking-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<BookingCreatedConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Logging.AddConsole()
    .SetMinimumLevel(LogLevel.Debug);

var host = builder.Build();
host.Run();
