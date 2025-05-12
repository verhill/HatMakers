using hatmaker_team2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace hatmaker_team2.Controllers
{
    public class CustomerController : Controller
    {
        private ModelContext context;
        public CustomerController(ModelContext context)
        {
            this.context = context;
        }

        public IActionResult AllCustomers()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            List<Customer> customers = new List<Customer>();

            IQueryable<Customer> dbCustomers = from c in context.Customers select c;
            dbCustomers = dbCustomers.Where(c => c.IsActive);
            customers = dbCustomers.ToList();

            return View(customers);
        }

        [HttpGet]
        public IActionResult EditCustomer(int CID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            IQueryable<Customer> dbCustomers = from c in context.Customers select c;
            dbCustomers = dbCustomers.Where(c => c.CID == CID);
            Customer theCustomer = dbCustomers.FirstOrDefault();

            return View(theCustomer);
        }

        [HttpPost]
        public IActionResult EditCustomerInfo(Customer customer)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            List<Customer> customers = new List<Customer>();
            IQueryable<Customer> dbCustomers = from c in context.Customers select c;
            dbCustomers = dbCustomers.Where(c => c.IsActive);
            customers = dbCustomers.ToList();

            if (ModelState.IsValid)
            {
                Customer emailExists = context.Customers.FirstOrDefault(c => c.Email == customer.Email && c.CID != customer.CID);

                if (emailExists != null)
                {
                    ModelState.AddModelError("", "Det finns redan en kund med denna epost");
                    return View("EditCustomer", customer);
                }

                Customer existingCustomer = context.Customers.FirstOrDefault(c => c.CID == customer.CID);
                existingCustomer.Firstname = customer.Firstname;
                existingCustomer.Lastname = customer.Lastname;
                existingCustomer.Email = customer.Email;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.Address = customer.Address;
                existingCustomer.City = customer.City;
                existingCustomer.Country = customer.Country;

                context.SaveChanges();
                return RedirectToAction("AllCustomers", customers);
            }
            else
            {
                return View("EditCustomer", customer);
            }

        }

        [HttpPost]
        public IActionResult DeleteCustomer(int CID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            Customer existingCustomer = context.Customers.FirstOrDefault(c => c.CID == CID);
           existingCustomer.IsActive = false;

           context.SaveChanges();

            List<Customer> customers = new List<Customer>();
            IQueryable<Customer> dbCustomers = from c in context.Customers select c;
            dbCustomers = dbCustomers.Where(c => c.IsActive);
            customers = dbCustomers.ToList();

            return RedirectToAction("AllCustomers", customers);
            

        }
    }
}
