using Core.DTOs.TMDB;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Controllers;

[Authorize(Roles = "Admin")] 
public class AdminTmdbController : Controller
{
    private readonly ITmdbService _tmdbService;
    private readonly IMovieService _movieService; 

    public AdminTmdbController(ITmdbService tmdbService, IMovieService movieService)
    {
        _tmdbService = tmdbService;
        _movieService = movieService;
    }

    // GET: /AdminTmdb
    public async Task<IActionResult> Index()
    {
        var movies = await _tmdbService.GetPopularMoviesAsync();
        return View(movies);
    }

    // POST: /AdminTmdb/Import/{id}
    [HttpPost]
    public async Task<IActionResult> Import(int id)
    {
        try
        {
            var tmdbMovie = await _tmdbService.GetMovieByIdAsync(id);
            if (tmdbMovie == null) return NotFound();


            var movieToCreate = new Core.DTOs.MovieDTO
            {
                Title = tmdbMovie.Title,
                Description = tmdbMovie.Overview,
                ReleaseDate = DateTime.TryParse(tmdbMovie.ReleaseDate, out var date) ? date : DateTime.Now,
                DurationMinutes = 120, 
                Director = "Unknown", 
                Rating = (decimal)tmdbMovie.VoteAverage,
                PosterUrl = tmdbMovie.FullPosterUrl,
                TrailerUrl = "",
                AgeRestriction = 12,
                IsActive = true,
                Genres = GenreType.None 
            };

            await _movieService.CreateMovieAsync(movieToCreate, new int[0]);

            TempData["SuccessMessage"] = $"Movie '{tmdbMovie.Title}' imported successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Import failed: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}