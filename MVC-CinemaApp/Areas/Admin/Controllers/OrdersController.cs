using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Core.Interfaces.Services;

namespace Cinema_MVC_App.Areas.Admin.Controllers;

public class OrdersController : BaseAdminController
{
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;

    public OrdersController(IOrderService orderService, IUserService userService)
    {
        _orderService = orderService;
        _userService = userService;
    }

    // GET: /Orders
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _orderService.GetOrdersByUserAsync(userId);
        
        var userEmails = new Dictionary<string, string>();
        foreach (var order in orders)
        {
            if (!userEmails.ContainsKey(order.UserId))
            {
                var user = await _userService.GetUserByIdAsync(order.UserId);
                userEmails[order.UserId] = user?.Email ?? "Unknown";
            }
        }
        
        ViewBag.UserEmails = userEmails;
        return View(orders);
    }

    // GET: /Orders/Details/{id}
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);

        if (order == null) return NotFound();

        var user = await _userService.GetUserByIdAsync(order.UserId);
        ViewBag.UserEmail = user?.Email ?? "Unknown";

        return View(order);
    }
}