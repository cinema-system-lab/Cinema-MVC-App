using System.Text.Json.Serialization;

namespace Core.DTOs.TMDB;

public class TmdbResponseDto
{
    [JsonPropertyName("results")]
    public List<TmdbMovieDto> Results { get; set; } = new List<TmdbMovieDto>();
}