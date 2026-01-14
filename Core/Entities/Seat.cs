namespace Core.Entities;

public class Seat
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public virtual Hall Hall { get; set; } = null!;

    public int RowNumber { get; set; }
    public int SeatNumber { get; set; }

    public virtual ICollection<SessionSeat> SessionSeats { get; set; } = new List<SessionSeat>();
}