using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
