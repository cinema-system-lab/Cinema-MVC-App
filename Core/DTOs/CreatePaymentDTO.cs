using Core.Enums;

namespace Core.DTOs;

public class CreatePaymentDTO
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
}