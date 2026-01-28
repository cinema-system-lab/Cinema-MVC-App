namespace Core.DTOs;

public class TicketCreateDTO
{
    public Guid OrderId { get; set; }
    public int SessionId { get; set; }
    public int SeatId { get; set; }
}