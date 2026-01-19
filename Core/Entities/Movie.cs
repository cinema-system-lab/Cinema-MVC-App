using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    
    /* ВАЖЛИВО: атрибути замінив використанням FluentApi (конфіги прописав в: Infrastructure -> Configurations).
       Будеш робити міграції — обов'язково врахуй конфіги, оскільки вони ставлять обмеження. */
    public string Description { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    
    // Зберігаємо акторів просто строкою: "Brad Pitt, Leonardo DiCaprio"
    // public string Actors { get; set; } = string.Empty;
    public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    public DateTime ReleaseDate { get; set; }
    
    public short DurationMinutes { get; set; } // Оптимізовано: short замість int
    public byte AgeRestriction { get; set; } // Оптимізовано: byte замість int
    
    public decimal Rating { get; set; }
    public GenreType Genres { get; set; }
    
    public string PosterUrl { get; set; } = string.Empty;
    public string TrailerUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}