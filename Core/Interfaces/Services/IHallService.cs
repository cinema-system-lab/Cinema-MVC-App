using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IHallService
{
    Task<List<HallDTO>> GetAllHallsAsync();
    Task<HallDTO?> GetHallAsync(int id);
    HallDTO CreateNewHallDTO();
    Task CreateHallAsync(HallDTO hall);
    Task UpdateHallAsync(HallDTO hall);
    Task DeleteHallAsync(int id);
    Task<int> GetSeatsCountAsync(int hallId);
    Task<int> GetActiveSessionsCountAsync(int hallId);
    Task<int> GetBookedSeatsCountAsync(int hallId);
    Task<List<HallSessionInfoDTO>> GetUpcomingSessionsAsync(int hallId, int count = 10);
    Task<HallStatisticsDTO> GetHallStatisticsAsync(int hallId, int? days = null);
}
