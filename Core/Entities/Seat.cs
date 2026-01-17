using Core.Enums;

namespace Core.Entities;

public class Seat
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public virtual Hall Hall { get; set; }
    public int RowNumber { get; set; }
    public int SeatNumber { get; set; }
    public SeatType Type { get; set; }
}