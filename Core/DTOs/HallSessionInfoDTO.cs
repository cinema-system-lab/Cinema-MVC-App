namespace Core.DTOs;

public class HallSessionInfoDTO
{
    public int SessionId { get; set; }
    public string MovieName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsOngoing { get; set; }
    public bool IsStartingSoon { get; set; }
}
