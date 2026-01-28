using Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieDTO>> GetAllMoviesAsync();
        Task<MovieDTO?> GetMovieAsync(int id);
        Task CreateMovieAsync(MovieDTO movieDto, int[] selectedGenres);
        Task UpdateMovieAsync(MovieDTO movieDto, int[] selectedGenres);
        Task DeleteMovieAsync(int id);
    }
}