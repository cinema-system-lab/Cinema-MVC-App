using Core.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly CinemaAppDbContext _context;

    public OrderService(CinemaAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateOrderAsync(string userId, CreateOrderRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == request.SessionId);

            if (session == null)
                throw new InvalidOperationException("Session not found");

            if (session.StartTime <= DateTime.UtcNow)
                throw new InvalidOperationException("Cannot create order for started session");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SessionId = request.SessionId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var seatId in request.SeatIds)
            {
                var ticket = new Ticket
                {
                    OrderId = order.Id,
                    SessionId = request.SessionId,
                    SeatId = seatId
                };
                _context.Tickets.Add(ticket);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return order.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task<List<OrderDTO>> GetOrdersByUserAsync(string userId)
    {
        var orders = await _context.Orders
            .Include(o => o.Session).ThenInclude(s => s.Movie)
            .Include(o => o.Session).ThenInclude(s => s.Hall)
            .Include(o => o.Tickets).ThenInclude(t => t.Seat)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderDTO?> GetOrderByIdAsync(Guid id)
    {
        var order = await _context.Orders
            .Include(o => o.Session).ThenInclude(s => s.Movie)
            .Include(o => o.Session).ThenInclude(s => s.Hall)
            .Include(o => o.Tickets).ThenInclude(t => t.Seat)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order == null ? null : MapToDto(order);
    }

    public async Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
            throw new InvalidOperationException("Order not found");

        var isValidTransition = order.Status switch
        {
            OrderStatus.Pending => newStatus is OrderStatus.Paid or OrderStatus.Cancelled,
            OrderStatus.Paid => newStatus == OrderStatus.Refunded,
            _ => false
        };

        if (!isValidTransition)
            throw new InvalidOperationException(
                $"Invalid status transition: {order.Status} -> {newStatus}");

        order.Status = newStatus;
        await _context.SaveChangesAsync();
    }

    private static OrderDTO MapToDto(Order o)
    {
        return new OrderDTO
        {
            Id = o.Id,
            CreatedAt = o.CreatedAt,
            Status = o.Status,
            MovieTitle = o.Session.Movie.Title,
            HallName = o.Session.Hall.Name,
            SessionStartTime = o.Session.StartTime,
            TotalPrice = o.Tickets.Count * o.Session.BasePrice,
            Tickets = o.Tickets.Select(t => new TicketDTO
            {
                OrderId = t.OrderId,
                SessionId = t.SessionId,
                SeatId = t.SeatId,
                RowNumber = t.Seat.RowNumber,
                SeatNumber = t.Seat.SeatNumber,
                Price = o.Session.BasePrice,
                MovieTitle = o.Session.Movie.Title,
                HallName = o.Session.Hall.Name,
                StartTime = o.Session.StartTime
            }).ToList()
        };
    }
}
