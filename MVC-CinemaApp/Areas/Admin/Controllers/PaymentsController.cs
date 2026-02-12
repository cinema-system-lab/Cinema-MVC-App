using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Services;
using Core.Enums;

namespace Cinema_MVC_App.Areas.Admin.Controllers;

public class PaymentsController : BaseAdminController
{
    private readonly IPaymentService _paymentService;
    private readonly IUserService _userService;
    
    public PaymentsController(IPaymentService paymentService, IUserService userService)
    {
        _paymentService = paymentService;
        _userService = userService;
    }

    // GET: /Payments
    public async Task<IActionResult> Index()
    {
        var payments = await _paymentService.GetAllPaymentsAsync();
        var allUsers = await _userService.GetAllUsersAsync();
        ViewBag.UserEmails = allUsers.ToDictionary(u => u.Id, u => u.Email);

        return View(payments);
    }

    // GET: /Payments/Details/{id}
    public async Task<IActionResult> Details(Guid id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
            return NotFound();

        return View(payment);
    }

    // POST: /Payments/UpdateStatus todo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid id, PaymentStatus newStatus)
    {
        try
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                TempData["Error"] = "Payment not found.";
                return RedirectToAction(nameof(Index));
            }

            var isValidTransition = (payment.Status, newStatus) switch
            {
                (PaymentStatus.Pending, PaymentStatus.Success) => true,
                (PaymentStatus.Pending, PaymentStatus.Failed) => true,
                (PaymentStatus.Success, PaymentStatus.Refunded) => true,
                _ => false
            };

            if (!isValidTransition)
            {
                TempData["Error"] = $"Cannot change status from {payment.Status} to {newStatus}. Invalid transition.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _paymentService.UpdatePaymentStatusAsync(id, newStatus);

            TempData["Success"] = newStatus switch
            {
                PaymentStatus.Success => "Payment marked as successful.",
                PaymentStatus.Failed => "Payment marked as failed.",
                PaymentStatus.Refunded => "Payment refunded successfully.",
                _ => "Payment status updated successfully."
            };
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error updating payment: {ex.Message}";
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}