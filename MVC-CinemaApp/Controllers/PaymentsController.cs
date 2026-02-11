using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Core.Enums;
using Core.DTOs;
using System.Security.Claims;

namespace Cinema_MVC_App.Controllers;

[Authorize]
public class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;

    public PaymentsController(IPaymentService paymentService, IOrderService orderService)
    {
        _paymentService = paymentService;
        _orderService = orderService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFromSeats(CreateOrderRequest request)
    {
        if (request.SeatIds == null || !request.SeatIds.Any())
        {
            TempData["ErrorMessage"] = "You must select at least one seat.";
            return RedirectToAction("Book", "Tickets", new { sessionId = request.SessionId });
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderId = await _orderService.CreateOrderAsync(userId, request);
            return RedirectToAction(nameof(Checkout), new { orderId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Book", "Tickets", new { sessionId = request.SessionId });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null || order.Status != OrderStatus.Pending)
        {
            return NotFound();
        }
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null || order.Status != OrderStatus.Pending) return NotFound();

        try
        {
            var paymentId = await _paymentService.CreatePaymentAsync(new CreatePaymentDTO
            {
                OrderId = orderId,
                Amount = order.TotalPrice
            });

            await _paymentService.UpdatePaymentStatusAsync(paymentId, PaymentStatus.Success);
            return RedirectToAction(nameof(Success), new { orderId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Checkout), new { orderId });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Success(Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        return order == null ? NotFound() : View(order);
    }
}