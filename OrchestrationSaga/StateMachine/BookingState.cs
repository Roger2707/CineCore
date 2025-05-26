using MassTransit;

namespace OrchestrationSaga.StateMachine
{
    public class BookingState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public Guid BookingId { get; set; }
        public List<Guid> SeatIds { get; set; }
        public Guid ScreeningId { get; set; }
        public Guid UserId { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
