using hatmaker_team2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hatmaker_team2.Controllers
{
    public class UserManageHatOrderController : Controller
    {
        private ModelContext context;
        public UserManageHatOrderController(ModelContext context)
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
    

         public IActionResult LeaveOrderPage()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var userOrders = context.UserManageHatOrders.Include(u => u.Order).Include(u => u.Hat).Where(u => u.UserId == userId).ToList();
            return View("~/Views/User/LeaveOrderView.cshtml", userOrders);  
        }
        [HttpPost]
        public async Task<IActionResult> LeaveOrder(int orderId, List<int> hatIds)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var hatsToRemove = context.UserManageHatOrders.Where(u => u.UserId == userId && u.OrderId == orderId && hatIds.Contains(u.HatId)).ToList();

            if (hatsToRemove.Any())
            {
                context.UserManageHatOrders.RemoveRange(hatsToRemove);
                await context.SaveChangesAsync();

                var userStillManagesHats = context.UserManageHatOrders.Any(u => u.UserId == userId && u.OrderId == orderId);
                var otherUserStillManagesHats = context.UserManageHatOrders.Any(u => u.UserId != userId && u.OrderId == orderId);

                if (!userStillManagesHats && !otherUserStillManagesHats)
                {
                    var order = await context.Orders.FindAsync(orderId);
                    if (order != null)
                    {
                        order.Status = "Ej behandlad";
                        await context.SaveChangesAsync();
                    }
                }
                else if (otherUserStillManagesHats)
                {
                    var orders = await context.Orders.FindAsync(orderId);
                    if (orders != null && orders.Status != "Påbörjad")
                    {
                        orders.Status = "Påbörjad";
                        await context.SaveChangesAsync();
                    }
                }

                    TempData["SuccessMessage"] = "Du har lämnat hatten/ordern.";
            }
            else
            {
                TempData["InfoMessage"] = "Inga hattar hittades.";
            }
            return RedirectToAction("LeaveOrderPage");
        }
    }
}
