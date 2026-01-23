using Core.DTOs;
using Core.Entities;
using Core.Interfaces.Services;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class MovieService : IMovieService
{
    private readonly CinemaAppDbContext _context;
    private readonly IMapper _mapper;

    public MovieService(CinemaAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<MovieDTO>> GetAllMoviesAsync()
    {
        var movies = await _context.Movies.Where(x => x.IsActive).ToListAsync();
        return _mapper.Map<List<MovieDTO>>(movies);
    }

    public async Task<MovieDTO?> GetMovieAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        return movie == null ? null : _mapper.Map<MovieDTO>(movie);
    }

    public async Task CreateMovieAsync(MovieDTO movie)
    {
        var entity = _mapper.Map<Movie>(movie);
        _context.Movies.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateMovieAsync(MovieDTO movie)
    {
        var entity = await _context.Movies.FindAsync(movie.Id);
        if (entity == null) return;

        _mapper.Map(movie, entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMovieAsync(int id)
    {
        var entity = await _context.Movies.FindAsync(id);
        if (entity == null) return;

        entity.IsActive = false;
        await _context.SaveChangesAsync();
    }
}