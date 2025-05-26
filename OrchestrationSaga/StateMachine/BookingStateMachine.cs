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
            //Initially(
            //    When(BookingCreatedEvent)
            //        .Then(ctx =>
            //        {
            //            Console.WriteLine($"[Saga] Booking created: {ctx.Message.BookingId}");
            //            // Update seat status to BOOKED
            //        })
            //        .Send(new Uri("queue:update-seat-status"), ctx => new UpdateSeatStatus(
            //            ctx.Message.ScreeningId,
            //            ctx.Message.SeatIds,
            //            SeatStatus.BOOKED))
            //        .Send(new Uri("queue:sending-email-ticket"), ctx => new EmailTicketCreated(
            //            ctx.Message.BookingId,
            //            ctx.Message.UserId))
            //        .TransitionTo(ProcessingTicket)
            //);

            //During(ProcessingTicket,
            //    When(SeatUpdateCompletedEvent)
            //        .Then(ctx => Console.WriteLine($"[Saga] Seats updated for: {ctx.Message.BookingId}"))
            //        .TransitionTo(TicketSending),

            //    When(SeatUpdateFailedEvent)
            //        .Then(ctx => Console.WriteLine($"[Saga] Seat update failed: {ctx.Message.BookingId}"))
            //        .Send(new Uri("queue:refund-payment"), ctx => new RefundPayment(ctx.Message.PaymentIntentId))
            //        .TransitionTo(Failed)
            //);

            //During(TicketSending,
            //    When(TicketDeliveredEvent)
            //        .Then(ctx => Console.WriteLine($"[Saga] Ticket delivered: {ctx.Message.BookingId}"))
            //        .TransitionTo(Completed)
            //        .Finalize()
            //);

            //// Compensation logic
            //DuringAny(
            //    When(BookingFailedEvent)
            //        .Then(ctx =>
            //        {
            //            Console.WriteLine($"[Saga] Booking failed: {ctx.Message.BookingId}");
            //            // Release seats and refund if needed
            //        })
            //        .Send(new Uri("queue:release-seats"), ctx => new ReleaseSeatHold(
            //            ctx.Message.ScreeningId,
            //            ctx.Message.SeatIds))
            //        .SendIf(ctx => !string.IsNullOrEmpty(ctx.Message.PaymentIntentId),
            //            new Uri("queue:refund-payment"), ctx => new RefundPayment(ctx.Message.PaymentIntentId))
            //        .TransitionTo(Failed)
            //        .Finalize()
            //);

            //SetCompletedWhenFinalized();
        }
    }
}
