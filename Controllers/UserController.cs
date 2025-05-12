using hatmaker_team2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography;

namespace hatmaker_team2.Controllers
{
    public class UserController : Controller
    {
        private ModelContext context;
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;

        public UserController(UserManager<User> userManag, SignInManager<User> signInMang, ModelContext context)
        {
            this.userManager = userManag;
            this.signInManager = signInMang;
            this.context = context;
            SeedUsersIfDbEmpty();
            SeedDataIfDbEmpty();
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            return View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                loginViewModel.Epost,
                loginViewModel.Losenord,
                isPersistent: loginViewModel.RememberMe,
                lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Fel användarnam/lösenord.");
                }
            }
            return View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "User");
        }


        public IActionResult AllUsers()
        {
            IQueryable<User> UserList = from u in context.Users select u;
            List<User> users = new List<User>();
            UserList = UserList.Where(c => c.IsDeactivated == false);
            users = UserList.ToList();
            return View(users);
        }
        public void SeedDataIfDbEmpty()
        {
            IQueryable<Hat> hatList = from hat in context.Hats select hat;
            Hat existHat = hatList.FirstOrDefault();
            IQueryable<Customer> customerList = from c in context.Customers select c;
            Customer existCust = customerList.FirstOrDefault();

            if (existHat != null && existCust != null)
            {
                return;
            }

            // === Customers ===
            var customers = new List<Customer>
                {
                    new Customer { Firstname = "Lars", Lastname = "Persson", Address = "Gatan 23", City = "Örebro", Country = "Sverige", Email = "lars@outlook.com", Phone = "0701234567" },
                    new Customer { Firstname = "Anna", Lastname = "Svensson", Address = "Vägen 45", City = "Uppsala", Country = "Sverige", Email = "anna@email.com", Phone = "0702345678" },
                    new Customer { Firstname = "Olof", Lastname = "Karlsson", Address = "Stigen 12", City = "Lund", Country = "Sverige", Email = "olof@gmail.com", Phone = "0733456789" },
                    new Customer { Firstname = "Emma", Lastname = "Johansson", Address = "Torget 5", City = "Göteborg", Country = "Sverige", Email = "emma@hotmail.com", Phone = "0724567890" },
                    new Customer { Firstname = "Erik", Lastname = "Nilsson", Address = "Allén 8", City = "Malmö", Country = "Sverige", Email = "erik@yahoo.com", Phone = "0765678901" },
                };
            context.Customers.AddRange(customers);
            context.SaveChanges();

            // === Materials ===
            var materials = new List<Material>
                {
                    new Material { Name = "Ullfilt" },
                    new Material { Name = "Kaninfilt" },
                    new Material { Name = "Toquillastrå" },
                    new Material { Name = "Rishalm" },
                    new Material { Name = "Halmlöv" },
                    new Material { Name = "Majsblad" },
                    new Material { Name = "Hampfiber" },
                    new Material { Name = "Bomull" },
                    new Material { Name = "Linne" },
                    new Material { Name = "Ull" },
                    new Material { Name = "Band" },
                    new Material { Name = "Fjädrar" },
                    new Material { Name = "Läder" },
                    new Material { Name = "Tygblommor" },
                    new Material { Name = "Pärlor" },
                    new Material { Name = "Spets" },
                    new Material { Name = "Lakerat papper" },
                    new Material { Name = "Lurextråd" },
                    new Material { Name = "Fuskpäls" },
                    new Material { Name = "Silke" },
                    new Material { Name = "Satin" },
                    new Material { Name = "Tweed" },
                    new Material { Name = "Polyester" },
                    new Material { Name = "Trä" },
                    new Material { Name = "Plast" },
                    new Material { Name = "Metall" },
                    
                };
            context.Materials.AddRange(materials);
            context.SaveChanges();

            // === Hats ===
            var hats = new List<Hat>
                {
                    new Hat { Name = "Filthatt", Price = 500, Picture = "/images/Filthatt.png" },
                    new Hat { Name = "Panamahatt", Price = 600, Picture = "/images/panamahatt.png" },
                    new Hat { Name = "Stråhatt", Price = 449, Picture = "/images/strahatt.png" },
                    new Hat { Name = "Tyghatt", Price = 700, Picture = "/images/tyghatt.png" },
                    new Hat { Name = "Läderhätta", Price = 549, Picture = "/images/Laderhatta.png" },
                };
            context.Hats.AddRange(hats);
            context.SaveChanges();

            // === Orders ===
            var orders = new List<Order>
                {
                    new Order { DeliveryDate = new DateOnly(2025, 6, 6), TotPrice = 500, CreatedById = "user1-id", CustomerId = 1 },
                    new Order { DeliveryDate = new DateOnly(2025, 6, 10), TotPrice = 600, CreatedById = "user2-id", CustomerId = 1 },
                    new Order { DeliveryDate = new DateOnly(2025, 6, 12), TotPrice = 449, CreatedById = "user1-id", CustomerId = 2 },
                    new Order { DeliveryDate = new DateOnly(2025, 6, 15), TotPrice = 700, CreatedById = "user3-id", CustomerId = 3 },
                    new Order { DeliveryDate = new DateOnly(2025, 6, 18), TotPrice = 549, CreatedById = "user2-id", CustomerId = 5 },
                };
            context.Orders.AddRange(orders);
            context.SaveChanges();

            // === Hat_Made_Of_Material ===
            var hmom = new List<Hat_Made_Of_Material>
                {
                    new Hat_Made_Of_Material { HatId = 1, MaterialId = 1 }, //Filthatt
                    new Hat_Made_Of_Material { HatId = 2, MaterialId = 3 }, //Panamahatt
                    new Hat_Made_Of_Material { HatId = 3, MaterialId = 4 }, //Stråhatt
                    new Hat_Made_Of_Material { HatId = 3, MaterialId = 5 }, //Stråhatt
                    new Hat_Made_Of_Material { HatId = 3, MaterialId = 6 }, //Stråhatt
                    new Hat_Made_Of_Material { HatId = 3, MaterialId = 7 }, //Stråhatt
                    new Hat_Made_Of_Material { HatId = 4, MaterialId = 8 }, //Tyghatt
                    new Hat_Made_Of_Material { HatId = 5, MaterialId = 13 }, //Läderhätta
                };
            context.HatmadeOfMaterials.AddRange(hmom);
            context.SaveChanges();

            // === Order_Contains_Hat ===
            var och = new List<Order_Contains_Hat>
                {
                    new Order_Contains_Hat { HatId = 1, OrderId = 1 },
                    new Order_Contains_Hat { HatId = 2, OrderId = 2 },
                    new Order_Contains_Hat { HatId = 3, OrderId = 3 },
                    new Order_Contains_Hat { HatId = 4, OrderId = 4 },
                    new Order_Contains_Hat { HatId = 5, OrderId = 5 },
                };
            context.OrderContainsHats.AddRange(och);
            context.SaveChanges();

            // === User_Manage_Hat_Orders ===
            var umho = new List<User_Manage_Hat_Orders>
                {
                    new User_Manage_Hat_Orders { HatId = 1, OrderId = 1, UserId = "user1-id" },
                    new User_Manage_Hat_Orders { HatId = 1, OrderId = 2, UserId = "user2-id" },
                    new User_Manage_Hat_Orders { HatId = 2, OrderId = 3, UserId = "user1-id" },
                    new User_Manage_Hat_Orders { HatId = 3, OrderId = 3, UserId = "user3-id" },
                    new User_Manage_Hat_Orders { HatId = 4, OrderId = 4, UserId = "user2-id" },
                };
            context.UserManageHatOrders.AddRange(umho);
            context.SaveChanges();
        }

        public void SeedUsersIfDbEmpty()
        {
            IQueryable<User> userList = from user in context.Users select user;
            User userExists = userList.FirstOrDefault();
            if (userExists != null)
            {
                return;
            }
            else
            {
                User user1 = new User
                {
                    Id = "user1-id",
                    Firstname = "Otto",
                    Lastname = "Hattmakarsson",
                    UserName = "otto@hatmail.com",
                    NormalizedUserName = "OTTO@HATMAIL.COM",
                    Email = "otto@hatmail.com",
                    NormalizedEmail = "OTTO@HATMAIL.COM",
                    EmailConfirmed = true,
                    IsAdmin = true,
                    SecurityStamp = "static-stamp-1",
                    PasswordHash = "AQAAAAIAAYagAAAAEB9kc022lIacHYyCoxijpmN+44SOYcC7aU9dT8mrGfLCZhD/iaJVzaxrWrWGklfeMA=="
                };


                User user2 = new User
                {
                    Id = "user2-id",
                    Firstname = "Judith",
                    Lastname = "Hattsson",
                    UserName = "judith@hatmail.com",
                    NormalizedUserName = "JUDITH@HATMAIL.COM",
                    Email = "judith@hatmail.com",
                    NormalizedEmail = "JUDITH@HATMAIL.COM",
                    EmailConfirmed = true,
                    IsAdmin = true,
                    SecurityStamp = "static-stamp-2",
                    PasswordHash = "AQAAAAIAAYagAAAAEB9kc022lIacHYyCoxijpmN+44SOYcC7aU9dT8mrGfLCZhD/iaJVzaxrWrWGklfeMA=="
                };


                User user3 = new User
                {
                    Id = "user3-id",
                    Firstname = "Tanja",
                    Lastname = "Havstorm",
                    UserName = "tanja@hatmail.com",
                    NormalizedUserName = "TANJA@HATMAIL.COM",
                    Email = "tanja@hatmail.com",
                    NormalizedEmail = "TANJA@HATMAIL.COM",
                    EmailConfirmed = true,
                    IsAdmin = true,
                    SecurityStamp = "static-stamp-3",
                    PasswordHash = "AQAAAAIAAYagAAAAEB9kc022lIacHYyCoxijpmN+44SOYcC7aU9dT8mrGfLCZhD/iaJVzaxrWrWGklfeMA=="
                };


                User user4 = new User
                {
                    Id = "user4-id",
                    Firstname = "Andreas",
                    Lastname = "Ask",
                    UserName = "andreas@hatmail.com",
                    NormalizedUserName = "ANDREAS@HATMAIL.COM",
                    Email = "andreas@hatmail.com",
                    NormalizedEmail = "ANDREAS@HATMAIL.COM",
                    EmailConfirmed = true,
                    IsAdmin = true,
                    SecurityStamp = "static-stamp-4",
                    PasswordHash = "AQAAAAIAAYagAAAAEB9kc022lIacHYyCoxijpmN+44SOYcC7aU9dT8mrGfLCZhD/iaJVzaxrWrWGklfeMA=="
                };

                context.Users.Add(user1);
                context.Users.Add(user2);
                context.Users.Add(user3);
                context.Users.Add(user4);
                context.SaveChanges();
            }



               
        }

        [HttpGet]
        public IActionResult EditUser(string Id)
        {
           IQueryable<User> loginUser = from u in context.Users select u;
           loginUser = loginUser.Where(u => u.UserName == User.Identity.Name);
           User loggedInUser = loginUser.ToList().FirstOrDefault();

            if (!loggedInUser.IsAdmin)
            {
                IQueryable<User> UserList = context.Users.Where(u => u.IsDeactivated == false);
                List<User> users = new List<User>();
                users = UserList.ToList();
                ModelState.AddModelError("", "Du har ej behörighet för att göra detta!");
                return View("AllUsers", users);
            }
                IQueryable<User> dbUsers = from u in context.Users select u;
                dbUsers = dbUsers.Where(u => u.Id == Id);
                User theUser = dbUsers.FirstOrDefault();
                return View(theUser);
            
        }

        [HttpPost]
        public IActionResult EditUserInfo(User user)
        {

            IQueryable<User> UserList = from u in context.Users select u;
            List<User> users = new List<User>();
            users = UserList.ToList();

            if (ModelState.IsValid)
            {
                User existingUser = context.Users.FirstOrDefault(u => u.Id == user.Id);
                existingUser.Firstname = user.Firstname;
                existingUser.Lastname = user.Lastname;
                context.SaveChanges();
                return RedirectToAction("AllUsers", users);
            }
            else
            {
                return View("EditUser", user);
            }

        }


        [HttpGet]
        public IActionResult CreateUser()
        {
            IQueryable<User> loginUser = from u in context.Users select u;
            loginUser = loginUser.Where(u => u.UserName == User.Identity.Name);
            User loggedInUser = loginUser.ToList().FirstOrDefault();

            if (!loggedInUser.IsAdmin)
            {
                IQueryable<User> UserList = from u in context.Users select u;
                List<User> users = new List<User>();
                users = UserList.ToList();
                ModelState.AddModelError("", "Du har ej behörighet för att göra detta!");
                return View("AllUsers", users);
            }
          
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserInDb(CreateUserViewModel model)
        {

            if (ModelState.IsValid)
            {            
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    IsAdmin = model.IsAdmin,
                    EmailConfirmed = true
                    
                };

                var existingUser = context.Users.FirstOrDefault(u => u.Email == model.Email);

                if (existingUser != null && existingUser.IsDeactivated == false)
                {
                    ModelState.AddModelError("", "Epost-adressen används redan.");           
                    return View("CreateUser", model);
                }

                

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("AllUsers");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Debug.WriteLine(error.Code);
                    }
                    ModelState.AddModelError("", "Lösenordet måste innehålla:");
                    ModelState.AddModelError("", "Minst 6 tecken långt");
                    ModelState.AddModelError("", "Bokstav, stor och liten");
                    ModelState.AddModelError("", "Tecken (ex: !%&=?)");
                    return View("CreateUser");
                }

            }
            else
            {
                
                return View("CreateUser");

            }
            

        }

        [HttpPost]
        public IActionResult DeleteUser(string Id) 
        {
            IQueryable<User> loginUser = from u in context.Users select u;
            loginUser = loginUser.Where(u => u.UserName == User.Identity.Name);
            User loggedInUser = loginUser.ToList().FirstOrDefault();

            IQueryable<User> UserList = context.Users.Where(u => u.IsDeactivated == false);
            List<User> users = new List<User>();
            users = UserList.ToList();

            if (!loggedInUser.IsAdmin)
            {
                
                ModelState.AddModelError("", "Du har ej behörighet att ta bort en användare!");
                return View("AllUsers", users);
            }

            User existingUser = context.Users.FirstOrDefault(u => u.Id == Id);

            if(loggedInUser == existingUser)
            {
                ModelState.AddModelError("", "Du kan ej ta bort dig själv!");
                return View("AllUsers", users);
            }

            existingUser.IsDeactivated = true;
            context.SaveChanges();

            return RedirectToAction("AllUsers");

        }     
    }
}