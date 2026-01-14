namespace Core.Entities;

public class Actor
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public virtual ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}