using MassTransit;

namespace OrchestrationSaga.StateMachine
{
    public class BookingState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = null!;
        public bool SeatHoldSuccess { get; set; }
        public bool PaymentSuccess { get; set; }
    }
}
