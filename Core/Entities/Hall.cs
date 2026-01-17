using Core.Enums;

namespace Core.Entities;

public class Hall
{
    public int Id { get; set; }

    //[MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    
    public HallType Type { get; set; }
    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
