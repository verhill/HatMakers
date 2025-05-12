using Castle.Core.Logging;
using Castle.Core.Resource;
using hatmaker_team2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using SkiaSharp;
using ZXing.SkiaSharp;
using static System.Net.Mime.MediaTypeNames;
using ZXing;
using ZXing.SkiaSharp.Rendering;
using PdfSharpCore.Drawing.Layout;


namespace hatmaker_team2.Controllers
{
    public class OrderController : Controller
    {
        private ModelContext context;
        

        public OrderController(ModelContext context)
        {
            this.context = context;

        }

        [HttpGet]
        public IActionResult AllOrders(string hatFilter, string customerFilter, DateOnly? startDate, DateOnly? endDate)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var orders = context.Orders
                .Include(o => o.HatsInOrder)
                    .ThenInclude(h => h.Hat)
                .Include(o => o.User)
                .Include(o => o.Customer)
                .AsQueryable();

            

            if (!string.IsNullOrEmpty(hatFilter))
            {
                orders = orders.Where(o => o.HatsInOrder.Any(h => h.Hat.Name == hatFilter));
            }

            if (!string.IsNullOrEmpty(customerFilter))
            {
                orders = orders.Where(o => o.Customer.Email == customerFilter);
            }

            if (startDate.HasValue)
                orders = orders.Where(o => o.DeliveryDate >= startDate);

            if (endDate.HasValue)
                orders = orders.Where(o => o.DeliveryDate <= endDate);

            return View(orders.ToList());
        }



        [HttpGet]
        public IActionResult OrderConfirmation(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            Console.WriteLine("Inkommande id: " + id);
            var order = context.Orders
                .Include(o => o.Customer)
                .Include(o => o.HatsInOrder)
                    .ThenInclude(ho => ho.Hat)
                .FirstOrDefault(o => o.OID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View("OrderConfirmation", order);
        }


        [HttpGet]
        public IActionResult NewOrder()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            NewOrderViewModel model = new NewOrderViewModel
            {
                AllCustomers = context.Customers.ToList(),
                SelectedCustomer = new Customer(),
                CurrentOrder = new Order(),
                StandardHats = new List<Hat>()
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult Sales(DateOnly? startDate = null, DateOnly? endDate = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var query = context.Orders
                .Where(o => o.Status == "Avslutad")
                .Include(o => o.Customer)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(o => o.DeliveryDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.DeliveryDate <= endDate.Value);
            }

            var completedOrders = query.ToList();


            double totalCompletedSales = completedOrders.Sum(o => o.TotPrice);
            int completedOrderCount = completedOrders.Count;
            double averageOrderValue = completedOrderCount > 0 ? totalCompletedSales / completedOrderCount : 0;

            // Gruppera ordrar per månad
            var monthlySales = completedOrders
                .Where(o => o.DeliveryDate.HasValue)
                .GroupBy(o => new {
                    Year = o.DeliveryDate.Value.Year,
                    Month = o.DeliveryDate.Value.Month
                })
                .Select(g => new MonthlySalesViewModel
                {
                    YearMonth = $"{g.Key.Year}-{g.Key.Month:D2}",
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    OrderCount = g.Count(),
                    TotalSales = g.Sum(o => o.TotPrice)
                })
                .OrderByDescending(x => x.YearMonth)
                .ToList();
            // Gruppera ordrar per kvartal
            var quarterlySales = completedOrders
                .Where(o => o.DeliveryDate.HasValue)
                .GroupBy(o => new {
                    Year = o.DeliveryDate.Value.Year,
                    Quarter = (o.DeliveryDate.Value.Month - 1) / 3 + 1 // Calculate quarter (1-4)
                })
                .Select(g => new QuarterlySalesViewModel
                {
                    YearQuarter = $"{g.Key.Year}-Q{g.Key.Quarter}",
                    QuarterName = $"Q{g.Key.Quarter} {g.Key.Year}",
                    OrderCount = g.Count(),
                    TotalSales = g.Sum(o => o.TotPrice)
                })
                .OrderByDescending(x => x.YearQuarter)
                .ToList();
            // Gruppera ordrar per år
            var yearlySales = completedOrders
                .Where(o => o.DeliveryDate.HasValue)
                .GroupBy(o => o.DeliveryDate.Value.Year)
                .Select(g => new YearlySalesViewModel
                {
                    Year = g.Key.ToString(),
                    OrderCount = g.Count(),
                    TotalSales = g.Sum(o => o.TotPrice)
                })
                .OrderByDescending(x => x.Year)
                .ToList();



            var recentOrders = completedOrders
                .OrderByDescending(o => o.DeliveryDate)
                .Take(5)
                .ToList();

            var viewModel = new SalesDashboardViewModel
            {
                TotalCompletedSales = totalCompletedSales,
                CompletedOrderCount = completedOrderCount,
                AverageOrderValue = averageOrderValue,
                MonthlySales = monthlySales,
                QuarterlySales = quarterlySales,
                YearlySales = yearlySales,
                RecentCompletedOrders = recentOrders,
                StartDate = startDate,
                EndDate = endDate
            };

            return View(viewModel);
        }

        public IActionResult OrderHistory()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            IQueryable<Order> orderList = from Orders in context.Orders select Orders;
            //ViewData["Title"] = "Orderinfo";
            return View(orderList.ToList());
        }

