using Cinema_MVC_App.Models;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Controllers;

public class MoviesController : Controller
{
    private readonly IMovieService _movieService;
    private readonly ISessionService _sessionService;
    private readonly IHallService _hallService;

    public MoviesController(
        IMovieService movieService,
        ISessionService sessionService,
        IHallService hallService)
    {
        _movieService = movieService;
        _sessionService = sessionService;
        _hallService = hallService;
    }

    // GET: /Movies
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var movies = await _movieService.GetAllMoviesAsync();
        return View(movies);
    }

    // GET: /Movies/Details/{id}
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var movie = await _movieService.GetMovieAsync(id);
        if (movie == null)
            return NotFound();

        var allSessions = await _sessionService.GetAllSessionsAsync();
        var allHalls = await _hallService.GetAllHallsAsync();

        var viewModel = new MovieDetailsVM
        {
            Movie = movie,
            HallNames = allHalls.ToDictionary(h => h.Id, h => h.Name),
            SessionsByDate = allSessions
                .Where(s => s.MovieId == id && s.StartTime >= DateTime.Now)
                .GroupBy(s => s.StartTime.Date)
                .OrderBy(g => g.Key)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(s => s.StartTime).ToList()
                )
        };

        return View(viewModel);
    }

}