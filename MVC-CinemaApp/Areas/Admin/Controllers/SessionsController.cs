using Core.Constants;
using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema_MVC_App.Areas.Admin.Controllers;

public class SessionsController : BaseAdminController
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
        var movies = (await _movieService.GetAllMoviesAsync())
                    .Where(m => m.IsActive);
        var halls = await _hallService.GetAllHallsAsync();

        ViewBag.MovieNames = movies.ToDictionary(m => m.Id, m => m.Title);
        ViewBag.HallNames = halls.ToDictionary(h => h.Id, h => h.Name);
        
        return View(sessions);
    }

    // GET: /Sessions/Details/{id}
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

    // GET: /Sessions/Create
    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        var session = new SessionDTO
        {
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(3)
        };
        return View(session);
    }

    // POST: /Sessions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SessionDTO session)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(session.MovieId, session.HallId);
            return View(session);
        }

        try
        {
            await _sessionService.CreateSessionAsync(session);
            TempData["SuccessMessage"] = "Session successfully created!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateDropdowns(session.MovieId, session.HallId);
            return View(session);
        }
    }

    // GET: /Sessions/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var session = await _sessionService.GetSessionAsync(id);
        if (session == null) return NotFound();

        await PopulateDropdowns(session.MovieId, session.HallId);
        return View(session);
    }

    // POST: /Sessions/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SessionDTO session)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(session.MovieId, session.HallId);
            return View(session);
        }

        try
        {
            await _sessionService.UpdateSessionAsync(session);
            TempData["SuccessMessage"] = "Session successfully updated!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateDropdowns(session.MovieId, session.HallId);
            return View(session);
        }
    }

    // GET: /Sessions/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var session = await _sessionService.GetSessionAsync(id);
        if (session == null) return NotFound();
        
        var movie = await _movieService.GetMovieAsync(session.MovieId);
        var hall = await _hallService.GetHallAsync(session.HallId);

        ViewBag.MovieName = movie?.Title ?? "Unknown";
        ViewBag.HallName = hall?.Name ?? "Unknown";
        
        return View(session);
    }

    // POST: /Sessions/DeleteConfirmed
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _sessionService.DeleteSessionAsync(id);
            TempData["SuccessMessage"] = "Session deleted!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    
    private async Task PopulateDropdowns(int? selectedMovieId = null, int? selectedHallId = null)
    {
        var movies = (await _movieService.GetAllMoviesAsync())
                    .Where(m => m.IsActive);
        var halls = await _hallService.GetAllHallsAsync();

        ViewBag.Movies = new SelectList(movies, "Id", "Title", selectedMovieId);
        ViewBag.Halls = new SelectList(halls, "Id", "Name", selectedHallId);
    }
}
