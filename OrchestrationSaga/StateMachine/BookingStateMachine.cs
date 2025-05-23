using Contracts.BookingEvents;
using MassTransit;

namespace OrchestrationSaga.StateMachine
{
    public class BookingStateMachine : MassTransitStateMachine<BookingState>
    {
        public State TicketSending { get; set; }
        public State Completed { get; }
        public State Failed { get; }

        public Event<BookingCreated> BookingCreatedEvent { get; }
        public Event<BookingFailed> BookingFailedEvent { get; }
        public Event<TicketDelivered> TicketDeliveredEvent { get; private set; }

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
            Event(() => BookingFailedEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));
            Event(() => TicketDeliveredEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));

            ConfigureStateMachine();
        }

        private void ConfigureStateMachine()
        {
            Initially(
                When(BookingCreatedEvent)
                    .Then(ctx => Console.WriteLine($"[Saga] Created Booking Success, BookingId: {ctx.Message.BookingId}"))
                    .Send(new Uri("queue:sending-email-ticket"), ctx => new EmailTicketCreated(ctx.Message.BookingId, "abc@gmail.com"))
                    .TransitionTo(TicketSending)
            );

            During(TicketSending,
                When(TicketDeliveredEvent)
                    .Then(ctx => Console.WriteLine($"[Saga] Ticket delivered successfully for BookingId: {ctx.Message.BookingId}"))
                    .TransitionTo(Completed)
                    .Finalize()
            );

            DuringAny(
                When(BookingFailedEvent)
                    .Then(ctx => Console.WriteLine($"[Saga] Booking failed, bookingId: {ctx.Message.BookingId}"))
                    .TransitionTo(Failed)
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }
}
