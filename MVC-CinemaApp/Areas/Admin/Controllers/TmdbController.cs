using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Areas.Admin.Controllers;

public class TmdbController : BaseAdminController
{
        private readonly ITmdbService _tmdbService;

        public TmdbController(ITmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        // GET: /Admin/Tmdb
        public async Task<IActionResult> Index(string? searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                var popular = await _tmdbService.GetPopularMoviesAsync();
                ViewBag.Title = "Popular Movies";
                return View(popular);
            }
            else
            {
                // Якщо щось ввели - шукаємо
                var searchResults = await _tmdbService.SearchMoviesAsync(searchQuery);
                ViewBag.Title = $"Search results for '{searchQuery}'";
                ViewBag.SearchQuery = searchQuery; 
                return View(searchResults);
            }
        }
}