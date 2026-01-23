using Core.DTOs;

namespace Core.Interfaces.Services;

public interface ISeatService
{
    Task<List<SeatDTO>> GetSeatsByHallIdAsync(int hallId);
    Task AddSeatAsync(SeatDTO seatDto);
    Task DeleteSeatAsync(int id);
    Task DeleteAllSeatsByHallIdAsync(int hallId);
    Task ToggleSeatTypeAsync(int id);
    Task GenerateSeatsAsync(SeatGenerationDTO generationDto);

}