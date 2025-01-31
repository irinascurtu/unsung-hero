namespace Contracts.Events
{
    public class OrderCreated
    {
        public DateTime CreatedAt { get; set; }
        public int OrderId { get; set; }
    }
}
