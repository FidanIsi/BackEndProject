using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
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

            List<(BasketItem, Product)> items = new();

            foreach (var basketItem in basket.BasketItems)
            {
                Product product = _context.Products?.Include(p => p.ProductImages).ThenInclude(p => p.Image).FirstOrDefault(x => x.Id == basketItem.ProductId)!;

                items.Add(new(basketItem, product));
            }
            var model = new BasketVM
            {
                Items = items
            };

            return View(model);
        }
    }
}
