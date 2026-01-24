using AutoMapper;
using Core.DTOs;
using Core.Entities;
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
        var hall = await _context.Halls.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return hall == null ? null : _mapper.Map<HallDTO>(hall);
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

        _mapper.Map(hall, entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteHallAsync(int id)
    {
        var entity = await _context.Halls.FindAsync(id);
        if (entity == null) return;

        _context.Halls.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
