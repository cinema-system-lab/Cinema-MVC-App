using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IMovieService
{
    Task<List<MovieDTO>> GetAllMoviesAsync();
    Task<MovieDTO?> GetMovieAsync(int id);
    Task CreateMovieAsync(MovieDTO movie);
    Task UpdateMovieAsync(MovieDTO movie);
    Task DeleteMovieAsync(int id);
}