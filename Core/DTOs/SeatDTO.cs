using Core.Enums;

namespace Core.DTOs;

public class SeatDTO
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public byte RowNumber { get; set; }
    public byte SeatNumber { get; set; }
    public SeatType Type { get; set; }
}