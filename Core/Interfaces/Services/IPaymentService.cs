using Core.DTOs;
using Core.Enums;

namespace Core.Interfaces.Services;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync();
    Task<PaymentDTO> GetPaymentByIdAsync(Guid id);
    
    Task<Guid> CreatePaymentAsync(Guid orderId, decimal amount, PaymentStatus status = PaymentStatus.Pending);
    Task UpdatePaymentStatusAsync(Guid id, PaymentStatus newStatus);
    
    Task<PaymentDTO> GetPaymentByOrderIdAsync(Guid orderId);
}