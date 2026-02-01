using Core.DTOs;
using Core.Enums;

namespace Core.Interfaces.Services;

public interface IOrderService
{
    Task<Guid>  CreateOrderAsync(string userId, CreateOrderRequest request);
    Task<List<OrderDTO>> GetOrdersByUserAsync(string userId);

    
    Task<OrderDTO?> GetOrderByIdAsync(Guid id);

    Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus);

}