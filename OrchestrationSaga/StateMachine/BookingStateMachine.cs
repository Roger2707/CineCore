using Contracts.BookingEvents;
using MassTransit;

namespace OrchestrationSaga.StateMachine
{
    public class BookingStateMachine : MassTransitStateMachine<BookingState>
    {
        public State ProcessingTicket { get; set; }
        public State TicketSending { get; set; }
        public State Completed { get; }
        public State Failed { get; }

        public Event<BookingCreated> BookingCreatedEvent { get; }
        public Event<FailedSagaEvent> FailedSagaEvent { get; }
        public Event<SeatUpdateCompleted> SeatUpdateCompletedEvent { get; }
        public Event<SeatUpdateFailed> SeatUpdateFailedEvent { get; }
        public Event<TicketDelivered> TicketDeliveredEvent { get; }

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
                    SeatIds = ctx.Message.SeatIds,
                    UserId = ctx.Message.UserId,
                    PaymentIntentId = ctx.Message.PaymentIntentId
                });
            });
            Event(() => SeatUpdateCompletedEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));
            Event(() => SeatUpdateFailedEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));
            Event(() => TicketDeliveredEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));
            Event(() => FailedSagaEvent, x => x.CorrelateById(ctx => ctx.Message.BookingId));

            ConfigureStateMachine();
        }

        private void ConfigureStateMachine()
        {
            Initially(
                When(BookingCreatedEvent)
                    .Then(ctx =>
                    {
                        Console.WriteLine($"[Saga] Booking created: {ctx.Message.BookingId}");
                        // Update seat status to BOOKED
                    })
                    .Send(new Uri("queue:update-seat-status"), ctx => new UpdateSeatStatus(
                        ctx.Message.BookingId,
                        ctx.Message.ScreeningId,
                        ctx.Message.SeatIds,
                        SeatStatus.BOOKED,
                        ctx.Message.UserId,
                        ctx.Message.PaymentIntentId
                    ))
                    .TransitionTo(ProcessingTicket)
            );

            During(ProcessingTicket,
                When(SeatUpdateCompletedEvent)
                    .Then(ctx => Console.WriteLine($"[Saga] Seats updated for: {ctx.Message.BookingId}"))
                    .Send(new Uri("queue:email-ticket-created"), ctx => new EmailTicketCreated(
                        ctx.Message.BookingId,
                        "user@gmail.com"
                    ))
                    .TransitionTo(TicketSending),

                When(SeatUpdateFailedEvent)
                    .Then(ctx => Console.WriteLine($"[Saga] Seat update failed: {ctx.Message.BookingId}"))
                    .TransitionTo(Failed)
                    .Send(new Uri("queue:processing-failed-saga")
                        , ctx => new ProcessingFailedSaga(ctx.Message.BookingId, ctx.Message.Seats, ctx.Message.ScreeningId, ctx.Message.PaymentIntentId))
            );

            During(TicketSending,
                When(TicketDeliveredEvent)
                    .Then(ctx => Console.WriteLine($"[Saga] Ticket delivered: {ctx.Message.BookingId}"))
                    .TransitionTo(Completed)
                    .Finalize()
            );

            // Compensation logic
            DuringAny(
                When(FailedSagaEvent)
                    .Then(ctx => Console.WriteLine($"[Saga] Event Saga failed: {ctx.Message.BookingId}"))
                    // Will add refund later (because check out success happen earlier than booking)
                    .TransitionTo(Failed)
                    .Finalize()
            );
            //SetCompletedWhenFinalized();
        }
    }
}
