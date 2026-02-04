using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Core.Enums;

namespace Cinema_MVC_App.Controllers;

[Authorize(Policy = "AdminOnly")]
public class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService) 
        => _paymentService = paymentService;

    // GET: /Payments
    public async Task<IActionResult> Index() 
        => View(await _paymentService.GetAllPaymentsAsync());

    // GET: /Payments/Details/{id}
    public async Task<IActionResult> Details(Guid id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
            return NotFound();

        return View(payment);
    }
    
    // POST: /Payments/UpdateStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
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
}