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

            if (foundCategory == null || foundBrand == null)
            {
                model.Categories = _context.Categories.AsNoTracking().ToList();
                model.Brands = _context.Brands.AsNoTracking().ToList();
                return View(model);
            }

            newProduct.Category = foundCategory;
            newProduct.Brand = foundBrand;

            if (foundColor is null) return View(model);

            newProduct.ProductColors = new()
            {
                new ProductColor
                {
                    Color = foundColor
                }
            };

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
            if (id is null) return BadRequest();


            Product product = _context.Products
                .Include(p => p.Category).Include(p => p.Brand)
                .Include(x => x.ProductColors).ThenInclude(x => x.Color)
                .Include(x => x.ProductImages).ThenInclude(x => x.Image)
                .FirstOrDefault(x => x.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            List<Category> categories = _context.Categories.ToList();
            List<Brand> brands = _context.Brands.ToList();
            List<Color> colors = _context.Colors.ToList();

            List<string> currentImageUrls = product.ProductImages?
                .Select(pi => pi?.Image?.ImageUrl)
                .Where(url => url != null)
                .ToList() ?? new List<string>();
            ProductUpdateVM updatedModel = new()
            {
                Id = product.Id,
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

        [HttpPost]
        public IActionResult Update(ProductUpdateVM editedProduct)
        {
            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }

                return View(editedProduct);
            }

            Product product = _context.Products
                .Include(p => p.Category).Include(p => p.Brand)
                .Include(x => x.ProductColors).ThenInclude(x => x.Color)
                .Include(x => x.ProductImages).ThenInclude(x => x.Image)
                .FirstOrDefault(p => p.Id == editedProduct.Id);

            if (product is null)
                return NotFound();

            if (editedProduct.CurrentImage != null)
            {
                foreach (var currentImageUrl in product.ProductImages.Select(pi => pi.Image.ImageUrl).Except(editedProduct.CurrentImage))
                {
                    _fileService.DeleteFile(currentImageUrl, Path.Combine("img", "product-img"));
                    product.ProductImages.RemoveAll(pi => pi.Image.ImageUrl == currentImageUrl);
                }
            }

            if (editedProduct.Image != null && editedProduct.Image.Any())
            {
                foreach (var currentImageUrl in product.ProductImages.Select(pi => pi.Image.ImageUrl))
                {
                    _fileService.DeleteFile(currentImageUrl, Path.Combine("img", "product-img"));
                }

                var newImageUrls = _fileService.AddFile(editedProduct.Image, Path.Combine("img", "product-img"));
                product.ProductImages = newImageUrls.Select(imageUrl => new ProductImage
                {
                    Image = new Image
                    {
                        ImageUrl = imageUrl
                    }
                }).ToList();
            }

            var foundCategory = _context.Categories.FirstOrDefault(x => x.Id == editedProduct.CategoryId);
            var foundBrand = _context.Brands.FirstOrDefault(x => x.Id == editedProduct.BrandId);
            var foundColor = _context.Colors.FirstOrDefault(x => x.Id == editedProduct.ColorId);


            product.Name = editedProduct.Name;
            product.Price = (decimal)editedProduct.Price;
            product.Description = editedProduct.Description;
            product.InStock = editedProduct.InStock;
            product.CategoryId = editedProduct.CategoryId;
            product.BrandId = editedProduct.BrandId;
            product.Category = foundCategory;
            product.Brand = foundBrand;

            product.ProductColors = new List<ProductColor>
            {
                new ProductColor
                {
                    ColorId = (int)editedProduct.ColorId
                }
            };

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        
        public IActionResult Details(int? id)
        {
            if (id is null) return BadRequest();
            Product? product = _context.Products.Include(x => x.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages).ThenInclude(pi => pi.Image)
                .Include(p => p.ProductColors).ThenInclude(pi => pi.Color)
                .FirstOrDefault(x => x.Id == id);

            if (product is null) return NotFound();
            ViewBag.Color = product.ProductColors?.FirstOrDefault()?.Color?.Value;
            return View(product);
        }
        
    }
}
