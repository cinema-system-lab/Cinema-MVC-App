using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Core.Interfaces.Services;
using Core.DTOs;
using Core.Enums;

namespace Cinema_MVC_App.Controllers;

[Authorize] // Доступ тільки авторизованим
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // 1. GetOrdersByUser
    // GET: /Orders/MyOrders
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _orderService.GetOrdersByUserAsync(userId);
        return View(orders); 
    }

    // 2. CreateOrder
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

    // 3. UpdateStatus
    // POST: /Orders/Cancel/{id}
    [HttpPost]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            await _orderService.UpdateStatusAsync(id, OrderStatus.Cancelled);
        }
        catch (Exception ex)
        {
            return BadRequest("Error updating status: " + ex.Message);
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: /Orders/UpdateStatus
    [HttpPost]
    // [Authorize(Roles = "Admin")] // Розкоментувати, якщо є ролі
    public async Task<IActionResult> UpdateStatus(Guid id, OrderStatus status)
    {
        await _orderService.UpdateStatusAsync(id, status);
        return RedirectToAction(nameof(Index));
    }

    // 4. Delete
    // POST: /Orders/Delete/{id}
    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _orderService.DeleteOrderAsync(id);
        }
        catch (Exception ex)
        {
            return BadRequest("Error deleting order: " + ex.Message);
        }
        return RedirectToAction(nameof(Index));
    }
}