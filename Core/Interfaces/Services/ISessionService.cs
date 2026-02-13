using Core.DTOs;

namespace Core.Interfaces.Services;
public interface ISessionService
{
    Task<List<SessionDTO>> GetAllSessionsAsync();
    Task<SessionDTO?> GetSessionAsync(int id);
    Task CreateSessionAsync(SessionDTO session);
    Task UpdateSessionAsync(SessionDTO session);
    Task DeleteSessionAsync(int id);
}
