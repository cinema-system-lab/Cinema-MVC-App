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
    private const int HoldMinutes = 15;

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

    private const int PaymentTimerMinutes = 5; // Separate shorter timer for payment

    [HttpGet]
    public async Task<IActionResult> Checkout(Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null || order.Status != OrderStatus.Pending)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var seatIds = order.Tickets.Select(t => t.SeatId).ToArray();
        
        // Get the actual seat hold expiration (for reference)
        var seatHoldExpiresAtUtc = SeatHoldStore.GetHoldExpiration(userId!, order.SessionId, seatIds);
        
        // If seat holds expired, user needs to go back and reselect
        if (seatHoldExpiresAtUtc == null || seatHoldExpiresAtUtc <= DateTime.UtcNow)
        {
            TempData["ErrorMessage"] = "Your seat reservation has expired. Please select seats again.";
            return RedirectToAction("Book", "Tickets", new { sessionId = order.SessionId, reset = true });
        }
        
        // Payment timer is separate and always fresh (shorter duration)
        // Use the minimum of payment timer or remaining seat hold time
        var paymentExpiresAt = DateTime.UtcNow.AddMinutes(PaymentTimerMinutes);
        if (seatHoldExpiresAtUtc < paymentExpiresAt)
        {
            paymentExpiresAt = seatHoldExpiresAtUtc.Value;
        }
        
        ViewBag.PaymentExpiresAtUtc = paymentExpiresAt;
        ViewBag.SeatHoldExpiresAtUtc = seatHoldExpiresAtUtc;
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
            
            // Release holds after successful payment - seats are now permanently booked
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var seatIds = order.Tickets.Select(t => t.SeatId).ToArray();
            SeatHoldStore.Release(userId!, order.SessionId, seatIds);
            
            return RedirectToAction(nameof(Success), new { orderId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Checkout), new { orderId });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelExpired(Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null) return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (order.UserId != userId) return Forbid();
        
        if (order.Status == OrderStatus.Pending)
        {
            // Release held seats
            var seatIds = order.Tickets.Select(t => t.SeatId).ToArray();
            SeatHoldStore.Release(userId!, order.SessionId, seatIds);
            
            // Cancel the order
            await _orderService.UpdateStatusAsync(orderId, OrderStatus.Cancelled);
        }
        
        TempData["ErrorMessage"] = "Your reservation time expired. Please select your seats again.";
        return RedirectToAction("Book", "Tickets", new { sessionId = order.SessionId });
    }

    [HttpGet]
    public async Task<IActionResult> Success(Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        return order == null ? NotFound() : View(order);
    }
}