using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cinema_MVC_App.Models;
using Core.Interfaces.Services;

namespace Cinema_MVC_App.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMovieService _movieService; // Поле для сервиса

    public HomeController(ILogger<HomeController> logger, IMovieService movieService)
    {
        _logger = logger;
        _movieService = movieService;
    }

    public async Task<IActionResult> Index()
    {
        // Получаем все фильмы для отображения на витрине
        var movies = await _movieService.GetAllMoviesAsync();
        return View(movies);
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