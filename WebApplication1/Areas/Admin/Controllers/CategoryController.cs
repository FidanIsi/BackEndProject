using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Entities;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _context.Categories.AsEnumerable();
            return View(categories);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Add")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Add(Category newCategory)
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
            bool isDuplicated = _context.Categories.Any(c => c.CategoryName == newCategory.CategoryName);
            if (isDuplicated)
            {
                ModelState.AddModelError("", "You cannot duplicate value");
                return View();
            }
            _context.Categories.Add(newCategory);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category is null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var categoryToDelete = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (categoryToDelete == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(categoryToDelete);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            if (id == 0) return NotFound();
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category is null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Update(int id, Category edited)
        {
            if (id != edited.Id) return BadRequest();
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category is null) return NotFound();
            bool duplicate = _context.Categories.Any(c => c.CategoryName == edited.CategoryName && edited.CategoryName != category.CategoryName);//test == albert 
            if (duplicate)
            {
                ModelState.AddModelError("", "You cannot duplicate category name");
                return View(category);
            }
            category.CategoryName = edited.CategoryName;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
