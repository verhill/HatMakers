using hatmaker_team2.Models;
using Microsoft.AspNetCore.Mvc;

namespace hatmaker_team2.Controllers
{
    public class CalenderController : Controller
    {
        private ModelContext context;


        public CalenderController(ModelContext context)
        {
            this.context = context;
        }

        public IActionResult CalenderView()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            return View();

        }
        public JsonResult GetOrderCalender()
        {
            var orders = context.Orders.Where(o => o.DeliveryDate.HasValue).Select(o => new { id = o.OID, title = $"Order #{o.OID}", start = o.DeliveryDate.Value.ToString("yyyy-MM-dd"), status = o.Status }).ToList();
            return Json(orders);
        }
    }
}
