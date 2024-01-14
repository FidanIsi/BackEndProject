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
                .Include(x => x.ProductImages).ThenInclude(x => x.Image)
                .Include(p => p.Category).Include(p => p.Brand)
                .Include(x => x.ProductColors).ThenInclude(x => x.Color)
                .FirstOrDefault(x => x.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            List<Category> categories = _context.Categories.ToList();
            List<Brand> brands = _context.Brands.ToList();
            List<Color> colors = _context.Colors.ToList();

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
                AllImages = product.ProductImages?.Select(p => new Image
                {
                    Id = p.Image.Id,
                    ImageUrl = p.Image.ImageUrl
                }).ToList() ?? new List<Image>()
            };

            updatedModel.Colors ??= new List<Color>();
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
                .Include(x => x.ProductImages).ThenInclude(x => x.Image)
                .Include(p => p.Category).Include(p => p.Brand)
                .Include(x => x.ProductColors).ThenInclude(x => x.Color)
                .FirstOrDefault(p => p.Id == editedProduct.Id);

            if (product is null) return NotFound();

            if (editedProduct.DeletedImageIds != null && editedProduct.DeletedImageIds.Any())
            {
                foreach (var deletedImageId in editedProduct.DeletedImageIds)
                {
                    Image imageToDelete = _context.Images.FirstOrDefault(i => i.Id == deletedImageId);
                    if (imageToDelete != null)
                    {
                        var imagePath = Path.Combine("img", "product-img", imageToDelete.ImageUrl);
                        _fileService.DeleteFile(imageToDelete.ImageUrl, imagePath);

                        _context.Images.Remove(imageToDelete);
                    }
                }

            }

            if (editedProduct.Images != null)
            {
                var newImageUrls = _fileService.AddFile(editedProduct.Images, Path.Combine("img", "product-img"));
                List<ProductImage> newImg = newImageUrls.Select(imageUrl => new ProductImage
                {
                    Image = new Image
                    {
                        ImageUrl = imageUrl
                    }
                }).ToList();

                product.ProductImages.AddRange(newImg);
            }

            var fCategory = _context.Categories.FirstOrDefault(x => x.Id == editedProduct.CategoryId);
            var fBrand = _context.Brands.FirstOrDefault(x => x.Id == editedProduct.BrandId);
            var fColor = _context.Colors.FirstOrDefault(x => x.Id == editedProduct.ColorId);
            product.Name = editedProduct.Name;
            product.Price = editedProduct.Price;
            product.Description = editedProduct.Description;
            product.InStock = editedProduct.InStock;
            product.CategoryId = editedProduct.CategoryId;
            product.BrandId = editedProduct.BrandId;
            product.Category = fCategory;
            product.Brand = fBrand;
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
