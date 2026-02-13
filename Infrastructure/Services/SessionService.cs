using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;
public class SessionService : ISessionService
{
    private readonly CinemaAppDbContext _context;
    private readonly IMapper _mapper;
    
    public SessionService(CinemaAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<SessionDTO>> GetAllSessionsAsync()
    {
        var sessions = await _context.Sessions.AsNoTracking().ToListAsync();
        return _mapper.Map<List<SessionDTO>>(sessions);
    }

    public async Task<SessionDTO?> GetSessionAsync(int id)
    {
        var session = await _context.Sessions.FindAsync(id);
        return session == null ? null : _mapper.Map<SessionDTO>(session);
    }

    public async Task CreateSessionAsync(SessionDTO session)
    {

        var duration = (session.EndTime - session.StartTime).TotalMinutes;

        if (duration > 300)
        {
            throw new InvalidOperationException("The session duration cannot exceed 5 hours (300 minutes).");
        }

        if (duration <= 0)
        {
            throw new InvalidOperationException("End time must be after start time.");
        }

        var overlapExists = await _context.Sessions.AnyAsync(s =>
            s.HallId == session.HallId &&
            session.StartTime < s.EndTime &&
            session.EndTime > s.StartTime);

        if (overlapExists)
            throw new InvalidOperationException("Session overlaps with another session in the same hall");

        var entity = _mapper.Map<Session>(session);
        _context.Sessions.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSessionAsync(SessionDTO session)
    {
        var entity = await _context.Sessions.FindAsync(session.Id);
        if (entity == null) return;

        var duration = (session.EndTime - session.StartTime).TotalMinutes;

        if (duration > 300)
        {
            throw new InvalidOperationException("The session duration cannot exceed 5 hours (300 minutes).");
        }

        if (duration <= 0)
        {
            throw new InvalidOperationException("End time must be after start time.");
        }

        var now = DateTime.Now;
        if (entity.StartTime <= now)
        {
            throw new InvalidOperationException("Cannot update a session that has already started.");
        }

        var overlapExists = await _context.Sessions.AnyAsync(s =>
            s.Id != session.Id &&
            s.HallId == session.HallId &&
            session.StartTime < s.EndTime &&
            session.EndTime > s.StartTime);

        if (overlapExists)
            throw new InvalidOperationException("Session overlaps with another session in the same hall");

        var hasTickets = await _context.Tickets.AnyAsync(t =>
            t.SessionId == session.Id &&
            (t.Order.Status == OrderStatus.Paid || t.Order.Status == OrderStatus.Pending));
        
        if (hasTickets)
            throw new InvalidOperationException("Cannot update session with sold tickets");

        _mapper.Map(session, entity);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteSessionAsync(int id)
    {
        var entity = await _context.Sessions.FindAsync(id);
        if (entity == null) return;

        var now = DateTime.Now;

        if (entity.EndTime < now)
        {
            throw new InvalidOperationException("Cannot delete a completed session. It must remain in history.");
        }

        if (entity.StartTime <= now && entity.EndTime >= now)
        {
            throw new InvalidOperationException("Cannot delete an ongoing session.");
        }

        var hasTickets = await _context.Tickets.AnyAsync(t =>
                t.SessionId == id &&
                (t.Order.Status == OrderStatus.Paid || t.Order.Status == OrderStatus.Pending));
        
        if (hasTickets)
        {
            throw new InvalidOperationException("Cannot delete session with sold tickets. Please refund or cancel orders first.");
        }

        _context.Sessions.Remove(entity);
        await _context.SaveChangesAsync();
    }
}