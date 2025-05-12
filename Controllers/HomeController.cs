using System.Diagnostics;
using hatmaker_team2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace hatmaker_team2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ModelContext context;
        private SignInManager<User> signInManager;
        public HomeController(ILogger<HomeController> logger, ModelContext context, SignInManager<User> signInMang)
        {
            _logger = logger;
            this.context = context;
            this.signInManager = signInMang;
        }

        public IActionResult Index()
        {
            IQueryable<User> loginUser = from u in context.Users select u;
            loginUser = loginUser.Where(u => u.UserName == User.Identity.Name);
            User loggedInUser = loginUser.ToList().FirstOrDefault();

            
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            else if(loggedInUser.IsDeactivated == true)
            {
                signInManager.SignOutAsync();
                return RedirectToAction("Login", "User");
            }
            else
            {
                return View();
            }
                
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}
