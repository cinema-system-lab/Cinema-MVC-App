using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class TicketService : ITicketService
{
    private readonly CinemaAppDbContext _context;
    private readonly IMapper _mapper;

    public TicketService(CinemaAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TicketDTO>> GetAllTicketsAsync()
    {
        var tickets = await _context.Tickets
            .Include(t => t.Order)
            .Include(t => t.Session).ThenInclude(s => s.Movie)
            .Include(t => t.Session).ThenInclude(s => s.Hall)
            .Include(t => t.Seat)
            .Where(t => t.Order.Status == OrderStatus.Pending || 
                        t.Order.Status == OrderStatus.Paid)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<TicketDTO>>(tickets);
    }

    public async Task<List<TicketDTO>> GetTicketsByOrderAsync(Guid orderId)
    {
        var tickets = await _context.Tickets
            .Where(t => t.OrderId == orderId)
            .Include(t => t.Order)
            .Include(t => t.Session).ThenInclude(s => s.Movie)
            .Include(t => t.Session).ThenInclude(s => s.Hall)
            .Include(t => t.Seat)
            .Where(t => t.Order.Status == OrderStatus.Pending || 
                        t.Order.Status == OrderStatus.Paid)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<TicketDTO>>(tickets);
    }

    public async Task<TicketDTO?> GetTicketByIdAsync(Guid orderId, int sessionId, int seatId)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Session).ThenInclude(s => s.Movie)
            .Include(t => t.Session).ThenInclude(s => s.Hall)
            .Include(t => t.Seat)
            .FirstOrDefaultAsync(t => t.OrderId == orderId
                                   && t.SessionId == sessionId
                                   && t.SeatId == seatId);

        return _mapper.Map<TicketDTO>(ticket);
    }

    public async Task CreateTicketAsync(TicketCreateDTO ticketDto)
    {
        var session = await _context.Sessions.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == ticketDto.SessionId);
        if (session == null) throw new InvalidOperationException("Session not found.");
        
        if (session.StartTime <= DateTime.Now)
            throw new InvalidOperationException("Cannot create ticket for a started or past session.");

        var seat = await _context.Seats.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == ticketDto.SeatId);
        if (seat == null) throw new InvalidOperationException("Seat not found.");
        
        if (seat.HallId != session.HallId)
            throw new InvalidOperationException("Seat does not belong to the session hall.");

        var order = await _context.Orders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == ticketDto.OrderId);
        if (order == null) throw new InvalidOperationException("Order not found.");

        if (order.SessionId != ticketDto.SessionId)
            throw new InvalidOperationException("Ticket session must match order session.");
        
        if (order.Status != OrderStatus.Pending)
            throw new InvalidOperationException("Tickets can be added only to pending orders.");

        var isOccupied = await _context.Tickets.AnyAsync(t => t.SessionId == ticketDto.SessionId
                   && t.SeatId == ticketDto.SeatId
                   && t.Order.Status != OrderStatus.Cancelled
                   && t.Order.Status != OrderStatus.Refunded);

        if (isOccupied) throw new InvalidOperationException("This seat is already booked and active.");

        var ticket = _mapper.Map<Ticket>(ticketDto);
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTicketAsync(Guid orderId, int sessionId, int seatId)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Order)
            .Include(t => t.Session)
            .FirstOrDefaultAsync(t => t.OrderId == orderId
                                   && t.SessionId == sessionId
                                   && t.SeatId == seatId);

        if (ticket == null) return;

        if (ticket.Session.StartTime <= DateTime.Now)
        {
            throw new InvalidOperationException("Cannot cancel a ticket for a session that has already started or passed.");
        }

        if (ticket.Order.Status != OrderStatus.Pending)
        {
            string message = ticket.Order.Status switch
            {
                OrderStatus.Paid => "Cannot cancel a paid ticket.",
                OrderStatus.Cancelled => "This order is already cancelled.",
                OrderStatus.Refunded => "This ticket has already been refunded.",
                _ => "Ticket cannot be deleted in current order status."
            };

            throw new InvalidOperationException(message);
        }

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<List<int>> GetOccupiedSeatIdsAsync(int sessionId)
    {
        return await _context.Tickets
            .Where(t => t.SessionId == sessionId
                        && t.Order.Status != OrderStatus.Cancelled
                        && t.Order.Status != OrderStatus.Refunded)
            .Select(t => t.SeatId)
            .Distinct()
            .ToListAsync();
    }
}
