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

    public async Task CreateOrderAsync(string userId, CreateOrderRequest request)
    {
        var session = await _context.Sessions
            .Include(s => s.Hall)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId);

        if (session == null) throw new Exception("Session not found");

        var takenSeats = await _context.Tickets
            .Where(t => t.SessionId == request.SessionId && request.SeatIds.Contains(t.SeatId))
            .Select(t => t.SeatId)
            .ToListAsync();

        if (takenSeats.Any())
        {
            throw new InvalidOperationException($"Seats with IDs {string.Join(", ", takenSeats)} are already taken.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SessionId = request.SessionId,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.Pending 
            };

            _context.Orders.Add(order);

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

        return orders.Select(o => new OrderDTO
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
        }).ToList();
    }

    public async Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) throw new Exception("Order not found");

        order.Status = newStatus;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOrderAsync(Guid orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Tickets)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order != null)
        {
            _context.Tickets.RemoveRange(order.Tickets);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}