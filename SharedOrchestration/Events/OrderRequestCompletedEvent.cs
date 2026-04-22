using SharedOrchestration.Interfaces;

namespace SharedOrchestration.Events
{
    public class OrderRequestCompletedEvent : IOrderRequestCompletedEvent
    {
        public int OrderId { get; set; }
    }
}
