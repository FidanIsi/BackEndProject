using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class SubscriptionController : Controller
    {
        private AppDbContext _context;

        public SubscriptionController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(SubscriptionVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var subscription = new Subscription
            {
                EmailAddress = model.EmailAddress
            };

            _context.Add(subscription);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }

}
