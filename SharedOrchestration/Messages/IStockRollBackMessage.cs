namespace SharedOrchestration.Messages
{
    public interface IStockRollBackMessage
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
