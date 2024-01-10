using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Add()
        {
            return View();
        }
        public IActionResult Update()
        {
            return View();
        }
    }
}
