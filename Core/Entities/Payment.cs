using Core.Enums;

namespace Core.Entities;

public class Payment
{
    public Guid Id { get; set; } 
    
    public Guid OrderId { get; set; } 
    public Order Order { get; set; } 
    public string UserId => Order?.UserId ?? string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentStatus Status { get; set; }
}