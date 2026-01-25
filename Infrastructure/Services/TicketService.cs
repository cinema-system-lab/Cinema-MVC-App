using AutoMapper;
using Core.DTOs;
using Core.Entities;
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
            .Include(t => t.Session).ThenInclude(s => s.Movie)
            .Include(t => t.Session).ThenInclude(s => s.Hall)
            .Include(t => t.Seat)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<TicketDTO>>(tickets);
    }

    public async Task<List<TicketDTO>> GetTicketsByOrderAsync(Guid orderId)
    {
        var tickets = await _context.Tickets
            .Where(t => t.OrderId == orderId)
            .Include(t => t.Session).ThenInclude(s => s.Movie)
            .Include(t => t.Session).ThenInclude(s => s.Hall)
            .Include(t => t.Seat)
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

        var seat = await _context.Seats.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == ticketDto.SeatId);
        if (seat == null) throw new InvalidOperationException("Seat not found.");

        var order = await _context.Orders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == ticketDto.OrderId);
        if (order == null) throw new InvalidOperationException("Order not found.");

        if (order.SessionId != ticketDto.SessionId)
            throw new InvalidOperationException("Ticket session must match order session.");

        var isOccupied = await _context.Tickets.AnyAsync(t =>
            t.SessionId == ticketDto.SessionId && t.SeatId == ticketDto.SeatId);

        if (isOccupied) throw new InvalidOperationException("This seat is already booked for this session.");

        var ticket = _mapper.Map<Ticket>(ticketDto);
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteTicketAsync(Guid orderId, int sessionId, int seatId)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.OrderId == orderId
                                   && t.SessionId == sessionId
                                   && t.SeatId == seatId);

        if (ticket != null)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
    }
}
