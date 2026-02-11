using Core.Constants;
using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var sessions = await _sessionService.GetAllSessionsAsync();
        var movies = (await _movieService.GetAllMoviesAsync())
                    .Where(m => m.IsActive);
        var halls = await _hallService.GetAllHallsAsync();

        ViewBag.MovieNames = movies.ToDictionary(m => m.Id, m => m.Title);
        ViewBag.HallNames = halls.ToDictionary(h => h.Id, h => h.Name);
        
        return View(sessions);
    }

    // GET: /Sessions/Details/{id}
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var session = await _sessionService.GetSessionAsync(id);
        if (session == null) return NotFound();
        
        var movie = await _movieService.GetMovieAsync(session.MovieId);
        var hall = await _hallService.GetHallAsync(session.HallId);

        ViewBag.MovieName = movie?.Title ?? "Unknown";
        ViewBag.HallName = hall?.Name ?? "Unknown";
        
        return View(session);
    }
}
