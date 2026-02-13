namespace Core.DTOs;

public class SessionDTO
{
    public int Id { get; set; }
    public int MovieId { get; set; }
        
    public int HallId { get; set; }
        
    public DateTime StartTime { get; set; }
        
    public DateTime EndTime { get; set; }
    public decimal BasePrice { get; set; }
}