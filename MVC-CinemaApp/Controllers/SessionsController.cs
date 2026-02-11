using Cinema_MVC_App.Models;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Controllers;

public class SessionsController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IMovieService _movieService;
    private readonly IHallService _hallService;

    public SessionsController(
        ISessionService sessionService,
        IMovieService movieService,
        IHallService hallService)
    {
        _sessionService = sessionService;
        _movieService = movieService;
        _hallService = hallService;
    }

    // GET: /Sessions
    public async Task<IActionResult> Index()
    {
        var sessions = await _sessionService.GetAllSessionsAsync();
        var movies = (await _movieService.GetAllMoviesAsync()).Where(m => m.IsActive);
        var halls = await _hallService.GetAllHallsAsync();

        var model = new SessionsListVM
        {
            Sessions = sessions,
            Movies = movies.ToDictionary(m => m.Id),
            Halls = halls.ToDictionary(h => h.Id)
        };

        return View(model);
    }

    // GET: /Sessions/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var session = await _sessionService.GetSessionAsync(id);
        if (session == null) return NotFound();

        var movie = await _movieService.GetMovieAsync(session.MovieId);
        var hall = await _hallService.GetHallAsync(session.HallId);

        ViewBag.MovieName = movie?.Title ?? "Unknown";
        ViewBag.PosterUrl = movie?.PosterUrl;
        ViewBag.HallName = hall?.Name ?? "Unknown";
        ViewBag.HallType = hall?.Type ?? Core.Enums.HallType.Standard;

        return View(session);
    }

    // GET: /Sessions/Schedule
    public async Task<IActionResult> Schedule()
    {
        var allSessions = await _sessionService.GetAllSessionsAsync();
        var movies = await _movieService.GetAllMoviesAsync();
        var halls = await _hallService.GetAllHallsAsync();

        var model = new SessionsListVM
        {
            Sessions = allSessions.Where(s => s.StartTime >= DateTime.Today).OrderBy(s => s.StartTime),
            Movies = movies.ToDictionary(m => m.Id),
            Halls = halls.ToDictionary(h => h.Id)
        };

        return View(model);
    }
}