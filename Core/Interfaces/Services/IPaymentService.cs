using Core.DTOs;
using Core.Enums;

namespace Core.Interfaces.Services;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync();
    Task<PaymentDTO> GetPaymentByIdAsync(Guid id);
    Task<PaymentDTO> GetPaymentByOrderIdAsync(Guid orderId);
    
    Task<Guid> CreatePaymentAsync(CreatePaymentDTO createPaymentDto);
    Task UpdatePaymentStatusAsync(Guid id, PaymentStatus newStatus);
}