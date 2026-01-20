using Core.Enums;

namespace Core.Entities;

public class Ticket
{
    // Тип має збігатися з Order.Id
    public Guid OrderId { get; set; } 
    public Order Order { get; set; } 
    
    public int SessionId { get; set; }
    public Session Session { get; set; } 
    
    public int SeatId { get; set; }
    public Seat Seat { get; set; } 
}