﻿using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1, string order = "desc")
        {
            if (page <= 0) page = 1;

            int productsPerPage = 2; 
            var productCount = await _context.Products.CountAsync();

            int totalPageCount = (int)Math.Ceiling(((decimal)productCount / productsPerPage));

            var productsQuery = order switch
            {
                "desc" => _context.Products.OrderByDescending(x => x.Id),
                "asc" => _context.Products.OrderBy(x => x.Id),
                _ => _context.Products.OrderByDescending(x => x.Id)
            };

            var pagedProducts = await productsQuery
            .Skip((page - 1) * productsPerPage)
            .Take(productsPerPage)
            .Include(p => p.ProductImages).ThenInclude(pi => pi.Image)
            .ToListAsync();

            foreach (var product in pagedProducts)
            {
                product.ProductImages = product.ProductImages.ToList(); // Ensure the images are loaded
            }

            ViewBag.Order = order;
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.Color = _context.Colors.ToList();

            var model = new ShopIndexVM
            {
                Products = pagedProducts,
                TotalPageCount = totalPageCount,
                CurrentPage = page,
            };

            return View(model);
        }


        public IActionResult Filter(int page, string order = "desc")
        {
            if (page <= 0) page = 1;

            int productsPerPage = 2; // Change this to 2 for 2 products per page
            var productCount = _context.Products.Count();

            int totalPageCount = (int)Math.Ceiling(((decimal)productCount / productsPerPage));

            var products = order switch
            {
                "desc" => _context.Products.OrderByDescending(x => x.Id),
                "asc" => _context.Products.OrderBy(x => x.Id),
                _ => _context.Products.OrderByDescending(x => x.Id)
            };

            var pagedProducts = products
                .Skip((page - 1) * productsPerPage)
                .Take(productsPerPage)
                .ToList();

            var model = new ShopIndexVM
            {
                Products = pagedProducts,
                TotalPageCount = totalPageCount,
                CurrentPage = page,
                Order = order
            };

            return PartialView("_ShopPartial", model);
        }

        public IActionResult Sorted(int titleid, int brandid, int colorid, int page = 1, string order = "desc")
        {
            int productsPerPage = 2; // Change this to 2 for 2 products per page

            IQueryable<Product> allProducts = _context.Products
                .Include(p => p.ProductImages).ThenInclude(pi => pi.Image)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color);

            if (page <= 0) page = 1;

            if (titleid == 0 && brandid == 0 && colorid == 0)
            {
                var products = allProducts
                    .Skip((page - 1) * productsPerPage)
                    .Take(productsPerPage)
                    .ToList();

                var model = new ShopIndexVM
                {
                    Products = products,
                };

                return PartialView("_ShopPartial", model);
            }

            var sortedProducts = allProducts
            .Where(x => (titleid == 0 || x.CategoryId == titleid)
                        && (brandid == 0 || x.BrandId == brandid)
                        && (colorid == 0 || x.ProductColors.FirstOrDefault().ColorId == colorid))
            .OrderBy(x => order == "asc" ? x.Id : -x.Id) // Apply ascending or descending order
            .Skip((page - 1) * productsPerPage)
            .Take(productsPerPage)
            .Include(p => p.ProductImages).ThenInclude(pi => pi.Image)
            .ToList();

            var totalPageCount = (int)Math.Ceiling((double)allProducts.Count() / productsPerPage);

            var sortedModel = new ShopIndexVM
            {
                Products = sortedProducts,
                TotalPageCount = totalPageCount,
                CurrentPage = page
            };

            return PartialView("_ShopPartial", sortedModel);
        }

        public IActionResult Search(string? input)
        {
            var products = input == null ? new List<Product>()
                : _context.Products
                    .Where(x => x.Name.ToLower().StartsWith(input.ToLower()))
                    .ToList();

            return ViewComponent("SearchResult", products);
        }

        public IActionResult ProductDetail(int? id)
        {
            if (id is null) return NotFound();

            Product? product = _context.Products
                .Include(p => p.ProductImages).ThenInclude(pi => pi.Image)
                .FirstOrDefault(x => x.Id == id);

            if (product is null) return NotFound();

            ShopIndexVM model = new()
            {
                Product = product,
            };

            return View(model);
        }
       
        
        #region For Basket
        public IActionResult AddToBasket(int? id)
        {
            if (id is null) return NotFound();

            var foundProduct = _context.Products.FirstOrDefault(x => x.Id == id);

            if (foundProduct is null) return NotFound();

            Request.Cookies.TryGetValue("basket", out var basketSerialized);

            Basket basket = null!;
            if (basketSerialized is null)
            {
                basket = new Basket();
            }
            else
            {
                basket = JsonSerializer.Deserialize<Basket>(basketSerialized)!;
            }


            var foundBasketItem = basket.BasketItems.FirstOrDefault(x => x.ProductId == foundProduct.Id);

            if (foundBasketItem is null)
            {
                foundBasketItem = new BasketItem
                {
                    ProductId = foundProduct.Id,
                    Count = 1
                };

                basket.BasketItems.Add(foundBasketItem);
            }
            else
            {
                foundBasketItem.Count++;
            }

            Response.Cookies.Append("basket", JsonSerializer.Serialize(basket));

            return RedirectToAction("Index", "Basket");
        }

        public IActionResult DeleteFromBasket(int? id)
        {
            if (id is null) return NotFound();

            Request.Cookies.TryGetValue("basket", out var basketSerialized);

            if (basketSerialized is null) return RedirectToAction("Index", "Home");

            var basket = JsonSerializer.Deserialize<Basket>(basketSerialized)!;

            var foundBasketItem = basket.BasketItems.FirstOrDefault(x => x.ProductId == id);

            if (foundBasketItem is null) return NotFound();

            basket.BasketItems.Remove(foundBasketItem);

            Response.Cookies.Append("basket", JsonSerializer.Serialize(basket));

            return RedirectToAction("Index", "Basket");
        }
        #endregion

    }
}
