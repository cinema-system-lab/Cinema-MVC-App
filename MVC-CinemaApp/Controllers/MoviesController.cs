using Microsoft.AspNetCore.Mvc;
using Core.DTOs;
using Core.Interfaces.Services;
using Core.Enums;

namespace Cinema_MVC_App.Controllers;

public class MoviesController : Controller
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
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
        if (movie == null) return NotFound();
        return View(movie);
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
        if (selectedGenres != null && selectedGenres.Length > 0)
        {
            movie.Genres = (GenreType)selectedGenres.Aggregate(0, (current, next) => current | next);
        }

        if (!ModelState.IsValid) return View(movie);

        try
        {
            await _movieService.CreateMovieAsync(movie);
            TempData["SuccessMessage"] = "Movie successfully created!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while creating the movie.");
            return View(movie);
        }
    }

    // GET: /Movies/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var movie = await _movieService.GetMovieAsync(id);
        if (movie == null) return NotFound();

        return View(movie);
    }

    // POST: /Movies/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MovieDTO movie, int[]? selectedGenres)
    {
        var combinedGenres = (selectedGenres != null && selectedGenres.Length > 0)
            ? selectedGenres.Aggregate(0, (current, next) => current | next)
            : 0;
        movie.Genres = (GenreType)combinedGenres;

        if (!ModelState.IsValid) return View(movie);

        try
        {
            await _movieService.UpdateMovieAsync(movie);
            TempData["SuccessMessage"] = "Movie successfully updated!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while updating the movie.");
            return View(movie);
        }
    }

    // GET: /Movies/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _movieService.GetMovieAsync(id);
        if (movie == null) return NotFound();
        return View(movie);
    }

    // POST: /Movies/DeleteConfirmed
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _movieService.DeleteMovieAsync(id);
        TempData["SuccessMessage"] = "Movie deleted!";
        return RedirectToAction(nameof(Index));
    }
}