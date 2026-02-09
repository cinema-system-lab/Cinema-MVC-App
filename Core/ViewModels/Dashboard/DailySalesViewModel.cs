namespace Cinema_MVC_App.Areas.Admin.ViewModels.Dashboard;

public class DailySalesViewModel
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int TicketsSold { get; set; }
}