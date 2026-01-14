namespace Core.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int AgeRestriction { get; set; }
    public decimal Rating { get; set; }
    public string? PosterUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    public virtual ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}