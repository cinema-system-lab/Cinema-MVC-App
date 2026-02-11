using Cinema_MVC_App.Models;
using Core.Constants;
using Core.DTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Controllers;

[Authorize]
public class TicketsController : Controller
{
    private readonly ITicketService _ticketService;
    private readonly ISessionService _sessionService;
    private readonly IMovieService _movieService;
    private readonly IHallService _hallService;
    private readonly ISeatService _seatService;

    public TicketsController(
        ITicketService ticketService,
        ISessionService sessionService,
        IMovieService movieService,
        IHallService hallService,
        ISeatService seatService)
    {
        _ticketService = ticketService;
        _sessionService = sessionService;
        _movieService = movieService;
        _hallService = hallService;
        _seatService = seatService;
    }

    // GET: /Tickets
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Index()
    {
        var tickets = await _ticketService.GetAllTicketsAsync();
        return View(tickets);
    }
    

    // GET: /Tickets/ByOrder/{orderId}
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ByOrder(Guid orderId)
    {
        var tickets = await _ticketService.GetTicketsByOrderAsync(orderId);
        ViewBag.OrderId = orderId;
        return View("Index", tickets); // Використовуємо той самий Index
    }

    // GET: /Tickets/Details?orderId=...&sessionId=...&seatId=...
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Details(Guid orderId, int sessionId, int seatId)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(orderId, sessionId, seatId);
        if (ticket == null) return NotFound();

        return View(ticket);
    }

    // GET: /Tickets/Delete?orderId=...&sessionId=...&seatId=...
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(Guid orderId, int sessionId, int seatId)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(orderId, sessionId, seatId);
        if (ticket == null) return NotFound();

        return View(ticket);
    }

    // POST: /Tickets/DeleteConfirmed
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteConfirmed(Guid orderId, int sessionId, int seatId)
    {
        try
        {
            await _ticketService.DeleteTicketAsync(orderId, sessionId, seatId);
            TempData["SuccessMessage"] = "Ticket successfully canceled!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An unexpected error occurred while canceling the ticket.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: /Tickets/Book/{sessionId}
    [HttpGet]
    public async Task<IActionResult> Book(int sessionId)
    {
        var session = await _sessionService.GetSessionAsync(sessionId);
        if (session == null)
            return NotFound();

        var movie = await _movieService.GetMovieAsync(session.MovieId);
        var hall = await _hallService.GetHallAsync(session.HallId);
        if (movie == null || hall == null)
            return NotFound();

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
