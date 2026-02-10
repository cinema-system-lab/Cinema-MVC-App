using Core.DTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Areas.Admin.Controllers;

public class MoviesController : BaseAdminController
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
        if (movie == null)
            return NotFound();

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
}