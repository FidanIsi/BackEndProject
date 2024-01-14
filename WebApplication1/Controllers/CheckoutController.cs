using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly AppDbContext _context;

        public CheckoutController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            Checkout check = _context.Checkouts.FirstOrDefault();
            List<Country> countries = _context.Countries.AsNoTracking().ToList();

            var model = new CheckoutVM()
            {
                Countries = countries,
            };

            if (check != null)
            {
                model.FirstName = check.FirstName;
                model.LastName = check.LastName;
                model.CompanyName = check.CompanyName;
                model.Address = check.Address;
                model.Email = check.Email;
                model.Town = check.Town;
                model.PhoneNumber = check.PhoneNumber;
                model.ZipCode = check.ZipCode;
                model.Comment = check.Comment;
                model.CountryId = check.CountryId;
            }

            return View(model);
        }


        [HttpPost]
        public IActionResult Index(CheckoutVM model)
        {
            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }

                return View();
            }

            bool isDuplicated = _context.Checkouts
                .Any(c => c.FirstName == model.FirstName && c.LastName == model.LastName);

            if (isDuplicated)
            {
                ModelState.AddModelError("", "You cannot duplicate value");
                return View();
            }

            Checkout checkout = new Checkout()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                CompanyName = model.CompanyName,
                Address = model.Address,
                Email = model.Email,
                Town = model.Town,
                PhoneNumber = model.PhoneNumber,
                ZipCode = model.ZipCode,
                Comment = model.Comment,
                CountryId = model.CountryId,
            };

            _context.Checkouts.Add(checkout);
            _context.SaveChanges();

            // Fetch countries again after saving to the database
            List<Country> countries = _context.Countries.AsNoTracking().ToList();

            // Create a new instance of CheckoutVM with updated countries list
            var updatedModel = new CheckoutVM()
            {
                Countries = countries,
            };

            return RedirectToAction("Index", "Home");
        }
    }
}
