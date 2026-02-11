using Core.DTOs.TMDB;

namespace Core.Interfaces.Services;

public interface ITmdbService
{
    Task<List<TmdbMovieDto>> GetPopularMoviesAsync();
    Task<TmdbMovieDto?> GetMovieByIdAsync(int tmdbId);
    Task<List<TmdbMovieDto>> SearchMoviesAsync(string query);
}
