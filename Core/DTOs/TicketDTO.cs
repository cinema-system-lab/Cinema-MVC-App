using Core.Enums;

namespace Core.DTOs;

public class TicketDTO
{
    public Guid OrderId { get; set; }
    public int SessionId { get; set; }
    public int SeatId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public byte RowNumber { get; set; }
    public byte SeatNumber { get; set; }
    public SeatType SeatType { get; set; }
    public decimal Price { get; set; }
}