using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class HallService : IHallService
{
    private readonly CinemaAppDbContext _context;
    private readonly IMapper _mapper;

    public HallService(CinemaAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<HallDTO>> GetAllHallsAsync()
    {
        var halls = await _context.Halls.AsNoTracking().ToListAsync();
        return _mapper.Map<List<HallDTO>>(halls);
    }

    public async Task<HallDTO?> GetHallAsync(int id)
    {
        var hall = await _context.Halls
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        return hall == null ? null : _mapper.Map<HallDTO>(hall);
    }

    public HallDTO CreateNewHallDTO()
    {
        return new HallDTO { Type = HallType.Standard };
    }

    public async Task CreateHallAsync(HallDTO hall)
    {
        var entity = _mapper.Map<Hall>(hall);
        _context.Halls.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateHallAsync(HallDTO hall)
    {
        var entity = await _context.Halls.FindAsync(hall.Id);
        if (entity == null) return;

        // Check if there are any active/future sessions
        var now = DateTime.Now;
        var hasFutureSessions = await _context.Sessions
            .AnyAsync(s => s.HallId == hall.Id && s.EndTime >= now);

        if (hasFutureSessions)
        {
            throw new InvalidOperationException("Cannot edit hall because it has scheduled future sessions.");
        }

        _mapper.Map(hall, entity);
        await _context.SaveChangesAsync();
    }

    //fix: seats are not a reason to block deletion, sessions are
    public async Task DeleteHallAsync(int id)
    {
        var entity = await _context.Halls.FindAsync(id);
        if (entity == null) return;

        var now = DateTime.Now;
        var hasFutureSessions = await _context.Sessions
            .AnyAsync(s => s.HallId == id);

        if (hasFutureSessions)
        {
            throw new InvalidOperationException(
                "Cannot delete hall because it has scheduled sessions or had."
            );
        }

        _context.Halls.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetSeatsCountAsync(int hallId)
    {
        return await _context.Seats.CountAsync(s => s.HallId == hallId);
    }

    public async Task<int> GetActiveSessionsCountAsync(int hallId)
    {
        var now = DateTime.Now;
        return await _context.Sessions
            .Where(s => s.HallId == hallId && s.StartTime >= now)
            .CountAsync();
    }

    public async Task<int> GetBookedSeatsCountAsync(int hallId)
    {
        var now = DateTime.Now;
        var activeSessionIds = await _context.Sessions
            .Where(s => s.HallId == hallId && s.EndTime >= now)
            .Select(s => s.Id)
            .ToListAsync();

        return await _context.Tickets
            .Include(t => t.Order)
            .Where(t => activeSessionIds.Contains(t.SessionId) && t.Order.Status == OrderStatus.Paid)
            .Select(t => t.SeatId)
            .Distinct()
            .CountAsync();
    }

    public async Task<List<HallSessionInfoDTO>> GetUpcomingSessionsAsync(int hallId, int count = 10)
    {
        var now = DateTime.Now;
        var sessions = await _context.Sessions
            .Where(s => s.HallId == hallId && s.EndTime >= now)
            .Include(s => s.Movie)
            .OrderBy(s => s.StartTime)
            .Take(count)
            .AsNoTracking()
            .Select(s => new HallSessionInfoDTO
            {
                SessionId = s.Id,
                MovieName = s.Movie.Title,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                BasePrice = s.BasePrice,
                IsOngoing = s.StartTime <= now && s.EndTime >= now,
                IsStartingSoon = s.StartTime > now && s.StartTime <= now.AddHours(2)
            })
            .ToListAsync();

        return sessions;
    }

    public async Task<HallStatisticsDTO> GetHallStatisticsAsync(int hallId, int? days = null)
    {
        var now = DateTime.Now;
        DateTime? fromDate = days.HasValue ? now.AddDays(-days.Value) : null;

        // Get total seats
        var totalSeats = await _context.Seats.CountAsync(s => s.HallId == hallId);

        // Build session query with optional date filter
        var sessionsQuery = _context.Sessions
            .Where(s => s.HallId == hallId && s.EndTime < now);

        if (fromDate.HasValue)
        {
            sessionsQuery = sessionsQuery.Where(s => s.StartTime >= fromDate.Value);
        }

        var totalSessions = await sessionsQuery.CountAsync();

        // Get session IDs for ticket counting
        var sessionIds = await sessionsQuery.Select(s => s.Id).ToListAsync();

        // Count tickets sold for these sessions (only Paid orders)
        var totalTicketsSold = await _context.Tickets
            .Include(t => t.Order)
            .Where(t => sessionIds.Contains(t.SessionId) && t.Order.Status == OrderStatus.Paid)
            .CountAsync();

        // Calculate average occupancy rate
        var avgOccupancy = totalSessions > 0 && totalSeats > 0
            ? (decimal)totalTicketsSold / ((decimal)totalSessions * totalSeats) * 100
            : 0;

        // Calculate total revenue (tickets * session base price, only Paid orders)
        var revenue = await _context.Tickets
            .Include(t => t.Order)
            .Where(t => sessionIds.Contains(t.SessionId) && t.Order.Status == OrderStatus.Paid)
            .Join(_context.Sessions,
                ticket => ticket.SessionId,
                session => session.Id,
                (ticket, session) => session.BasePrice)
            .SumAsync();

        return new HallStatisticsDTO
        {
            TotalSeats = totalSeats,
            TotalSessions = totalSessions,
            TotalTicketsSold = totalTicketsSold,
            AverageOccupancyRate = Math.Round(avgOccupancy, 2),
            TotalRevenue = revenue
        };
    }
}
