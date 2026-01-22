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
    public IActionResult Index()
    {
        var movies = _movieService.GetAllMovies();
        return View(movies);
    }

    // GET: /Movies/Details/{id}
    public IActionResult Details(int id)
    {
        var movie = _movieService.GetMovie(id);
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
    public IActionResult Create(MovieDTO movie, int[]? genres)
    {
        if (genres != null && genres.Length > 0)
        {
            movie.Genres = (GenreType)genres.Aggregate(0, (current, next) => current | next);
        }
        if (!ModelState.IsValid) return View(movie);

        _movieService.CreateMovie(movie);
        TempData["SuccessMessage"] = "Movie successfully created!";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Movies/Edit/{id}
    public IActionResult Edit(int id)
    {
        var movie = _movieService.GetMovie(id);
        if (movie == null) return NotFound();

        return View(movie);
    }

    // POST: /Movies/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(MovieDTO movie, int[] selectedGenres)
    {
        var combinedGenres = (selectedGenres != null && selectedGenres.Length > 0)
            ? selectedGenres.Aggregate(0, (current, next) => current | next)
            : 0;
        movie.Genres = (GenreType)combinedGenres;
        
        if (!ModelState.IsValid) return View(movie);

        _movieService.UpdateMovie(movie);
        TempData["SuccessMessage"] = "Movie successfully updated!";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Movies/Delete/{id}
    public IActionResult Delete(int id)
    {
        var movie = _movieService.GetMovie(id);
        if (movie == null) return NotFound();
        return View(movie);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        _movieService.DeleteMovie(id);
        TempData["SuccessMessage"] = "Movie deleted!";
        return RedirectToAction(nameof(Index));
    }

}