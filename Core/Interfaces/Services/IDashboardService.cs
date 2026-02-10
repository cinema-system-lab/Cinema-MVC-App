using Cinema_MVC_App.Areas.Admin.ViewModels.Dashboard;

namespace Core.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardDataAsync();
}