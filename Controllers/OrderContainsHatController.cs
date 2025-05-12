using hatmaker_team2.Models;
using Microsoft.AspNetCore.Mvc;

namespace hatmaker_team2.Controllers
{
    public class OrderContainsHatController : Controller
    {
        private ModelContext context;
        public OrderContainsHatController(ModelContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }
    }
}
