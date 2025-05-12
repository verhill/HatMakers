using hatmaker_team2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hatmaker_team2.Controllers
{
    public class MaterialController : Controller
    {
        private ModelContext context;

        public MaterialController(ModelContext context)
        {
            this.context = context;
        }

        public IActionResult AllMaterial()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var materials = context.Materials
                .Include(m => m.MaterialInHat)
                    .ThenInclude(hm => hm.Hat)
                        .ThenInclude(h => h.HatInOrder)
                            .ThenInclude(ho => ho.Order)
                .ToList();

           
            foreach (var material in materials)
            {
                material.MaterialInHat = material.MaterialInHat
                    .Where(hm =>
                        hm.Hat != null &&
                        hm.Hat.HatInOrder.Any(ho => ho.Order.Status == "Ej behandlad")
                    ).ToList();
            }

            
            var filteredMaterials = materials
                .Where(m => m.MaterialInHat.Any())
                .ToList();

            return View(filteredMaterials);
        }

    }
}
