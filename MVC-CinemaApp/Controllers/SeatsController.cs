using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Controllers;

public class SeatsController : Controller
{
    private readonly ISeatService _seatService;
    private readonly IHallService _hallService; // Щоб отримати назву залу

    public SeatsController(ISeatService seatService, IHallService hallService)
    {
        _seatService = seatService;
        _hallService = hallService;
    }

    // GET: /Seats/Index?hallId=5
    // Це сторінка "Керування місцями залу"
    public async Task<IActionResult> Index(int hallId)
    {
        var hall = await _hallService.GetHallAsync(hallId);
        if (hall == null) return NotFound();

        var seats = await _seatService.GetSeatsByHallIdAsync(hallId);
        
        ViewBag.HallName = hall.Name;
        ViewBag.HallId = hall.Id;

        return View(seats);
    }

    // GET: /Seats/Create?hallId=5
    // Показує форму генерації
    public async Task<IActionResult> Create(int hallId)
    {
        var hall = await _hallService.GetHallAsync(hallId);
        if (hall == null) return NotFound();

        var model = new SeatGenerationDTO { HallId = hallId, Rows = 5, SeatsPerRow = 8 };
        
        ViewBag.HallName = hall.Name;
        return View(model);
    }

    // POST: /Seats/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SeatGenerationDTO generationDto)
    {
        if (!ModelState.IsValid) return View(generationDto);

        try
        {
            await _seatService.GenerateSeatsAsync(generationDto);
            TempData["SuccessMessage"] = "Seats generated successfully!";
            return RedirectToAction(nameof(Index), new { hallId = generationDto.HallId });
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Error generating seats.");
            return View(generationDto);
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(SeatDTO seat)
    {
        // Валідація мінімальна
        if (seat.RowNumber < 1 || seat.SeatNumber < 1)
        {
            TempData["ErrorMessage"] = "Row and Seat numbers must be greater than 0.";
            return RedirectToAction(nameof(Index), new { hallId = seat.HallId });
        }

        try
        {
            await _seatService.AddSeatAsync(seat);
            TempData["SuccessMessage"] = "Seat added successfully!";
        }
        catch (InvalidOperationException ex)
        {
            // Якщо місце вже існує
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Error adding seat.";
        }

        return RedirectToAction(nameof(Index), new { hallId = seat.HallId });
    }
    
    // POST: /Seats/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id, int hallId)
    {
        await _seatService.DeleteSeatAsync(id);
        return RedirectToAction(nameof(Index), new { hallId = hallId });
    }
    
    // POST: /Seats/DeleteAll?hallId=5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAll(int hallId)
    {
        try
        {
            await _seatService.DeleteAllSeatsByHallIdAsync(hallId);
            TempData["SuccessMessage"] = "All seats have been deleted from this hall.";
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Error clearing the hall.";
        }

        return RedirectToAction(nameof(Index), new { hallId = hallId });
    }
    
    [HttpPost]
    public async Task<IActionResult> ToggleType(int id, int hallId)
    {
        await _seatService.ToggleSeatTypeAsync(id);
        return RedirectToAction(nameof(Index), new { hallId = hallId });
    }
}