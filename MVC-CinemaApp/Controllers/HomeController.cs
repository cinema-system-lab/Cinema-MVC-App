using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cinema_MVC_App.Models;
using Core.Interfaces.Services;

namespace Cinema_MVC_App.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMovieService _movieService;

    public HomeController(ILogger<HomeController> logger, IMovieService movieService)
    {
        _logger = logger;
        _movieService = movieService;
    }

    public async Task<IActionResult> Index()
    {
        var allMovies = await _movieService.GetAllMoviesAsync();

        var activeMovies = allMovies.Where(m => m.IsActive).ToList();

        return View(activeMovies);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}