using Core.Constants;
using Core.DTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Areas.Admin.Controllers;

public class HallsController : BaseAdminController
{
    private readonly IHallService _hallService;
    private readonly ISeatService _seatService;

    public HallsController(IHallService hallService, ISeatService seatService)
    {
        _hallService = hallService;
        _seatService = seatService;
    }

    // GET: /Halls
    public async Task<IActionResult> Index()
    {
        var halls = await _hallService.GetAllHallsAsync();
        return View(halls);
    }

    // GET: /Halls/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var hall = await _hallService.GetHallAsync(id);
        if (hall == null) return NotFound();
        
        // Get seats count
        ViewBag.SeatsCount = await _hallService.GetSeatsCountAsync(id);
        
        // Get active sessions count
        ViewBag.ActiveSessionsCount = await _hallService.GetActiveSessionsCountAsync(id);
        
        // Get booked seats count
        ViewBag.BookedSeatsCount = await _hallService.GetBookedSeatsCountAsync(id);
        
        // Get upcoming sessions
        ViewBag.UpcomingSessions = await _hallService.GetUpcomingSessionsAsync(id);
        
        // Get statistics (last 30 days)
        ViewBag.Statistics = await _hallService.GetHallStatisticsAsync(id, 30);
        
        return View(hall);
    }

    // GET: /Halls/Create
    public IActionResult Create()
    {
        var hall = _hallService.CreateNewHallDTO();
        return View(hall);
    }

    // POST: /Halls/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HallDTO hall)
    {
        if (!ModelState.IsValid) return View(hall);

        try
        {
            await _hallService.CreateHallAsync(hall);
            TempData["SuccessMessage"] = "Hall successfully created!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while creating the hall.");
            return View(hall);
        }
    }

    // GET: /Halls/Edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var hall = await _hallService.GetHallAsync(id);
        if (hall == null) return NotFound();
        
        // Get seats for the hall
        ViewBag.Seats = await _seatService.GetSeatsByHallIdAsync(id);
        ViewBag.SeatsCount = await _hallService.GetSeatsCountAsync(id);
        
        return View(hall);
    }

    // POST: /Halls/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(HallDTO hall)
    {
        if (!ModelState.IsValid)
        {
            // Reload ViewBag data for the view
            ViewBag.Seats = await _seatService.GetSeatsByHallIdAsync(hall.Id);
            ViewBag.SeatsCount = await _hallService.GetSeatsCountAsync(hall.Id);
            return View(hall);
        }

        try
        {
            await _hallService.UpdateHallAsync(hall);
            TempData["SuccessMessage"] = "Hall successfully updated!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            // Reload ViewBag data for the view
            ViewBag.Seats = await _seatService.GetSeatsByHallIdAsync(hall.Id);
            ViewBag.SeatsCount = await _hallService.GetSeatsCountAsync(hall.Id);
            return View(hall);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while updating the hall.");
            // Reload ViewBag data for the view
            ViewBag.Seats = await _seatService.GetSeatsByHallIdAsync(hall.Id);
            ViewBag.SeatsCount = await _hallService.GetSeatsCountAsync(hall.Id);
            return View(hall);
        }
    }

    // GET: /Halls/Delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var hall = await _hallService.GetHallAsync(id);
        if (hall == null) return NotFound();
        return View(hall);
    }

    // POST: /Halls/DeleteConfirmed
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _hallService.DeleteHallAsync(id);
            TempData["SuccessMessage"] = "Hall deleted!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            var hall = await _hallService.GetHallAsync(id);
            if (hall == null) return NotFound();

            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Delete", hall);
        }
    }
}
