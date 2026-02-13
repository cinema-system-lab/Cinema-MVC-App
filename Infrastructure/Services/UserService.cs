using AutoMapper;
using Core.DTOs;
using Core.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly CinemaAppDbContext _context;
    private readonly IMapper _mapper;

    public UserService(CinemaAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserDTO?> GetUserByIdAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user == null ? null : _mapper.Map<UserDTO>(user);
    }

    public async Task<UserDTO?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
        return user == null ? null : _mapper.Map<UserDTO>(user);
    }

    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        return _mapper.Map<IEnumerable<UserDTO>>(users);
    }
}