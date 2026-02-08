using Core.Constants;
using Core.DTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Controllers;

[Authorize(Roles = Roles.Admin)]
public class SeatsController : Controller
{
    private readonly ISeatService _seatService;
    private readonly IHallService _hallService;

    public SeatsController(ISeatService seatService, IHallService hallService)
    {
        _seatService = seatService;
        _hallService = hallService;
    }

    // GET: /Seats/Index
    public async Task<IActionResult> Index(int hallId)
    {
        var hall = await _hallService.GetHallAsync(hallId);
        if (hall == null) return NotFound();
        
        bool isLocked = await _seatService.HasAnySessionsAsync(hallId);
    
        if (isLocked)
        {
            TempData["ErrorMessage"] = $"Cannot edit layout for '{hall.Name}' because it has associated sessions. Delete sessions first.";
        
            return RedirectToAction("Edit", "Halls", new { id = hallId });
        }

        var seats = await _seatService.GetSeatsByHallIdAsync(hallId);
    
        ViewBag.HallName = hall.Name;
        ViewBag.HallId = hall.Id;

        return View(seats);
    }

    // GET: /Seats/Create
    public async Task<IActionResult> Create(int hallId)
    {
        var hall = await _hallService.GetHallAsync(hallId);
        if (hall == null) return NotFound();

        var model = new SeatGenerationDTO { HallId = hallId, Rows = 5, SeatsPerRow = 8 };
        ViewBag.HallName = hall.Name;
        return View(model);
    }

    // POST: /Seats/Create (ГЕНЕРАЦІЯ)
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
        catch (InvalidOperationException ex)
        {
             ModelState.AddModelError("", ex.Message);
             var hall = await _hallService.GetHallAsync(generationDto.HallId);
             ViewBag.HallName = hall?.Name;
             return View(generationDto);
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Error generating seats.");
            return View(generationDto);
        }
    }

    // POST: /Seats/Add (ДОДАВАННЯ ОДНОГО)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(SeatDTO seat)
    {
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
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Error adding seat.";
        }

        return RedirectToAction(nameof(Index), new { hallId = seat.HallId });
    }
    
    // POST: /Seats/Delete (ВИДАЛЕННЯ ОДНОГО)
    [HttpPost]
    public async Task<IActionResult> Delete(int id, int hallId)
    {
        try 
        {
            await _seatService.DeleteSeatAsync(id);
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                Request.Headers.Accept.ToString().Contains("application/json"))
            {
                return Ok(new { success = true, message = "Seat deleted." });
            }
            
            TempData["SuccessMessage"] = "Seat deleted.";
        }
        catch (InvalidOperationException ex)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                Request.Headers.Accept.ToString().Contains("application/json"))
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                Request.Headers.Accept.ToString().Contains("application/json"))
            {
                return StatusCode(500, new { success = false, message = "Error deleting seat." });
            }
            
            TempData["ErrorMessage"] = "Error deleting seat.";
        }
        
        return RedirectToAction(nameof(Index), new { hallId = hallId });
    }
    
    // POST: /Seats/DeleteAll (ОЧИЩЕННЯ ЗАЛУ)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAll(int hallId)
    {
        try
        {
            await _seatService.DeleteAllSeatsByHallIdAsync(hallId);
            TempData["SuccessMessage"] = "All seats have been deleted from this hall.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Error clearing the hall.";
        }

        return RedirectToAction(nameof(Index), new { hallId = hallId });
    }
    
    // POST: /Seats/ToggleType (ЗМІНА ТИПУ)
    [HttpPost]
    public async Task<IActionResult> ToggleType(int id, int hallId)
    {
        try 
        {
            await _seatService.ToggleSeatTypeAsync(id);
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                Request.Headers.Accept.ToString().Contains("application/json"))
            {
                return Ok(new { success = true, message = "Seat type updated." });
            }
            
            TempData["SuccessMessage"] = "Seat type updated.";
        }
        catch (InvalidOperationException ex)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                Request.Headers.Accept.ToString().Contains("application/json"))
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            
            TempData["ErrorMessage"] = ex.Message;
        }
        catch 
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                Request.Headers.Accept.ToString().Contains("application/json"))
            {
                return StatusCode(500, new { success = false, message = "Error updating seat type." });
            }
            
            TempData["ErrorMessage"] = "Error updating seat type.";
        }
        
        return RedirectToAction(nameof(Index), new { hallId = hallId });
    }
}