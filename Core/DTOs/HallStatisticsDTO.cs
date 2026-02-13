namespace Core.DTOs;

public class HallStatisticsDTO
{
    public int TotalSeats { get; set; }
    public int TotalSessions { get; set; }
    public int TotalTicketsSold { get; set; }
    public decimal AverageOccupancyRate { get; set; }
    public decimal TotalRevenue { get; set; }
}
