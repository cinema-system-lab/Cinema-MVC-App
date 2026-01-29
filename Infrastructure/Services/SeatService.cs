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

    private async Task ValidateNoSessionsAsync(int hallId)
    {
        if (await HasAnySessionsAsync(hallId))
        {
            throw new InvalidOperationException("Cannot modify seats because there are sessions associated with this hall.");
        }
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
        // 1. Перевірка
        await ValidateNoSessionsAsync(generationDto.HallId);

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

    public async Task AddSeatAsync(SeatDTO seatDto)
    {
        // 1. Перевірка
        await ValidateNoSessionsAsync(seatDto.HallId);

        bool exists = await _context.Seats.AnyAsync(s => 
            s.HallId == seatDto.HallId && 
            s.RowNumber == seatDto.RowNumber && 
            s.SeatNumber == seatDto.SeatNumber);

        if (exists)
        {
            throw new InvalidOperationException($"Seat {seatDto.RowNumber}-{seatDto.SeatNumber} already exists.");
        }

        var seat = _mapper.Map<Seat>(seatDto);
        _context.Seats.Add(seat);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSeatAsync(int id)
    {
        var seat = await _context.Seats.FindAsync(id);
        if (seat != null)
        {
            await ValidateNoSessionsAsync(seat.HallId);

            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task ToggleSeatTypeAsync(int id)
    {
        var seat = await _context.Seats.FindAsync(id);
        if (seat != null)
        {
            await ValidateNoSessionsAsync(seat.HallId);

            seat.Type = seat.Type == SeatType.Regular ? SeatType.Premium : SeatType.Regular;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllSeatsByHallIdAsync(int hallId)
    {
        await ValidateNoSessionsAsync(hallId);

        var seats = _context.Seats.Where(s => s.HallId == hallId);
        _context.Seats.RemoveRange(seats);
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> HasAnySessionsAsync(int hallId)
    {
        return await _context.Sessions.AnyAsync(s => s.HallId == hallId);
    }
}