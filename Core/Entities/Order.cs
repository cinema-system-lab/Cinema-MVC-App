using Core.Enums;

namespace Core.Entities;

public class Order
{
    public Guid Id { get; set; } // було інт
    public string UserId { get; set; } // IdentityUser.Id has a string type
    public User User { get; set; } = null!;
    public int SessionId { get; set; }
    public DateTime CreatedAt { get; set; }

    public OrderStatus Status { get; set; }
    public Session Session { get; set; } = null!;
    public Payment? Payment { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}