using Microsoft.AspNetCore.Mvc;

namespace Cinema_MVC_App.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
