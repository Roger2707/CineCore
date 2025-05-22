using Contracts.BookingEvents;
using MassTransit;

namespace OrchestrationSaga.StateMachine
{
    public class BookingStateMachine : MassTransitStateMachine<BookingState>
    {
        public State Payment { get; }
        public State Completed { get; }
        public State Failed { get; }

        public Event<BookingCreated> BookingCreatedEvent { get; }
        public Event<PaymentCompleted> PaymentCompletedEvent { get; }
        public Event<PaymentFailed> PaymentFailedEvent { get; }

        public BookingStateMachine()
        {
            InstanceState(x => x.CurrentState);

            // 
            Event(() => BookingCreatedEvent, x =>
            {
                x.CorrelateById(ctx => ctx.Message.BookingId);
                x.InsertOnInitial = true;
                x.SetSagaFactory(ctx => new BookingState
                {
                    CorrelationId = ctx.Message.BookingId,
                    BookingId = ctx.Message.BookingId,
                    ScreeningId = ctx.Message.ScreeningId,
                    SeatIds = ctx.Message.SeatIds
                });
            });
            Event(() => PaymentCompletedEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));
            Event(() => PaymentFailedEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));

            ConfigureStateMachine();    
        }

        private void ConfigureStateMachine()
        {
            Initially(
                When(BookingCreatedEvent)
                    .Then(ctx => Console.WriteLine($"[Saga] BookingCreated: {ctx.Message.BookingId}"))
                    .Send(new Uri("queue:payment-requested"), ctx => new PaymentRequested(ctx.Message.BookingId))
                    .TransitionTo(Payment)
                    .Catch<Exception>(ex => ex
                        .Then(ctx => Console.WriteLine($"[Saga] FAILED for BookingCreated: {ctx.Message.BookingId}"))
                        .Send(new Uri("queue:booking-failed")
                                , ctx => new BookingFailed(ctx.Message.BookingId, ctx.Saga.SeatIds, ctx.Saga.ScreeningId))                          
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
