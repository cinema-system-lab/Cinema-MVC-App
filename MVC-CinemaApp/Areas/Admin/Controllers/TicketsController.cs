using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Areas.Admin.Controllers;

public class TicketsController : BaseAdminController
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    // GET: /Tickets
    public async Task<IActionResult> Index()
    {
        var tickets = await _ticketService.GetAllTicketsAsync();
        return View(tickets);
    }
    

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
}
