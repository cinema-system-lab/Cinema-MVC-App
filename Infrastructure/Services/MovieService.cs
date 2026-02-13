using Core.DTOs;
using Core.Entities;
using Core.Interfaces.Services;
using AutoMapper;
using Infrastructure.Data;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class MovieService : IMovieService
    {
        private readonly CinemaAppDbContext _context;
        private readonly IMapper _mapper;

        public MovieService(CinemaAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MovieDTO>> GetAllMoviesAsync()
        {
            var movies = await _context.Movies.ToListAsync();
            return _mapper.Map<IEnumerable<MovieDTO>>(movies);
        }

        public async Task<MovieDTO?> GetMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            return movie == null ? null : _mapper.Map<MovieDTO>(movie);
        }

        public async Task CreateMovieAsync(MovieDTO movieDto, int[] selectedGenres)
        {
            if (selectedGenres != null && selectedGenres.Length > 0)
            {
                movieDto.Genres = (GenreType)selectedGenres.Aggregate(0, (current, next) => current | next);
            }
            else
            {
                movieDto.Genres = GenreType.None;
            }

            if (movieDto.DurationMinutes > 300)
            {
                throw new InvalidOperationException("Movie duration cannot exceed 5 hours.");
            }

            var entity = _mapper.Map<Movie>(movieDto);
            _context.Movies.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMovieAsync(MovieDTO movieDto, int[] selectedGenres)
        {
            var entity = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == movieDto.Id);
            if (entity == null) return;

            var hasSessions = await _context.Sessions.AnyAsync(s => s.MovieId == movieDto.Id);

            if (hasSessions)
            {
                throw new InvalidOperationException("Cannot edit movie that already has scheduled sessions.");
            }

            if (selectedGenres != null && selectedGenres.Length > 0)
            {
                movieDto.Genres = (GenreType)selectedGenres.Aggregate(0, (current, next) => current | next);
            }
            else
            {
                movieDto.Genres = GenreType.None;
            }

            if (movieDto.DurationMinutes > 300)
            {
                throw new InvalidOperationException("Movie duration cannot exceed 5 hours.");
            }

            var existingEntity = await _context.Movies.FindAsync(movieDto.Id);
            _mapper.Map(movieDto, existingEntity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMovieAsync(int id)
        {
            var entity = await _context.Movies.FindAsync(id);
            if (entity == null) return;

            var hasSessions = await _context.Sessions.AnyAsync(s => s.MovieId == id);
            if (hasSessions)
            {
                throw new InvalidOperationException("Cannot delete movie with existing sessions");
            }

            _context.Movies.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
