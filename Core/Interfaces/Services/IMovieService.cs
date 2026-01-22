using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IMovieService
{
    List<MovieDTO> GetAllMovies();
    MovieDTO? GetMovie(int id);
    void CreateMovie(MovieDTO movie);
    void UpdateMovie(MovieDTO movie);
    void DeleteMovie(int id);
}