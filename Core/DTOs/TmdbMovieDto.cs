using System.Text.Json.Serialization;

namespace Core.DTOs.TMDB;

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

    public string FullPosterUrl => string.IsNullOrEmpty(PosterPath)
        ? "https://via.placeholder.com/500x750"
        : $"https://image.tmdb.org/t/p/w500{PosterPath}";
}