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
    private readonly IPaymentService _paymentService;

    public OrdersController(IOrderService orderService, IPaymentService paymentService)
    {
        _orderService = orderService;
        _paymentService = paymentService;
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

    // GET: /Orders/Checkout/{id}
    [HttpGet]
    public async Task<IActionResult> Checkout(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Pending)
        {
            TempData["ErrorMessage"] = "Only pending orders can be paid.";
            return RedirectToAction(nameof(Index));
        }

        return View(order);
    }

    // POST: /Orders/Pay
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
            return RedirectToAction(nameof(Index));
        }

        if (order.TotalPrice <= 0)
        {
            TempData["ErrorMessage"] = "Order total must be greater than zero.";
            return RedirectToAction(nameof(Checkout), new { id = orderId });
        }

        try
        {
            var paymentId = await _paymentService.CreatePaymentAsync(new CreatePaymentDTO
            {
                OrderId = orderId,
                Amount = order.TotalPrice
            });

            await _paymentService.UpdatePaymentStatusAsync(paymentId, PaymentStatus.Success);

            TempData["SuccessMessage"] = "Payment completed successfully.";
            return RedirectToAction(nameof(Details), new { id = orderId });
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Checkout), new { id = orderId });
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Unexpected error while processing payment.";
            return RedirectToAction(nameof(Checkout), new { id = orderId });
        }
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
            var orderId = await _orderService.CreateOrderAsync(userId, request);

            TempData["SuccessMessage"] = "Order created successfully.";
            return RedirectToAction(nameof(Checkout), new { id = orderId });
        }
        catch (InvalidOperationException ex)
        {
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
            await _orderService.UpdateStatusAsync(id, OrderStatus.Cancelled);
            TempData["SuccessMessage"] = "Order canceled successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Error canceling order.";
        }
        return RedirectToAction(nameof(Index));
    }
}