using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs;
using Core.Enums;

namespace Core.Interfaces.Services;

public interface IOrderService
{
    Task CreateOrderAsync(string userId, CreateOrderRequest request);

    Task<List<OrderDTO>> GetOrdersByUserAsync(string userId);

    Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus);

    Task DeleteOrderAsync(Guid orderId);
}