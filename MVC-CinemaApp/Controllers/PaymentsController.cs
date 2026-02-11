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

    // GET: /Payments (Admin only)
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Index() 
        => View(await _paymentService.GetAllPaymentsAsync());

    // GET: /Payments/Details/{id}
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Details(Guid id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
            return NotFound();

        return View(payment);
    }
    
    // POST: /Payments/UpdateStatus (Admin only)
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateStatus(Guid id, PaymentStatus newStatus)
    {
        try
        {
            await _paymentService.UpdatePaymentStatusAsync(id, newStatus); 
            TempData["Success"] = "Payment status updated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error updating payment: {ex.Message}";
        }
    
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /Payments/CreateFromSeats - Creates order and redirects to checkout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFromSeats(CreateOrderRequest request)
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
            var orderId = await _orderService.CreateOrderAsync(userId, request);

            return RedirectToAction(nameof(Checkout), new { orderId });
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Book", "Tickets", new { sessionId = request.SessionId });
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An unexpected error occurred.";
            return RedirectToAction("Book", "Tickets", new { sessionId = request.SessionId });
        }
    }

    // GET: /Payments/Checkout/{orderId}
    [HttpGet]
    public async Task<IActionResult> Checkout(Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Pending)
        {
            TempData["ErrorMessage"] = "Only pending orders can be paid.";
            return RedirectToAction("Index", "Orders");
        }

        return View(order);
    }

    // POST: /Payments/Pay
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Pending)
        {
            TempData["ErrorMessage"] = "Only pending orders can be paid.";
            return RedirectToAction("Index", "Orders");
        }

        if (order.TotalPrice <= 0)
        {
            TempData["ErrorMessage"] = "Order total must be greater than zero.";
            return RedirectToAction(nameof(Checkout), new { orderId });
        }

        try
        {
            var paymentId = await _paymentService.CreatePaymentAsync(new CreatePaymentDTO
            {
                OrderId = orderId,
                Amount = order.TotalPrice
            });

            await _paymentService.UpdatePaymentStatusAsync(paymentId, PaymentStatus.Success);

            TempData["SuccessMessage"] = "Payment completed successfully! Your tickets are confirmed.";
            return RedirectToAction(nameof(Success), new { orderId });
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Checkout), new { orderId });
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Unexpected error while processing payment.";
            return RedirectToAction(nameof(Checkout), new { orderId });
        }
    }

    // GET: /Payments/Success/{orderId}
    [HttpGet]
    public async Task<IActionResult> Success(Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}