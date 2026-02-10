namespace Cinema_MVC_App.Areas.Admin.ViewModels.Dashboard;

public class RecentOrderViewModel
{
    public Guid OrderId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TicketsCount { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
}