        [HttpGet]
        public IActionResult SelectCustomerInNewOrder(int customerId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var model = new NewOrderViewModel
            {
                AllCustomers = context.Customers.ToList(),
                SelectedCustomer = context.Customers.FirstOrDefault(c => c.CID == customerId) ?? new Customer(),
                CurrentOrder = new Order(),
                StandardHats = new List<Hat>()
            };
            return View("NewOrder", model);
        }

        [HttpPost]
        public IActionResult AddCustomerToOrder(NewOrderViewModel newOrderViewModel)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            Customer selectedCustomer;

            IQueryable<Customer> allCustomers = from customer in context.Customers select customer;
            List<Customer> customers = allCustomers.ToList();

            foreach (Customer c in customers)
            {
                if (string.Equals(c.Email, newOrderViewModel.SelectedCustomer.Email, StringComparison.OrdinalIgnoreCase)
                    && c.CID != newOrderViewModel.SelectedCustomer.CID)

                {
                    var errorModel = new NewOrderViewModel
                    {
                        AllCustomers = context.Customers.ToList(),
                        SelectedCustomer = newOrderViewModel.SelectedCustomer,
                        CurrentOrder = new Order(),
                        StandardHats = context.Hats.ToList(),
                    };
                    ModelState.AddModelError("SelectedCustomer.Email","Det finns redan en kund med denna epost.");
                    return View("NewOrder", errorModel);
                }
            }

            if (!Regex.IsMatch(newOrderViewModel.SelectedCustomer.Phone, @"^[\d\s\+\-]{1,15}$"))
            {
                var errorModel = new NewOrderViewModel
                {
                    AllCustomers = context.Customers.ToList(),
                    SelectedCustomer = newOrderViewModel.SelectedCustomer,
                    CurrentOrder = new Order(),
                    StandardHats = context.Hats.ToList(),
                };
                ModelState.AddModelError("SelectedCustomer.Phone", "Vänligen ange ett giltigt telefonnummer.");
                return View("NewOrder", errorModel);
            }

            

            if (newOrderViewModel.SelectedCustomer.CID == 0)
            {
                // Ny kund
                selectedCustomer = newOrderViewModel.SelectedCustomer;
                context.Customers.Add(selectedCustomer);
                context.SaveChanges(); // Här får kunden ett CID!
            }
            else
            {
                // Befintlig kund
                selectedCustomer = context.Customers.FirstOrDefault(c => c.CID == newOrderViewModel.SelectedCustomer.CID);
                if (selectedCustomer != null)
                {
                    selectedCustomer.Firstname = newOrderViewModel.SelectedCustomer.Firstname;
                    selectedCustomer.Lastname = newOrderViewModel.SelectedCustomer.Lastname;
                    selectedCustomer.Email = newOrderViewModel.SelectedCustomer.Email;
                    selectedCustomer.Phone = newOrderViewModel.SelectedCustomer.Phone;
                    selectedCustomer.Address = newOrderViewModel.SelectedCustomer.Address;
                    selectedCustomer.City = newOrderViewModel.SelectedCustomer.City;
                    selectedCustomer.Country = newOrderViewModel.SelectedCustomer.Country;
                }

                context.SaveChanges();
            }

