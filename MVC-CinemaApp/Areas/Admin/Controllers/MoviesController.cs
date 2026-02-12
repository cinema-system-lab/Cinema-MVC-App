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
    public async Task<IActionResult> Index(string status = "all")
    {
        var movies = await _movieService.GetAllMoviesAsync();
    
        movies = status switch
        {
            "active" => movies.Where(m => m.IsActive),
            "inactive" => movies.Where(m => !m.IsActive),
            _ => movies
        };
    
        ViewData["CurrentStatus"] = status;
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

        string trailerUrl = "";
        if (tmdbMovie.Videos?.Results != null)
        {
            var video = tmdbMovie.Videos.Results.FirstOrDefault(v => v.Type == "Trailer" && v.Site == "YouTube")
                     ?? tmdbMovie.Videos.Results.FirstOrDefault(v => v.Type == "Teaser" && v.Site == "YouTube")
                     ?? tmdbMovie.Videos.Results.FirstOrDefault(v => v.Site == "YouTube");

            if (video != null)
            {
                trailerUrl = $"https://www.youtube.com/watch?v={video.Key}";
            }
        }

        var genreMap = new Dictionary<int, GenreType>
        {
            { 28, GenreType.Action },
            { 18, GenreType.Drama },
            { 35, GenreType.Comedy },
            { 27, GenreType.Horror },
            { 878, GenreType.SciFi },
            { 99, GenreType.Documentary },
            { 53, GenreType.Thriller },
            { 14, GenreType.Fantasy },
            { 16, GenreType.Animation },
            { 12, GenreType.Adventure },
            { 80, GenreType.Crime },
            { 10751, GenreType.Family },
            { 9648, GenreType.Mystery },
            { 10749, GenreType.Romance },
            { 37, GenreType.Western },
            { 10752, GenreType.War },
            { 36, GenreType.History },
            { 10402, GenreType.Music }

        };

        GenreType mappedGenres = GenreType.None;
        if (tmdbMovie.Genres != null)
        {
            foreach (var g in tmdbMovie.Genres)
            {
                if (genreMap.TryGetValue(g.Id, out var genreValue))
                {
                    mappedGenres |= genreValue;
                }
            }
        }

        int ageLimit = 12;
        if (tmdbMovie.ReleaseDates?.Results != null)
        {
            var usCertification = tmdbMovie.ReleaseDates.Results
                .FirstOrDefault(r => r.CountryCode == "US")?.ReleaseDates
                .FirstOrDefault(d => !string.IsNullOrEmpty(d.Certification))?.Certification;

            if (!string.IsNullOrEmpty(usCertification))
            {
                ageLimit = usCertification.ToUpper() switch
                {
                    "G" => 0,
                    "PG" => 6,
                    "PG-13" => 12,
                    "R" => 16,
                    "NC-17" => 18,
                    _ => int.TryParse(usCertification, out int val) ? val : 12
                };
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
            TrailerUrl = trailerUrl,
            Director = tmdbMovie.Credits?.Crew.FirstOrDefault(c => c.Job == "Director")?.Name ?? "Unknown",
            Actors = string.Join(", ", tmdbMovie.Credits?.Cast.Take(5).Select(c => c.Name) ?? new List<string>()),
            AgeRestriction = (byte)ageLimit,
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
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(movie);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Unexpected error: {ex.Message}");
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
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            var movie = await _movieService.GetMovieAsync(id);
            if (movie == null) return NotFound();

            ModelState.AddModelError(string.Empty, ex.Message);

            return View("Delete", movie);
        }
    }

    // GET: /Movies/SearchTmdbJson?query=avatar
    [HttpGet]
    public async Task<IActionResult> SearchTmdbJson(string query)
    {
        try
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
        catch
        {
            return Json(new List<object>());
        }
    }
}