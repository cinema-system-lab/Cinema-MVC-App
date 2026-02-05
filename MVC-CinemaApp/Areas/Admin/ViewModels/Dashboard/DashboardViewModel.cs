namespace Cinema_MVC_App.Areas.Admin.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int TotalMovies { get; set; }
    public int ActiveMovies { get; set; }
    public int TotalHalls { get; set; }
    public int TotalSeats { get; set; }
    
    public int TotalSessions { get; set; }
    public int TodaySessions { get; set; }
    public int UpcomingSessions { get; set; }
    
    public int TotalTickets { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    
    public decimal TotalRevenue { get; set; }
    public decimal TodayRevenue { get; set; }
    public decimal ThisMonthRevenue { get; set; }
    
    public List<MovieStatsViewModel> TopMovies { get; set; } = new();
    public List<RecentOrderViewModel> RecentOrders { get; set; } = new();
    public List<HallStatsViewModel> HallStats { get; set; } = new();
    public List<DailySalesViewModel> DailySales { get; set; } = new();
}