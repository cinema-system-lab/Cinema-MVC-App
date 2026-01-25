using AutoMapper;
using Core.DTOs;
using Core.Entities;
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
        
        var overlapExists = await _context.Sessions.AnyAsync(s =>
            s.Id != session.Id &&
            s.HallId == session.HallId &&
            session.StartTime < s.EndTime &&
            session.EndTime > s.StartTime);
        
        if (overlapExists)
            throw new InvalidOperationException("Session overlaps with another session in the same hall");
        
        _mapper.Map(session, entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSessionAsync(int id)
    {
        var entity = await _context.Sessions.FindAsync(id);
        if (entity == null) return;
        
        var hasTickets = await _context.Tickets.AnyAsync(t => t.SessionId == id);
        if (hasTickets)
            throw new InvalidOperationException("Cannot delete session with sold tickets");

        _context.Sessions.Remove(entity);
        await _context.SaveChangesAsync();
    }
}