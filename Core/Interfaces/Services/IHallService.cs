using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IHallService
{
    Task<List<HallDTO>> GetAllHallsAsync();
    Task<HallDTO?> GetHallAsync(int id);
    Task CreateHallAsync(HallDTO hall);
    Task UpdateHallAsync(HallDTO hall);
    Task DeleteHallAsync(int id);
}
