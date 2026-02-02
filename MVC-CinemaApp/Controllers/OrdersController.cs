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

    public OrdersController(IOrderService orderService,  IPaymentService paymentService)
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

        var payment = await _paymentService.GetPaymentByOrderIdAsync(id);
        ViewBag.Payment = payment;
        
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
            var orderId = await _orderService.CreateOrderAsync(userId, request);
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new InvalidOperationException("Failed to retrieve created order");
            
            var createPaymentDto = new CreatePaymentDTO
            {
                OrderId = orderId,
                Amount = order.TotalPrice,
                Status = PaymentStatus.Pending
            };
            
            await _paymentService.CreatePaymentAsync(createPaymentDto);
            
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