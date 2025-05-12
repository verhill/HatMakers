using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using hatmaker_team2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace hatmaker_team2.Controllers
{
    public class HatController : Controller
    {
        private ModelContext context;
        public HatController(ModelContext context)
        {
            this.context = context;
        }

        public IActionResult AllHats()
        {
            IQueryable<Hat> hatLists = from hat in context.Hats where hat.IsSpecial == false && hat.IsDeactivated == false select hat;
            return View(hatLists.ToList());
        }

        [HttpGet]
        public IActionResult AddStandardHat()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var materials = context.Materials
                .Select(m => new SelectListItem
                {
                    Value = m.MID.ToString(),
                    Text = m.Name
                }).ToList();

            var viewModel = new HatViewModel
            {
                Hat = new Hat(),
                Materials = materials
            };

            return View(viewModel);
        }


        [HttpPost]

        public async Task<IActionResult> AddHat(HatViewModel hatViewModel, IFormFile bildFil)

        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }

            if (!ModelState.IsValid)
            {
                
                hatViewModel.Materials = context.Materials.Select(m => new SelectListItem
                {
                    Value = m.MID.ToString(),
                    Text = m.Name
                }).ToList();

                return View("AddStandardHat", hatViewModel);
            }

            var hatt = hatViewModel.Hat;

            
            if (context.Hats.Any(h => h.Name.ToLower() == hatt.Name.ToLower()))
            {
                ModelState.AddModelError("Hat.Name", "En hatt med det namnet finns redan.");

                hatViewModel.Materials = context.Materials.Select(m => new SelectListItem
                {
                    Value = m.MID.ToString(),
                    Text = m.Name
                }).ToList();

                return View("AddStandardHat", hatViewModel);
            }

            if (bildFil != null && bildFil.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(bildFil.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("Picture", "Endast bildfiler (JPG, PNG, GIF) är tillåtna.");
                    return RedirectToAction("AllHats");
                }


                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await bildFil.CopyToAsync(stream);
                }

                hatt.Picture = $"/images/{uniqueFileName}";
                
            }

                context.Hats.Add(hatt);
            context.SaveChanges();




            foreach (var materialId in hatViewModel.SelectedMaterialIds)
            {
                var hatMaterial = new Hat_Made_Of_Material
                {
                    HatId = hatt.HID,
                    MaterialId = materialId
                };
                context.HatmadeOfMaterials.Add(hatMaterial);
            }

            context.SaveChanges();

            return RedirectToAction("AllHats");

        }



        [HttpGet]
        public IActionResult HatInfo(int HatID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            IQueryable<Hat>hatList = from hat in context.Hats where hat.HID == HatID select hat;
            return View(hatList.FirstOrDefault());
        }

        [HttpPost]
        public IActionResult EditHat(Hat hat)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            Hat updatedHat = (from h in context.Hats where h.HID == hat.HID select h).FirstOrDefault();

            if (ModelState.IsValid)
            {
                updatedHat.Name = hat.Name;
                updatedHat.Price = hat.Price;
                //updatedHat.Size = hat.Size;

                context.Update(updatedHat);
                context.SaveChanges();
                return RedirectToAction("AllHats");
            }
            else
            {
                return View("HatInfo", hat);
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> UpdateHatPicture(IFormFile imgFile, int HID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            if (imgFile != null && imgFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(imgFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("Picture", "Endast bildfiler (JPG, PNG, GIF) är tillåtna.");
                    return RedirectToAction("AllHats");
                }

                
                var updatedHat = context.Hats.FirstOrDefault(h => h.HID == HID);
                if (updatedHat == null)
                {
                    return NotFound("Hatten hittades inte.");
                }

                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imgFile.CopyToAsync(stream);
                }

                updatedHat.Picture = $"/images/{uniqueFileName}";
                context.Update(updatedHat);
                context.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("Picture", "Ingen bild valdes.");
            }

            return RedirectToAction("AllHats");
        }

        [HttpPost]
        public IActionResult DeleteHat(int HID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            IQueryable<Hat> hatList = from hat in context.Hats where hat.HID == HID select hat;
            Hat toUpdate = hatList.FirstOrDefault();

            toUpdate.IsDeactivated = true;
            context.SaveChanges();
            return RedirectToAction("AllHats");

        }
    }

    
}
