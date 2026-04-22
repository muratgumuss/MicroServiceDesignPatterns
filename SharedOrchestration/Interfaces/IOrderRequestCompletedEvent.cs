namespace SharedOrchestration.Interfaces
{
    public interface IOrderRequestCompletedEvent
    {
        public int OrderId { get; set; }
    }
}
