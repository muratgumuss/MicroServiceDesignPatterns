using MassTransit;

namespace SharedOrchestration.Interfaces
{
    public interface IPaymentCompletedEvent : CorrelatedBy<Guid>
    {
    }
}
