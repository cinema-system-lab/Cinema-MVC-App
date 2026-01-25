using Core.DTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Controllers;

public class TicketsController : Controller
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    // GET: /Tickets

    public async Task<IActionResult> Index()
    {
        // Тимчасово для тесту в'юшки:
        var mockTickets = new List<TicketDTO>
    {
        new TicketDTO {
            MovieTitle = "Avatar 2",
            HallName = "IMAX",
            StartTime = DateTime.Now.AddHours(2),
            RowNumber = 5,
            SeatNumber = 10,
            Price = 150.00m,
            OrderId = Guid.NewGuid(), // Будь-який Guid
            SessionId = 1,
            SeatId = 1
        }
    };
        return View(mockTickets);
    }

    /*
    public async Task<IActionResult> Index()
    {
        var tickets = await _ticketService.GetAllTicketsAsync();
        return View(tickets);
    }
    */

    // GET: /Tickets/ByOrder/{orderId}
    public async Task<IActionResult> ByOrder(Guid orderId)
    {
        var tickets = await _ticketService.GetTicketsByOrderAsync(orderId);
        ViewBag.OrderId = orderId;
        return View("Index", tickets); // Використовуємо той самий Index
    }

    // GET: /Tickets/Details?orderId=...&sessionId=...&seatId=...
    public async Task<IActionResult> Details(Guid orderId, int sessionId, int seatId)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(orderId, sessionId, seatId);
        if (ticket == null) return NotFound();

        return View(ticket);
    }

    // GET: /Tickets/Delete?orderId=...&sessionId=...&seatId=...
    public async Task<IActionResult> Delete(Guid orderId, int sessionId, int seatId)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(orderId, sessionId, seatId);
        if (ticket == null) return NotFound();

        return View(ticket);
    }

    // POST: /Tickets/DeleteConfirmed
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid orderId, int sessionId, int seatId)
    {
        try
        {
            await _ticketService.DeleteTicketAsync(orderId, sessionId, seatId);
            TempData["SuccessMessage"] = "Ticket successfully canceled!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Error occurred while canceling the ticket.";
            return RedirectToAction(nameof(Index));
        }
    }
}
