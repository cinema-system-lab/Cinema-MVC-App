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
            .Include(h => h.Seats)
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

        // Check if there are any active sessions in this hall
        var now = DateTime.Now;
        var hasActiveSessions = await _context.Sessions
            .AnyAsync(s => s.HallId == hall.Id && s.StartTime >= now);
        
        if (hasActiveSessions)
        {
            throw new InvalidOperationException("Cannot edit hall because it has active or scheduled sessions.");
        }

        _mapper.Map(hall, entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteHallAsync(int id)
    {
        var entity = await _context.Halls.FindAsync(id);
        if (entity == null) return;

        // Check if there are any sessions in this hall
        var hasSessions = await _context.Sessions.AnyAsync(s => s.HallId == id);
        if (hasSessions)
        {
            throw new InvalidOperationException("Cannot delete hall because it has scheduled sessions.");
        }

        // Check if there are any seats in this hall
        var hasSeats = await _context.Seats.AnyAsync(s => s.HallId == id);
        if (hasSeats)
        {
            throw new InvalidOperationException("Cannot delete hall because it has seats.");
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
}
