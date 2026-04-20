using SharedOrchestration.Interfaces;

namespace SharedOrchestration.Events
{
    public class OrderRequestFailedEvent : IOrderRequestFailedEvent
    {
        public int OrderId { get; set; }
        public string Reason { get; set; }
    }
}
