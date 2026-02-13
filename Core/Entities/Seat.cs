using Core.Enums;

namespace Core.Entities;

public class Seat
{
    public int Id { get; set; }
    public int HallId { get; set; }
    
    public Hall Hall { get; set; } 
    public byte RowNumber { get; set; } 
    
    public byte SeatNumber { get; set; } 
    public SeatType Type { get; set; }
}