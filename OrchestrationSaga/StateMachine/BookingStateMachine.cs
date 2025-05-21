using Contracts.BookingEvents;
using MassTransit;

namespace OrchestrationSaga.StateMachine
{
    public class BookingStateMachine : MassTransitStateMachine<BookingState>
    {
        public State Payment { get; }
        public State Completed { get; }
        public State Failed { get; }

        public Event<BookingCreated> BookingCreatedEvent { get; private set; }
        public Event<PaymentCompleted> PaymentCompletedEvent { get; private set; }
        public Event<PaymentFailed> PaymentFailedEvent { get; private set; }

        public BookingStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => BookingCreatedEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));
            Event(() => PaymentCompletedEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));
            Event(() => PaymentFailedEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));

            ConfigureStateMachine();    
        }

        private void ConfigureStateMachine()
        {
            Initially(
                When(BookingCreatedEvent)
                    .Then(ctx =>
                    {
                        Console.WriteLine($"[Saga] BookingCreated: {ctx.Message.BookingId}");

                        ctx.Saga.BookingId = ctx.Message.BookingId;
                        ctx.Saga.SeatIds = ctx.Message.SeatIds;
                        ctx.Saga.ScreeningId = ctx.Message.ScreeningId;
                    })
                    .Send(new Uri("queue:payment-requested"), ctx => new PaymentRequested(ctx.Message.BookingId))
                    .TransitionTo(Payment)
                    .Catch<Exception>(ex => ex
                        .Then(async ctx =>
                        {
                            Console.WriteLine($"[Saga] Failed in BookingCreated: {ctx.Exception.Message}");
                            await ctx.Send(new Uri("queue:booking-failed")
                                            , new BookingFailed(ctx.Message.BookingId, ctx.Saga.SeatIds, ctx.Saga.ScreeningId)
                                        );
                        })
                        .TransitionTo(Failed)
                        .Finalize()
                    )
            );

            During(Payment,
                When(PaymentCompletedEvent)
                    .Then(ctx => Console.WriteLine($"[Saga] Payment success for booking: {ctx.Message.BookingId}"))
                    .TransitionTo(Completed)
                    .Finalize(),

                When(PaymentFailedEvent)
                    .Then(ctx => Console.WriteLine("[Saga] Payment Failed"))
                    .TransitionTo(Failed)
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }
}
