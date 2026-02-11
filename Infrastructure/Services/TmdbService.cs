using Core.DTOs.TMDB;
using Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Infrastructure.Services;

public class TmdbService : ITmdbService
{
    private readonly HttpClient _httpClient;

    public TmdbService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;

        var baseUrl = config["TMDB:BaseUrl"];
        var bearerToken = config["TMDB:BearerToken"];

        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(10);

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", bearerToken);

        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<TmdbMovieDto>> SearchMoviesAsync(string query)
    {
        var encodedQuery = Uri.EscapeDataString(query);
        var url = $"search/movie?query={encodedQuery}&language=en-US";

        var response = await _httpClient.GetFromJsonAsync<TmdbResponseDto>(url);
        return response?.Results ?? new List<TmdbMovieDto>();
    }

    public async Task<List<TmdbMovieDto>> GetPopularMoviesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<TmdbResponseDto>("movie/popular?language=en-US");
        return response?.Results ?? new List<TmdbMovieDto>();
    }

    public async Task<TmdbMovieDto?> GetMovieByIdAsync(int tmdbId)
    {
        var url = $"movie/{tmdbId}?language=en-US&append_to_response=videos,credits,release_dates";
        return await _httpClient.GetFromJsonAsync<TmdbMovieDto>(url);
    }
}