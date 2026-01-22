using Core.DTOs;
using Core.Entities;
using Core.Interfaces.Services;
using AutoMapper;
using Infrastructure.Data;

namespace DataAccessLayer.Services;

public class MovieService : IMovieService
{
    private readonly CinemaAppDbContext _context;
    private readonly IMapper _mapper;

    public MovieService(CinemaAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public List<MovieDTO> GetAllMovies()
    {
        var movies = _context.Movies.Where(x => x.IsActive).ToList();
        return _mapper.Map<List<MovieDTO>>(movies);
    }

    public MovieDTO? GetMovie(int id)
    {
        var movie = _context.Movies.Find(id);
        return movie == null ? null : _mapper.Map<MovieDTO>(movie);
    }

    public void CreateMovie(MovieDTO movie)
    {
        var entity = _mapper.Map<Movie>(movie);
        _context.Movies.Add(entity);
        _context.SaveChanges();
    }

    public void UpdateMovie(MovieDTO movie)
    {
        var entity = _context.Movies.Find(movie.Id);
        if (entity == null) return;

        _mapper.Map(movie, entity);
        _context.SaveChanges();
    }

    public void DeleteMovie(int id)
    {
        var entity = _context.Movies.Find(id);
        if (entity == null) return;

        _context.Movies.Remove(entity);
        _context.SaveChanges();
    }
}