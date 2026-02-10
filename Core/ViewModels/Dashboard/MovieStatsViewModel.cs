namespace Cinema_MVC_App.Areas.Admin.ViewModels.Dashboard;

public class MovieStatsViewModel
{
    public string MovieTitle { get; set; } = string.Empty;
    public int TicketsSold { get; set; }
    public decimal Revenue { get; set; }
}