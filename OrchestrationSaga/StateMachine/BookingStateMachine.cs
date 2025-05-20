using Contracts.BookingEvents;
using MassTransit;

namespace OrchestrationSaga.StateMachine
{
    public class BookingStateMachine : MassTransitStateMachine<BookingState>
    {
        public State SeatHold, Payment, Notification, Completed, Failed;

        public Event<BookingCreated> BookingCreatedEvent { get; private set; } = null!;
        public Event<SeatHoldCompleted> SeatHoldCompletedEvent { get; private set; } = null!;
        public Event<SeatHoldFailed> SeatHoldFailedEvent { get; private set; } = null!;
        public Event<PaymentCompleted> PaymentCompletedEvent { get; private set; } = null!;
        public Event<PaymentFailed> PaymentFailedEvent { get; private set; } = null!;

        public BookingStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => BookingCreatedEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));
            Event(() => SeatHoldCompletedEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));
            Event(() => SeatHoldFailedEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));
            Event(() => PaymentCompletedEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));
            Event(() => PaymentFailedEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));

            Initially(
                When(BookingCreatedEvent)
                    .Then(ctx => Console.WriteLine($"[Saga] BookingCreated: {ctx.Message.BookingId}"))
                    .Send(new Uri("queue:seat-hold"), ctx => new SeatHoldRequested(ctx.Message.BookingId))
                    .TransitionTo(SeatHold)
            );

            During(SeatHold,
                When(SeatHoldCompletedEvent)
                    .Then(ctx => ctx.Saga.SeatHoldSuccess = true)
                    .Send(new Uri("queue:payment"), ctx => new PaymentRequested(ctx.Message.BookingId))
                    .TransitionTo(Payment),

                When(SeatHoldFailedEvent)
                    .Then(ctx => Console.WriteLine("[Saga] SeatHold Failed"))
                    .TransitionTo(Failed)
                    .Finalize()
            );

            During(Payment,
                When(PaymentCompletedEvent)
                    .Then(ctx => ctx.Saga.PaymentSuccess = true)
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
