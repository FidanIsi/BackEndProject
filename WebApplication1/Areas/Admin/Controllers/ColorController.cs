using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;

namespace WebApplication1.Areas.Admin.Controllers
{
    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            IEnumerable<Color> colors = _context.Colors.Include(c => c.ProductColors).ThenInclude(pc => pc.Product);
            return View(colors);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Add")]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Color newColor)
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

            bool isDuplicated = _context.Colors.Any(c => c.ColorName == newColor.ColorName);
            if (isDuplicated)
            {
                ModelState.AddModelError("", "You cannot duplicate value");
                return View();
            }

            _context.Colors.Add(newColor);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            Color colorToUpdate = _context.Colors.Find(id);

            if (colorToUpdate == null)
            {
                return NotFound();
            }

            return View(colorToUpdate);
        }

        [HttpPost]
        [ActionName("Update")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Update(int id, Color editedColor)
        {
            if (id != editedColor.Id)
            {
                return BadRequest();
            }

            Color colorToUpdate = _context.Colors.Find(id);

            if (colorToUpdate == null)
            {
                return NotFound();
            }

            bool duplicate = _context.Colors.Any(c => c.ColorName == editedColor.ColorName && editedColor.ColorName != colorToUpdate.ColorName);

            if (duplicate)
            {
                ModelState.AddModelError("", "You cannot duplicate color name");
                return View(colorToUpdate);
            }

            colorToUpdate.ColorName = editedColor.ColorName;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            Color colorToDelete = _context.Colors.Find(id);

            if (colorToDelete == null)
            {
                return NotFound();
            }

            return View(colorToDelete);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Color colorToDelete = _context.Colors.Find(id);

            if (colorToDelete == null)
            {
                return NotFound();
            }

            _context.Colors.Remove(colorToDelete);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

    }
}
