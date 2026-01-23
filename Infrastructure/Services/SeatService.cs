using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class SeatService : ISeatService
{
    private readonly CinemaAppDbContext _context;
    private readonly IMapper _mapper;

    public SeatService(CinemaAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<SeatDTO>> GetSeatsByHallIdAsync(int hallId)
    {
        var seats = await _context.Seats
            .Where(s => s.HallId == hallId)
            .OrderBy(s => s.RowNumber)
            .ThenBy(s => s.SeatNumber)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<List<SeatDTO>>(seats);
    }

    public async Task GenerateSeatsAsync(SeatGenerationDTO generationDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var existingSeats = _context.Seats.Where(s => s.HallId == generationDto.HallId);
            _context.Seats.RemoveRange(existingSeats);
            await _context.SaveChangesAsync();

            var newSeats = new List<Seat>();
            
            for (byte row = 1; row <= generationDto.Rows; row++)
            {
                for (byte seatNum = 1; seatNum <= generationDto.SeatsPerRow; seatNum++)
                {
                    newSeats.Add(new Seat
                    {
                        HallId = generationDto.HallId,
                        RowNumber = row,
                        SeatNumber = seatNum,
                        Type = SeatType.Regular
                    });
                }
            }

            await _context.Seats.AddRangeAsync(newSeats);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteSeatAsync(int id)
    {
        var seat = await _context.Seats.FindAsync(id);
        if (seat != null)
        {
            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();
        }
    }
}