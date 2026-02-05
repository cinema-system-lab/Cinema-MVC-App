using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Areas.Admin.Controllers
{
    public class DashboardController : BaseAdminController // Наслідуємось від Base!
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
