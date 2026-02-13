using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    
    public string Actors { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    
    public short DurationMinutes { get; set; } 
    public byte AgeRestriction { get; set; } 
    
    public decimal Rating { get; set; }
    public GenreType Genres { get; set; }
    
    public string PosterUrl { get; set; } = string.Empty;
    public string TrailerUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}