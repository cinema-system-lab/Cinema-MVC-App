using Core.Interfaces.Services;
using Core.Enums;
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

        private static GenreType MapTmdbGenresToLocal(List<Core.DTOs.TMDB.TmdbGenreDto>? tmdbGenres)
        {
            if (tmdbGenres == null || !tmdbGenres.Any())
                return GenreType.None;

            var genreMap = new Dictionary<int, GenreType>
            {
                { 28, GenreType.Action },
                { 18, GenreType.Drama },
                { 35, GenreType.Comedy },
                { 27, GenreType.Horror },
                { 878, GenreType.SciFi },
                { 99, GenreType.Documentary },
                { 53, GenreType.Thriller },
                { 14, GenreType.Fantasy },
                { 16, GenreType.Animation },
                { 12, GenreType.Adventure },
                { 80, GenreType.Crime },
                { 10751, GenreType.Family },
                { 9648, GenreType.Mystery },
                { 10749, GenreType.Romance },
                { 37, GenreType.Western },
                { 10752, GenreType.War },
                { 36, GenreType.History },
                { 10402, GenreType.Music }
            };

            GenreType mappedGenres = GenreType.None;
            foreach (var g in tmdbGenres)
            {
                if (genreMap.TryGetValue(g.Id, out var genreValue))
                {
                    mappedGenres |= genreValue;
                }
            }

            return mappedGenres;
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