using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    //[MaxLength(2000)] Опис може бути довгим
    public string Description { get; set; } = string.Empty;


    public string Director { get; set; } = string.Empty;


    //[MaxLength(500)] Зберігаємо акторів просто строкою: "Brad Pitt, Leonardo DiCaprio"
    public string Actors { get; set; } = string.Empty;


    public DateTime ReleaseDate { get; set; }
    public int DurationMinutes { get; set; }
    public int AgeRestriction { get; set; }
    public decimal Rating { get; set; }

    // Ось тут магія бітового енама. В базі це буде int.
    public GenreType Genres { get; set; }

    //[MaxLength(500)]
    public string PosterUrl { get; set; } = string.Empty;
    //[MaxLength(500)]
    public string TrailerUrl { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
}
