using Core.Enums;

namespace Core.Entities;

public class Hall
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public HallType Type { get; set; }
    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
