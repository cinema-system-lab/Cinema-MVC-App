using Cinema_MVC_App.Models;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Controllers;

[Authorize]
public class TicketsController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IMovieService _movieService;
    private readonly IHallService _hallService;
    private readonly ISeatService _seatService;
    private readonly ITicketService _ticketService;

    public TicketsController(
        ISessionService sessionService,
        IMovieService movieService,
        IHallService hallService,
        ISeatService seatService,
        ITicketService ticketService)
    {
        _sessionService = sessionService;
        _movieService = movieService;
        _hallService = hallService;
        _seatService = seatService;
        _ticketService = ticketService;
    }

   
    [HttpGet]
    public async Task<IActionResult> Book(int sessionId)
    {
        var session = await _sessionService.GetSessionAsync(sessionId);
        if (session == null) return NotFound();

        var movie = await _movieService.GetMovieAsync(session.MovieId);
        var hall = await _hallService.GetHallAsync(session.HallId);

        if (movie == null || hall == null) return NotFound();

        var seats = await _seatService.GetSeatsByHallIdAsync(hall.Id);
        var occupiedSeatIds = await _ticketService.GetOccupiedSeatIdsAsync(sessionId);

        var viewModel = new SeatSelectionVM
        {
            SessionId = session.Id,
            MovieTitle = movie.Title,
            PosterUrl = movie.PosterUrl ?? string.Empty,
            StartTime = session.StartTime,
            BasePrice = session.BasePrice,
            HallId = hall.Id,
            HallName = hall.Name,
            HallType = hall.Type,
            Seats = seats,
            OccupiedSeatIds = occupiedSeatIds.ToHashSet()
        };

        return View(viewModel);
    }
}