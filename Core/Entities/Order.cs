using Core.Enums;

namespace Core.Entities;

public class Order
{
    public Guid Id { get; set; } // було інт
    public int UserId { get; set; } 
    
    public int SessionId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public OrderStatus Status { get; set; }
    public Session Session { get; set; } 
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>(); 
}