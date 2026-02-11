using Core.Constants;
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

    public async Task<IActionResult> Index()
    {
        return RedirectToAction("Index", "Movies");
    }

    public async Task<IActionResult> Details(int id)
    {
        var hall = await _hallService.GetHallAsync(id);
        if (hall == null) return NotFound();

        return View(hall);
    }
}