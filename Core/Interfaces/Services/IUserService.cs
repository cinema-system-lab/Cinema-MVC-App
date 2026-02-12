using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IUserService
{
    Task<UserDTO?> GetUserByIdAsync(string userId);
    Task<UserDTO?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDTO>> GetAllUsersAsync();
}