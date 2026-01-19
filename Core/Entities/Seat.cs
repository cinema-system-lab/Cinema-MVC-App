using Core.Enums;

namespace Core.Entities;

public class Seat
{
    public int Id { get; set; }
    public int HallId { get; set; }
    
    public Hall Hall { get; set; } // -virtual
    public byte RowNumber { get; set; } //було інт
    
    public byte SeatNumber { get; set; } // було інт
    public SeatType Type { get; set; }
}