            // Skapa modellen igen, sätt korrekt CustomerId på ordern
            var model = new NewOrderViewModel
            {
                AllCustomers = context.Customers.ToList(),
                SelectedCustomer = selectedCustomer,
                CurrentOrder = new Order(),
                StandardHats = context.Hats.ToList(),
                MaterialsInDb = context.Materials.ToList(),
            };

            IQueryable<Customer> currentCustomer = from customer in context.Customers select customer;
            currentCustomer = currentCustomer.Where(c => c.Email == newOrderViewModel.SelectedCustomer.Email);


            model.CurrentOrder.Customer = currentCustomer.FirstOrDefault();
            model.CustomerConfirmed = true;


            return View("NewOrder", model);
        }

        [HttpPost]
        public IActionResult AddHatToOrder(int hatId,
            int customerId,
            bool customerConfirmed,
            List<int> addedHatIds,
            double headsize,
            double height,
            List<int> orderHat_HIDs,
            List<double> orderHat_HeadSizes,
            List<double> orderHat_Heights,
            string specialDescription,
            float specialPrice,
            string specialName,
            string materialArr,
            IFormFile specialPicture)

        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            Debug.WriteLine("Materialer: " + materialArr);
            var newOrderVM = new NewOrderViewModel();

            IQueryable<Customer> allCustomers = from customer in context.Customers select customer;
            Customer theCustomer = allCustomers.FirstOrDefault(c => c.CID == customerId);

            newOrderVM.StandardHats = context.Hats.ToList();
            newOrderVM.CustomerConfirmed = customerConfirmed;
            newOrderVM.CurrentOrder = new Order
            {
                Customer = theCustomer
            };
            newOrderVM.MaterialsInDb = context.Materials.ToList();
            newOrderVM.HatsInOrder = new List<Hat>();
            newOrderVM.OrderContainsHats = new List<Order_Contains_Hat>();

            // Återskapa befintliga hattar i ordern
            if (orderHat_HIDs != null && orderHat_HeadSizes != null && orderHat_Heights != null)
            {
                for (int i = 0; i < orderHat_HIDs.Count; i++)
                {
                    var hat = context.Hats.FirstOrDefault(h => h.HID == orderHat_HIDs[i]);
                    if (hat != null)
                    {
                        newOrderVM.HatsInOrder.Add(hat);
                        newOrderVM.OrderContainsHats.Add(new Order_Contains_Hat
                        {
                            Hat = hat,
                            HeadSize = orderHat_HeadSizes[i],
                            Height = orderHat_Heights[i],
                        });
                        Debug.WriteLine("Storlek: " + orderHat_HeadSizes[i] + " " + orderHat_Heights[i]);
                    }
                }
            }
            ModelState.Remove("specialName");
            ModelState.Remove("specialDescription");
            ModelState.Remove("specialPrice");
            ModelState.Remove("materialArr");
            ModelState.Remove("specialPicture");
            // Lägg till ny hatt
            if (hatId != 0)
            {
                var newHat = context.Hats.FirstOrDefault(h => h.HID == hatId);
                if (newHat != null)
                {
                    newOrderVM.HatsInOrder.Add(newHat);
                    newOrderVM.OrderContainsHats.Add(new Order_Contains_Hat
                    {
                        Hat = newHat,
                        HeadSize = headsize,
                        Height = height
                    });
                }
                
            }
            else
            {
                if (string.IsNullOrEmpty(specialName) || string.IsNullOrEmpty(specialDescription) || specialPrice <= 0 || string.IsNullOrEmpty(materialArr))
                {
                    foreach (var hat in newOrderVM.HatsInOrder)
                    {
                        newOrderVM.CurrentOrder.TotPrice += hat.Price;
                    }

                    ModelState.AddModelError("specialName", "Vänligen fyll i namn på specialhattar.");
                    ModelState.AddModelError("specialDescription", "Vänligen fyll i beskrivning till specialhattar.");
                    ModelState.AddModelError("specialPrice", "Vänligen fyll i ett pris som är högre än 0.");
                    ModelState.AddModelError("materialArr", "Vänligen fyll i material till specialhattar.");


                    return View("NewOrder", newOrderVM);
                }

               //Skapa en special hatt
                    Hat specialHat = new Hat
                    {
                        Name = specialName,
                        IsSpecial = true,
                        Price = specialPrice,
                        SpecialDescription = specialDescription
                    };

                Debug.WriteLine($"specialPicture är null: {specialPicture == null}");
                Debug.WriteLine($"specialPicture.Length: {(specialPicture != null ? specialPicture.Length.ToString() : "null")}");
                Debug.WriteLine($"specialPicture.FileName: {(specialPicture != null ? specialPicture.FileName : "null")}");

                if (specialPicture != null && specialPicture.Length > 0)
                {
                    var tillåtnaFiltyper = new[] { ".jpg", ".jpeg", ".png" };
                    var tillåtnaMimeTyper = new[] { "image/jpeg", "image/png" };

                    var filÄndelse = Path.GetExtension(specialPicture.FileName).ToLowerInvariant();
                    var mimeTyp = specialPicture.ContentType.ToLowerInvariant();

                    if (!tillåtnaFiltyper.Contains(filÄndelse) || !tillåtnaMimeTyper.Contains(mimeTyp))
                    {
                        ModelState.AddModelError("specialPicture", "Endast .jpg, .jpeg eller .png-filer är tillåtna.");
                        return View("NewOrder", newOrderVM); // eller RedirectToAction beroende på ditt flöde
                    }

                    Debug.WriteLine("Bild if går igenom");
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniktFilnamn = Guid.NewGuid().ToString() + filÄndelse;
                    var filPath = Path.Combine(uploadsFolder, uniktFilnamn);

                    using (var fileStream = new FileStream(filPath, FileMode.Create))
                    {
                        specialPicture.CopyTo(fileStream);
                    }

                    specialHat.Picture = "/images/" + uniktFilnamn;
                }



                context.Hats.Add(specialHat);
                context.SaveChanges();

                newOrderVM.HatsInOrder.Add(specialHat);
                newOrderVM.OrderContainsHats.Add(new Order_Contains_Hat
                {
                    Hat = specialHat,
                    HeadSize = headsize,
                    Height = height
                });


                string cleanedString = materialArr.Replace("\r", "").Replace("\n", "").Trim();
                string[] splittedString = cleanedString.Split(',', StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < splittedString.Length - 1; i = i + 2)
                {
                    IQueryable<Material> materials = context.Materials.Where(m => m.Name.Equals(splittedString.GetValue(i)));
                    Material theMaterial = materials.FirstOrDefault();
                    if (theMaterial != null)
                    {
                        Hat_Made_Of_Material newEntry = new Hat_Made_Of_Material
                        {
                            MaterialId = theMaterial.MID,
                            Quantity = splittedString.GetValue(i + 1).ToString(),
                            Hat = specialHat
                        };
                        newOrderVM.MaterialsInHat.Add(newEntry);
                        context.HatmadeOfMaterials.Add(newEntry);
                        context.SaveChanges();

                    }
                    else
                    {
                        Debug.WriteLine("Hittar inte objhektet i db ");
                        
                    }
                }
            }

            foreach (var hat in newOrderVM.HatsInOrder)
            {
                newOrderVM.CurrentOrder.TotPrice += hat.Price;
            }

            
           

            return View("NewOrder", newOrderVM);
        }

        [HttpPost]
        public IActionResult RemoveHatFromOrder(int customerId,
            bool customerConfirmed,
            bool express,
            List<int> addedHatIds,
            List<int> orderHat_HIDs,
            List<double> orderHat_HeadSizes,
            List<double> orderHat_Heights,
            int removeIndex)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var newOrderVM = new NewOrderViewModel();

            Customer theCustomer = context.Customers.FirstOrDefault(c => c.CID == customerId);

            newOrderVM.CurrentOrder = new Order
            {
                Customer = theCustomer,
                ExpressDelivery = express
            };
            newOrderVM.CustomerConfirmed = customerConfirmed;
            newOrderVM.StandardHats = context.Hats.ToList();
            newOrderVM.HatsInOrder = new List<Hat>();
            newOrderVM.OrderContainsHats = new List<Order_Contains_Hat>();
            newOrderVM.MaterialsInDb = context.Materials.ToList();

            for (int i = 0; i < orderHat_HIDs.Count; i++)
            {
                if (i == removeIndex)
                {
                    // Skippa hatten som ska tas bort
                    continue;
                }

                int currentHID = orderHat_HIDs[i];
                double currentHeadSize = orderHat_HeadSizes[i];
                double currentHeight = orderHat_Heights[i];

                var hat = context.Hats.FirstOrDefault(h => h.HID == currentHID);
                if (hat != null && !newOrderVM.HatsInOrder.Contains(hat))
                {
                    newOrderVM.HatsInOrder.Add(hat);
                }

                newOrderVM.OrderContainsHats.Add(new Order_Contains_Hat
                {
                    Hat = hat,
                    HeadSize = currentHeadSize,
                    Height = currentHeight
                });
            }

            // Uppdatera pris
            foreach (Hat hat in newOrderVM.HatsInOrder)
            {
                newOrderVM.CurrentOrder.TotPrice += hat.Price;
            }

            return View("NewOrder", newOrderVM);
        }


        [HttpPost]
        public IActionResult AddOrder(int customerId, List<int> hatIds, List<double> headSizes, List<double> heights, List<string> descriptions)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }

            var newOrderVM = new NewOrderViewModel();

            Customer theCustomer = context.Customers.FirstOrDefault(c => c.CID == customerId);

            newOrderVM.CurrentOrder = new Order
            {
                Customer = theCustomer,
                DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3))

            };
            newOrderVM.CustomerConfirmed = true;
            newOrderVM.StandardHats = context.Hats.ToList();
            newOrderVM.HatsInOrder = new List<Hat>();
            newOrderVM.OrderContainsHats = new List<Order_Contains_Hat>();
            newOrderVM.CurrentOrder.DeliveryAddress = theCustomer.Address;


            for (int i = 0; i < hatIds.Count; i++)
            {
                

                int currentHID = hatIds[i];
                double currentHeadSize = headSizes[i];
                double currentHeight = heights[i];
                

                var hat = context.Hats.FirstOrDefault(h => h.HID == currentHID);
                if (hat != null && !newOrderVM.HatsInOrder.Contains(hat))
                {
                    if (hat.IsSpecial)
                    {
                        hat.SpecialDescription = descriptions[i];
                    }
                    newOrderVM.HatsInOrder.Add(hat);
                    
                }
                
                
                Debug.WriteLine("Hat läggs till i listan");
                Debug.WriteLine("id:" + currentHID);
                Debug.WriteLine("storlek:" + currentHeadSize);
                Debug.WriteLine("höjd:" + currentHeadSize);
                newOrderVM.OrderContainsHats.Add(new Order_Contains_Hat
                {
                    
                    HatId = currentHID, 
                    Hat = hat,
                    HeadSize = currentHeadSize,
                    Height = currentHeight
                });

            }

            // Uppdatera pris
            foreach (Hat hat in newOrderVM.HatsInOrder)
            {
                newOrderVM.CurrentOrder.TotPrice += hat.Price;
            }

            return View(newOrderVM);
        }




        [HttpPost]
        public IActionResult CreateNewOrder(
            NewOrderViewModel model,
            List<int> hatsInOrder,
            List<double> headSizes,
            List<double> heights,
            int customerId,
            double totPrice)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            Order order = model.CurrentOrder;
            order.CustomerId = customerId;
            order.TotPrice = totPrice;
            
            
            if (order.ExpressDelivery)
            {
                order.TotPrice = (order.TotPrice * 1.2);
            }

            IQueryable<User> users = context.Users.Where(u => u.Email == User.Identity.Name);
            User logInUser = users.FirstOrDefault();
            order.CreatedById = logInUser.Id;

            context.Orders.Add(order);
            context.SaveChanges();

            Order largestOrder = context.Orders.OrderByDescending(o => o.OID).FirstOrDefault();
            int orderId = largestOrder.OID;


            for (int i = 0; i < hatsInOrder.Count; i++)
            {
                Order_Contains_Hat och = new Order_Contains_Hat();
                och.OrderId = orderId;
                och.HatId = hatsInOrder[i];
                och.HeadSize = headSizes[i];
                och.Height = heights[i];
                context.OrderContainsHats.Add(och);
                context.SaveChanges();
            }

            return RedirectToAction("AllOrders", "Order");
        }

        [HttpPost]
        public IActionResult UpdateDeliveryDateForOrder(int orderID, DateTime deliveryDate)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var order = context.Orders.FirstOrDefault(o => o.OID == orderID);
            if (ModelState.IsValid && order != null)
            {
                order.DeliveryDate = DateOnly.FromDateTime(deliveryDate);
                context.Update(order);
                context.SaveChanges();
            }

            return RedirectToAction("JoinOrder", new { orderID });
        }

        [HttpPost]
        public IActionResult UpdateDeliveryForOrderDragAndDrop([FromBody] OrderDateUpdateDnD dto)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var order = context.Orders.FirstOrDefault(o => o.OID == dto.OrderID);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

            try
            {
                var parsedDate = DateOnly.ParseExact(dto.DeliveryDate, "yyyy-MM-dd");
                order.DeliveryDate = parsedDate;

                context.Update(order);
                context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Invalid date format or database error" });
            }
        }

        public class OrderDateUpdateDnD
        {
            public int OrderID { get; set; }
            public string DeliveryDate { get; set; }
        }


        [HttpGet]
        public IActionResult JoinOrder(int orderID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var order = context.Orders
                .Where(o => o.Status != "Avslutad" && o.OID == orderID)
                .Include(o => o.HatsInOrder)
                    .ThenInclude(ho => ho.Hat)
                .Include(o => o.User).FirstOrDefault();

            var users = context.Users.ToList();

            var viewModel = new User_Order_ViewModel
            {
                AllOrders = new List<Order> { order },
                AllUsers = users
            };

            var userHatLinks = context.UserManageHatOrders.ToList();
            ViewBag.UserHatLinks = userHatLinks;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus([FromBody] OrderStatusUpdateRequest request)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            try
            {
                var order = context.Orders.FirstOrDefault(o => o.OID == request.OrderId);
                if (order != null)
                {
                    order.Status = request.NewStatus;
                    context.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Ordern inte hittad" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public class OrderStatusUpdateRequest
        {
            public int OrderId { get; set; }
            public string NewStatus { get; set; }
        }

        public IActionResult AllOrders()
        {
            
            var orders = context.Orders.ToList();

            
            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int orderID, string status)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var order = context.Orders.Find(orderID);
            if (order == null)
            {
                return BadRequest("Order not found");
            }

            order.Status = status;
            context.SaveChanges();

            return Ok(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> JoinOrderAction(int orderId, List<int> SelectedHatIds, Dictionary<int, string> SelectedUserIds)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var order = context.Orders
                .Include(o => o.HatsInOrder)
                .FirstOrDefault(o => o.OID == orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Ordern finns inte.";
                return RedirectToAction("AllOrders");
            }

            if (SelectedHatIds == null || !SelectedHatIds.Any())
            {
                TempData["ErrorMessage"] = "Vänligen välj minst en hatt.";
                return RedirectToAction("JoinOrder", new { orderId });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await context.Users.FindAsync(userId);
            var isAdmin = currentUser?.IsAdmin ?? false;

            List<int> alreadyTakenHats = new();
            List<int> alreadyHandledByUser = new();

            foreach (var hatId in SelectedHatIds)
            {
                var selectedHat = order.HatsInOrder.FirstOrDefault(h => h.HatId == hatId);

                if (selectedHat != null)
                {
                    bool isTaken = context.UserManageHatOrders
                        .Any(u => u.OrderId == orderId && u.HatId == hatId);

                    if (isTaken)
                    {
                        alreadyTakenHats.Add(hatId);
                        continue;
                    }

                    string userIdToAssign = null;

                    if (SelectedUserIds.ContainsKey(hatId) && !string.IsNullOrEmpty(SelectedUserIds[hatId]))
                    {
                        userIdToAssign = SelectedUserIds[hatId];
                    }
                    else
                    {
                        continue;
                    }

                    bool alreadyHandled = context.UserManageHatOrders
                        .Any(u => u.UserId == userIdToAssign && u.OrderId == orderId && u.HatId == hatId);

                    if (alreadyHandled)
                    {
                        alreadyHandledByUser.Add(hatId);
                        continue;
                    }

                    var newEntry = new User_Manage_Hat_Orders
                    {
                        UserId = userIdToAssign,
                        OrderId = orderId,
                        HatId = hatId
                    };
                    context.UserManageHatOrders.Add(newEntry);
                }
                else
                {
                    TempData["ErrorMessage"] = "En eller flera valda hattar finns inte i ordern.";
                    return RedirectToAction("JoinOrder", new { orderId });
                }
            }

            await context.SaveChangesAsync();

            if (alreadyHandledByUser.Any())
            {
                TempData["InfoMessage"] = "Du var redan kopplad till en eller flera valda hattar.";
            }

            
            if (!alreadyTakenHats.Any() && !alreadyHandledByUser.Any())
            {
                TempData["SuccessMessage"] = isAdmin
                    ? "Hatt/hattar har tilldelats till valda användare."
                    : "Du har gått med i ordern och valt hattar att jobba på.";
            }
            else if (!alreadyTakenHats.Any() && alreadyHandledByUser.Any())
            {
                TempData["InfoMessage"] = "Du har redan valt vissa hattar, men några andra hattar har tilldelats till andra användare.";
            }

            return RedirectToAction("JoinOrder", new { orderId });
        }

        [HttpGet]
        public IActionResult GetWaybillPDF(int orderID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            var order = context.Orders
                .Where(o => o.Status == "Avslutad")
                .Include(o => o.Customer)
                .FirstOrDefault(o => o.OID == orderID);

            var pdfBytes = CreateWaybill(order);


            return File(pdfBytes, "application/pdf", $"waybill_{order.OID}.pdf");
        }

        private byte[] CreateWaybill(Order order)
        {
            string packageID1 = order.OID.ToString();
            string packageID2 = new Random().Next(100000000, 999999999).ToString();

            byte[] barcode1 = GenerateBarcode(packageID1);
            byte[] barcode2 = GenerateBarcode(packageID2);

            using var stream = new MemoryStream();
            var document = new PdfSharpCore.Pdf.PdfDocument();
            var page = document.AddPage();
            var gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);

            var pageWidth = page.Width;
            var pageHeight = page.Height;
            double margin = 40;
            double contentPadding = 20;
            double currentY = margin + contentPadding;

            var titleFont = new XFont("Arial", 20, XFontStyle.Bold);
            var sectionFont = new XFont("Arial", 14, XFontStyle.Bold);
            var normalFont = new XFont("Arial", 12, XFontStyle.Regular);

            
            gfx.DrawRectangle(XPens.Black, margin / 2, margin / 2, pageWidth - margin, pageHeight - margin);

            // Postnord
            gfx.DrawString("Postnord", sectionFont, XBrushes.Black,
                new XRect(margin + contentPadding, currentY, pageWidth - (margin + contentPadding) * 2, 30), XStringFormats.TopRight);
            currentY += 30;
            gfx.DrawLine(XPens.Black, margin, currentY, pageWidth - margin, currentY);
            currentY += 10;

            // Titel
            gfx.DrawString("FRAKTSEDEL", titleFont, XBrushes.Black,
                new XRect(margin, currentY, pageWidth - margin * 2, 40), XStringFormats.TopCenter);
            currentY += 50;
            gfx.DrawLine(XPens.Black, margin, currentY, pageWidth - margin, currentY);
            currentY += 15;

            // Avsändare
            gfx.DrawString("From:", sectionFont, XBrushes.Black,
                new XRect(margin + contentPadding, currentY, pageWidth - (margin + contentPadding) * 2, 20), XStringFormats.TopLeft);
            currentY += 25;

            string[] fromLines = {
                "Hattmakarna AB",
                "SE 70182 Örebro",
                "Sweden"
             };

            foreach (var line in fromLines)
            {
                gfx.DrawString(line, normalFont, XBrushes.Black,
                    new XRect(margin + contentPadding, currentY, pageWidth - (margin + contentPadding) * 2, 20), XStringFormats.TopLeft);
                currentY += 20;
            }

            gfx.DrawLine(XPens.Black, margin, currentY, pageWidth - margin, currentY);
            currentY += 15;

            // Mottagare
            double recipientBoxTop = currentY; // <- sparar var mottagarsektionen börjar
            gfx.DrawString("To:", sectionFont, XBrushes.Black,
                new XRect(margin + contentPadding, currentY, pageWidth - (margin + contentPadding) * 2, 20), XStringFormats.TopLeft);
            currentY += 25;

            string[] recipientLines = {
                $"Mottagare: {order.Customer?.Firstname ?? "Okänd"} {order.Customer?.Lastname ?? "Okänd"}",
                $"Adress: {order.DeliveryAddress ?? "Okänd"}",
                $"Tull-nr: {order.CustomsNumbers ?? "Okänt"}",
                $"Paket-ID: {packageID1}"
            };

            foreach (var line in recipientLines)
            {
                gfx.DrawString(line, normalFont, XBrushes.Black,
                    new XRect(margin + contentPadding, currentY, pageWidth - (margin + contentPadding) * 2, 20), XStringFormats.TopLeft);
                currentY += 20;
            }
            // Parametrar för hörn
            double boxTop = recipientBoxTop;
            double boxHeight = currentY - boxTop;
            double boxLeft = margin + contentPadding - 10;
            double boxRight = pageWidth - (margin + contentPadding) + 10;
            double cornerLength = 20;

            // Övre vänstra hörnet
            gfx.DrawLine(XPens.Black, boxLeft, boxTop, boxLeft + cornerLength, boxTop); // horisontell
            gfx.DrawLine(XPens.Black, boxLeft, boxTop, boxLeft, boxTop + cornerLength); // vertikal

            // Övre högra hörnet
            gfx.DrawLine(XPens.Black, boxRight, boxTop, boxRight - cornerLength, boxTop);
            gfx.DrawLine(XPens.Black, boxRight, boxTop, boxRight, boxTop + cornerLength);

            // Nedre vänstra hörnet
            gfx.DrawLine(XPens.Black, boxLeft, boxTop + boxHeight, boxLeft + cornerLength, boxTop + boxHeight);
            gfx.DrawLine(XPens.Black, boxLeft, boxTop + boxHeight, boxLeft, boxTop + boxHeight - cornerLength);

            // Nedre högra hörnet
            gfx.DrawLine(XPens.Black, boxRight, boxTop + boxHeight, boxRight - cornerLength, boxTop + boxHeight);
            gfx.DrawLine(XPens.Black, boxRight, boxTop + boxHeight, boxRight, boxTop + boxHeight - cornerLength);


            currentY += 10;

            // Streckkod 1
            using (var ms1 = new MemoryStream(barcode1))
            {
                var img1 = XImage.FromStream(() => ms1);
                double xCenter = (page.Width - img1.PixelWidth) / 2;
                gfx.DrawImage(img1, xCenter, currentY, img1.PixelWidth, img1.PixelHeight);
                currentY += img1.PixelHeight + 20;
            }

            // Streckkod 2
            using (var ms2 = new MemoryStream(barcode2))
            {
                var img2 = XImage.FromStream(() => ms2);
                double xCenter = (page.Width - img2.PixelWidth) / 2;
                gfx.DrawImage(img2, xCenter, currentY, img2.PixelWidth, img2.PixelHeight);
                currentY += img2.PixelHeight + 20;
            }

            document.Save(stream, false);
            return stream.ToArray();
        }

        private byte[] GenerateBarcode(string data)
        {
            var writer = new BarcodeWriter<SKBitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 80,
                    Width = 240,
                    Margin = 10
                },
                Renderer = new SKBitmapRenderer()
            };
            using (var skBitmap = writer.Write(data))
            using (var picStream = new MemoryStream())
            {
                skBitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(picStream);
                return picStream.ToArray();
            }
        }


    }
}
