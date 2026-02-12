using Core.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Constants;
using Core.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly CinemaAppDbContext _context;
    private readonly ITicketService _ticketService;

    public OrderService(
        CinemaAppDbContext context,
        ITicketService ticketService)
    {
        _context = context;
        _ticketService = ticketService;
    }

    public async Task<Guid> CreateOrderAsync(string userId, CreateOrderRequest request)
    {
        var session = await _context.Sessions
            .Include(s => s.Hall)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId);

        if (session == null)
            throw new Exception("Session not found");

        if (session.StartTime <= DateTime.UtcNow)
            throw new InvalidOperationException("Cannot create order for started session");

        if (!request.SeatIds.Any())
            throw new InvalidOperationException("Order must contain at least one ticket");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SessionId = session.Id,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var seatId in request.SeatIds)
            {
                await _ticketService.CreateTicketAsync(new TicketCreateDTO
                {
                    OrderId = order.Id,
                    SessionId = session.Id,
                    SeatId = seatId
                });
            }

            await transaction.CommitAsync();
            return order.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await _context.Orders
            .Include(o => o.Tickets)
            .Include(o => o.Session)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId);

        if (order == null)
            throw new Exception("Order not found");

        if (order.Session.StartTime <= DateTime.UtcNow)
            throw new InvalidOperationException("Cannot update order for started session");
        
        if (order.Status == OrderStatus.Pending && payment == null &&
            order.CreatedAt.AddMinutes(OrderConstants.ReservationMinutes) <= DateTime.UtcNow)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Tickets.RemoveRange(order.Tickets);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            throw new InvalidOperationException("Order expired and deleted");
        }

        if (!IsValidTransition(order.Status, newStatus))
            throw new InvalidOperationException("Invalid order status transition");

        order.Status = newStatus;
        await _context.SaveChangesAsync();
    }

    private static bool IsValidTransition(OrderStatus current, OrderStatus next)
    {
        return current switch
        {
            OrderStatus.Pending => next is OrderStatus.Paid or OrderStatus.Cancelled,
            OrderStatus.Paid => next == OrderStatus.Refunded,
            _ => false
        };
    }

    public async Task<List<OrderDTO>> GetOrdersByUserAsync(string userId)
    {
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Session).ThenInclude(s => s.Movie)
            .Include(o => o.Session).ThenInclude(s => s.Hall)
            .Include(o => o.Tickets).ThenInclude(t => t.Seat)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(o =>
        {
            var session = o.Session;
            var movie = session.Movie;
            var hall = session.Hall;

            var tickets = o.Tickets.Select(t => new TicketDTO
            {
                OrderId = t.OrderId,
                SessionId = t.SessionId,
                SeatId = t.SeatId,
                MovieTitle = movie.Title,
                HallName = hall.Name,
                StartTime = session.StartTime,
                RowNumber = t.Seat.RowNumber,
                SeatNumber = t.Seat.SeatNumber,
                SeatType = t.Seat.Type,
                Price = t.Seat.Type == SeatType.Premium 
                    ? session.BasePrice * 1.5m 
                    : session.BasePrice
            }).ToList();

            return new OrderDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                CreatedAt = o.CreatedAt,
                Status = o.Status,
                SessionId = o.SessionId,
                MovieTitle = movie.Title,
                HallName = hall.Name,
                SessionStartTime = session.StartTime,
                TotalPrice = tickets.Sum(t => t.Price),
                Tickets = tickets
            };
        }).ToList();
    }
    
    public async Task<OrderDTO?> GetOrderByIdAsync(Guid id)
    {
        var order = await _context.Orders
            .Where(o => o.Id == id)
            .Include(o => o.Session).ThenInclude(s => s.Movie)
            .Include(o => o.Session).ThenInclude(s => s.Hall)
            .Include(o => o.Tickets).ThenInclude(t => t.Seat)
            .FirstOrDefaultAsync();

        if (order == null)
            return null;

        var session = order.Session;
        var movie = session.Movie;
        var hall = session.Hall;

        var tickets = order.Tickets.Select(t => new TicketDTO
        {
            OrderId = t.OrderId,
            SessionId = t.SessionId,
            SeatId = t.SeatId,
            MovieTitle = movie.Title,
            HallName = hall.Name,
            StartTime = session.StartTime,
            RowNumber = t.Seat.RowNumber,
            SeatNumber = t.Seat.SeatNumber,
            SeatType = t.Seat.Type,
            Price = t.Seat.Type == SeatType.Premium 
                ? session.BasePrice * 1.5m 
                : session.BasePrice
        }).ToList();

        return new OrderDTO
        {
            Id = order.Id,
            UserId = order.UserId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            SessionId = order.SessionId,
            MovieTitle = movie.Title,
            HallName = hall.Name,
            SessionStartTime = session.StartTime,
            TotalPrice = tickets.Sum(t => t.Price),
            Tickets = tickets
        };
    }

}
