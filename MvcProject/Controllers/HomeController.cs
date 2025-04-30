using Microsoft.AspNetCore.Mvc;

namespace MvcProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}