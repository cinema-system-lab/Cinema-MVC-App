using Core.Enums;

namespace Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Зв'язок з Identity
        public int SessionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public virtual Session Session { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
