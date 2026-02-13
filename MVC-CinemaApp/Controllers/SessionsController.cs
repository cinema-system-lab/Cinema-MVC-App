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