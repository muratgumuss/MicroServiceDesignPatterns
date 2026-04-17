using MassTransit;

namespace SharedOrchestration.Interfaces
{
    public interface IOrderCreatedEvent : CorrelatedBy<Guid>
    {
        List<OrderItemMessage> OrderItems { get; set; }
    }
}
