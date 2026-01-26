using Core.DTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Controllers;

public class HallsController : Controller
{
    private readonly IHallService _hallService;

    public HallsController(IHallService hallService)
    {
        _hallService = hallService;
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
        
        return View(hall);
    }

    // GET: /Halls/Create
    public IActionResult Create()
    {
        var hall = new HallDTO { Type = HallType.Standard };
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
        return View(hall);
    }

    // POST: /Halls/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(HallDTO hall)
    {
        if (!ModelState.IsValid) return View(hall);

        try
        {
            await _hallService.UpdateHallAsync(hall);
            TempData["SuccessMessage"] = "Hall successfully updated!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while updating the hall.");
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
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
