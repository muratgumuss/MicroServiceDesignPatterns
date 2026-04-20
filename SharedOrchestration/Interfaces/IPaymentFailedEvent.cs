using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedOrchestration.Interfaces
{
    public interface IPaymentFailedEvent : CorrelatedBy<Guid>
    {
        public string Reason { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
