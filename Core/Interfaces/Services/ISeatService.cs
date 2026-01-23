using Core.DTOs;

namespace Core.Interfaces.Services;

public interface ISeatService
{
    Task<List<SeatDTO>> GetSeatsByHallIdAsync(int hallId);
    
    Task GenerateSeatsAsync(SeatGenerationDTO generationDto);
    
    Task DeleteSeatAsync(int id);
}