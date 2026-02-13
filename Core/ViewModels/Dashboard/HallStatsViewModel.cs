namespace Cinema_MVC_App.Areas.Admin.ViewModels.Dashboard;

public class HallStatsViewModel
{
    public string HallName { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public int SessionsCount { get; set; }
    public double OccupancyRate { get; set; }
}