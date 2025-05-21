using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrchestrationSaga;
using OrchestrationSaga.Data;
using OrchestrationSaga.StateMachine;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<OrchestratorDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<BookingStateMachine, BookingState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
            r.AddDbContext<DbContext, OrchestratorDbContext>((provider, opt) =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("booking-state", e =>
        {
            e.ConfigureSaga<BookingState>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Logging.AddConsole()
    .SetMinimumLevel(LogLevel.Debug);

var host = builder.Build();
host.Run();
