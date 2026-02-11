using System.Text.Json.Serialization;

namespace Core.DTOs.TMDB;

public class TmdbGenreDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class TmdbMovieDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("release_date")]
    public string ReleaseDate { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = string.Empty;

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    // Додаємо тривалість
    [JsonPropertyName("runtime")]
    public int? Runtime { get; set; }

    // Додаємо жанри
    [JsonPropertyName("genres")]
    public List<TmdbGenreDto> Genres { get; set; } = new List<TmdbGenreDto>();

    public string FullPosterUrl => string.IsNullOrEmpty(PosterPath)
        ? ""
        : $"https://image.tmdb.org/t/p/w500{PosterPath}";
}