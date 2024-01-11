using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Admin.Models;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Services;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController: Controller
    {
        private readonly AppDbContext _context;
        private readonly FileService _fileService;

        public ProductController(AppDbContext context, FileService fileService)
        {
            _context = context;
             _fileService = fileService;
        }
        public IActionResult Index()
        {
            List<Product> products = _context.Products.OrderByDescending(p => p.Id).ToList();

            ProductVM productList = new()
            {
                Products = products,
            };
            return View(productList);
        }
        public IActionResult Add()
        {
            var categories = _context.Categories.AsNoTracking().ToList();
            var brands = _context.Brands.AsNoTracking().ToList();
            var colors = _context.Colors.AsNoTracking().ToList();

            var model = new ProductAddVM
            {
                Categories = categories,
                Brands = brands,
                Colors = colors
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(ProductAddVM model)
        {
            if(!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }
               
                return View(model);
            }

            var newProduct = new Product
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                InStock = model.InStock
            };

            var foundCategory = _context.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
            var foundBrand = _context.Brands.FirstOrDefault(x => x.Id == model.BrandId);
            var foundColor = _context.Colors.FirstOrDefault(x => x.Id == model.ColorId);

            if (foundCategory == null || foundBrand == null || foundColor == null)
            {
                model.Categories = _context.Categories.AsNoTracking().ToList();
                model.Brands = _context.Brands.AsNoTracking().ToList();
                model.Colors = _context.Colors.AsNoTracking().ToList();
                return View(model);
            }

            newProduct.Category = foundCategory;
            newProduct.Brand = foundBrand;

            var imageUrls = _fileService.AddFile(model.Image, Path.Combine("img", "product-img"));

            newProduct.ProductImages = imageUrls.Select(imageUrl => new ProductImage
            {
                Image = new Image { ImageUrl = imageUrl }
            }).ToList();

            _context.Add(newProduct);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var productToDelete = _context.Products
                .Include(p => p.ProductImages)
                .ThenInclude(pi => pi.Image)
                .FirstOrDefault(p => p.Id == id);

            if (productToDelete is null) return NotFound();

            if (productToDelete.ProductImages != null)
            {
                foreach (var productImage in productToDelete.ProductImages)
                {
                    if (productImage.Image != null)
                    {
                        _fileService.DeleteFile(productImage.Image.ImageUrl, Path.Combine("img", "product-img"));
                    }
                }
            }

            _context.Products.Remove(productToDelete);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }

            Product product = _context.Products
                .Include(x => x.ProductColors).ThenInclude(x=>x.Color)
                .Include(x => x.ProductImages).ThenInclude(x => x.Image)
                .FirstOrDefault(x => x.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            List<Category> categories = _context.Categories.AsNoTracking().ToList();
            List<Brand> brands = _context.Brands.AsNoTracking().ToList();
            List<Color> colors = _context.Colors.AsNoTracking().ToList();

            List<string> currentImageUrls = product.ProductImages?.Select(pi => pi.Image.ImageUrl).ToList() ?? new List<string>();

            ProductUpdateVM updatedModel = new ProductUpdateVM()
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                InStock = product.InStock,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId,
                Categories = categories,
                Brands = brands,
                Colors = colors,
                ColorId = product.ProductColors.FirstOrDefault()?.ColorId,
                CurrentImage = currentImageUrls
            };

            if (updatedModel.Colors == null)
            {
                updatedModel.Colors = new List<Color>();
            }

            return View(updatedModel);
        }

    }
}
