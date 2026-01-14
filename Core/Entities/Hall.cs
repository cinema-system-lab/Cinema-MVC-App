namespace Core.Entities;

public class Hall
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalSeats { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}   