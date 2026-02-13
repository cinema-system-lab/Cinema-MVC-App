using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTOs;

public class MovieDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public string Actors { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public short DurationMinutes { get; set; }
    public byte AgeRestriction { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:F1}", ApplyFormatInEditMode = false)]
    public decimal Rating { get; set; }
    
    public GenreType Genres { get; set; }
    public string PosterUrl { get; set; } = string.Empty;
    public string TrailerUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}