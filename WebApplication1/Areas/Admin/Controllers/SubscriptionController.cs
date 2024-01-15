using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Entities;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SubscriptionController : Controller
    {
        private AppDbContext _context;

        public SubscriptionController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Subscription> subscriptions = _context.Subscriptions.ToList();

            return View(subscriptions);
        }
    }
}
