using System.Text.Json.Serialization;

namespace Core.DTOs.TMDB;

public class TmdbCreditsDto
{
    [JsonPropertyName("cast")]
    public List<TmdbCastDto> Cast { get; set; } = new();
    [JsonPropertyName("crew")]
    public List<TmdbCrewDto> Crew { get; set; } = new();
}
public class TmdbReleaseDatesResponseDto
{
    [JsonPropertyName("results")]
    public List<TmdbCountryReleaseDto> Results { get; set; } = new();
}

public class TmdbCountryReleaseDto
{
    [JsonPropertyName("iso_3166_1")]
    public string CountryCode { get; set; } = "";

    [JsonPropertyName("release_dates")]
    public List<TmdbReleaseDetailsDto> ReleaseDates { get; set; } = new();
}

public class TmdbReleaseDetailsDto
{
    [JsonPropertyName("certification")]
    public string Certification { get; set; } = "";
}

public class TmdbCastDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}

public class TmdbCrewDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("job")]
    public string Job { get; set; } = "";
}

public class TmdbVideoResponseDto
{
    [JsonPropertyName("results")]
    public List<TmdbVideoDto> Results { get; set; } = new();
}

public class TmdbVideoDto
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = "";
    [JsonPropertyName("site")]
    public string Site { get; set; } = "";
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";
}

public class TmdbGenreDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class TmdbMovieDto
{
    [JsonPropertyName("credits")]
    public TmdbCreditsDto? Credits { get; set; }

    [JsonPropertyName("videos")]
    public TmdbVideoResponseDto? Videos { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("release_dates")]
    public TmdbReleaseDatesResponseDto? ReleaseDates { get; set; }

    [JsonPropertyName("release_date")]
    public string ReleaseDate { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = string.Empty;

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("runtime")]
    public int? Runtime { get; set; }
    [JsonPropertyName("genres")]
    public List<TmdbGenreDto> Genres { get; set; } = new List<TmdbGenreDto>();

    public string FullPosterUrl => string.IsNullOrEmpty(PosterPath)
        ? ""
        : $"https://image.tmdb.org/t/p/w500{PosterPath}";
}