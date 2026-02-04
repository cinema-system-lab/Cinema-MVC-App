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

        // можна додати перевірку, чи належить замовлення поточному юзеру
        // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // if (order.UserId != userId) return Forbid();

        return View(order);
    }

    // POST: /Orders/Create
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _orderService.CreateOrderAsync(userId, request);

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
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