using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Core.Interfaces.Services;
using Core.DTOs;
using Core.Enums;

namespace Cinema_MVC_App.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET: /Orders
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _orderService.GetOrdersByUserAsync(userId);
        return View(orders);
    }

    // GET: /Orders/Details/{id}
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // POST: /Orders/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        if (request.SeatIds == null || !request.SeatIds.Any())
        {
            TempData["ErrorMessage"] = "You must select at least one seat.";
            return RedirectToAction("Book", "Tickets", new { sessionId = request.SessionId });
        }

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid order data.";
            return RedirectToAction("Book", "Tickets", new { sessionId = request.SessionId });
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _orderService.CreateOrderAsync(userId, request);

            TempData["SuccessMessage"] = "Order created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            // Наприклад: "This seat is already booked and active." або "Cannot create order for started session"
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Book", "Tickets", new { sessionId = request.SessionId });
        }
    }

    // POST: /Orders/Cancel/{id}
    [HttpPost]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            // Використовуємо Canceled (або Cancelled, як у вас в Enum)
            await _orderService.UpdateStatusAsync(id, OrderStatus.Cancelled);
            TempData["SuccessMessage"] = "Order canceled successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message; // Наприклад "Не можна скасувати Paid замовлення"
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Error canceling order.";
        }
        return RedirectToAction(nameof(Index));
    }



}