using Core.DTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Areas.Admin.Controllers;

public class MoviesController : BaseAdminController
{
    private readonly IMovieService _movieService;
    private readonly ITmdbService _tmdbService;

    public MoviesController(IMovieService movieService, ITmdbService tmdbService)
    {
        _movieService = movieService;
        _tmdbService = tmdbService;
    }

    // GET: /Movies or /Movies/Index
    public async Task<IActionResult> Index()
    {
        var movies = await _movieService.GetAllMoviesAsync();
        return View(movies);
    }

    // GET: /Movies/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var movie = await _movieService.GetMovieAsync(id);
        if (movie == null)
            return NotFound();

        return View(movie);
    }

    [HttpGet]
    public async Task<IActionResult> CreateFromTmdb(int tmdbId)
    {
        var tmdbMovie = await _tmdbService.GetMovieByIdAsync(tmdbId);

        if (tmdbMovie == null)
        {
            TempData["ErrorMessage"] = "Movie not found in TMDB.";
            
            return RedirectToAction("Index", "Tmdb");
        }

        GenreType mappedGenres = GenreType.None;
        if (tmdbMovie.Genres != null)
        {
            foreach (var g in tmdbMovie.Genres)
            {
                if (Enum.TryParse<GenreType>(g.Name.Replace(" ", ""), true, out var result))
                {
                    mappedGenres |= result; 
                }
            }
        }

        var model = new MovieDTO
        {
            Title = tmdbMovie.Title,
            Description = tmdbMovie.Overview,
            ReleaseDate = DateTime.TryParse(tmdbMovie.ReleaseDate, out var date) ? date : DateTime.Now,
            DurationMinutes = (short)(tmdbMovie.Runtime ?? 120), 
            Rating = Math.Round((decimal)tmdbMovie.VoteAverage, 1, MidpointRounding.AwayFromZero),
            PosterUrl = tmdbMovie.FullPosterUrl,
            Genres = mappedGenres, 

            Director = "",
            Actors = "",
            AgeRestriction = 0,
            IsActive = true
        };

      
        return View("Create", model);
    }

    // GET: /Movies/Create
    public IActionResult Create()
    {
        var movie = new MovieDTO
        {
            Genres = GenreType.None
        };
        return View(movie);
    }

    // POST: /Movies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MovieDTO movie, int[]? selectedGenres)
    {
        ModelState.Remove(nameof(movie.Genres));
        
        if (selectedGenres != null && selectedGenres.Length > 0)
        {
            movie.Genres = (GenreType)selectedGenres.Sum();
        }
        else
        {
            ModelState.AddModelError(nameof(selectedGenres), "Please select at least one genre.");
        }

        if (!ModelState.IsValid)
            return View(movie);

        try
        {
            await _movieService.CreateMovieAsync(movie, selectedGenres ?? Array.Empty<int>());
            TempData["SuccessMessage"] = "Movie successfully created!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return View(movie);
        }
    }

    // GET: /Movies/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var movie = await _movieService.GetMovieAsync(id);
        if (movie == null)
            return NotFound();

        return View(movie);
    }

    // POST: /Movies/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MovieDTO movie, int[]? selectedGenres)
    {
        ModelState.Remove(nameof(movie.Genres));
        
        if (selectedGenres != null && selectedGenres.Length > 0)
        {
            movie.Genres = (GenreType)selectedGenres.Sum();
        }
        else
        {
            ModelState.AddModelError(nameof(selectedGenres), "Please select at least one genre.");
        }

        if (!ModelState.IsValid)
            return View(movie);

        try
        {
            await _movieService.UpdateMovieAsync(movie, selectedGenres ?? Array.Empty<int>());
            TempData["SuccessMessage"] = "Movie successfully updated!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
            return View(movie);
        }
    }

    // GET: /Movies/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _movieService.GetMovieAsync(id);
        if (movie == null)
            return NotFound();

        return View(movie);
    }

    // POST: /Movies/DeleteConfirmed
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _movieService.DeleteMovieAsync(id);
            TempData["SuccessMessage"] = "Movie deleted!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: /Movies/SearchTmdbJson?query=avatar
    [HttpGet]
    public async Task<IActionResult> SearchTmdbJson(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Json(new List<object>());

        var results = await _tmdbService.SearchMoviesAsync(query);

        var jsonResult = results.Select(m => new
        {
            id = m.Id,
            title = m.Title,
            year = string.IsNullOrEmpty(m.ReleaseDate) ? "N/A" : m.ReleaseDate.Substring(0, 4),
            poster = m.FullPosterUrl,
            overview = m.Overview
        });

        return Json(jsonResult);
    }
}