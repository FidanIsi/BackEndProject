using Newtonsoft.Json;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface ICartService
    {
        List<CartItem> GetCartItems();
        void AddToCart(CartItem item);
    }

    public class CartService : ICartService
    {
        private IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<CartItem> GetCartItems()
        {
            var cartJson = _httpContextAccessor.HttpContext.Request.Cookies["cart"];
            if (cartJson != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
            }

            return new List<CartItem>();
        }

        public void AddToCart(CartItem item)
        {
            var cart = GetCartItems();
            var existingItem = cart.FirstOrDefault(x => x.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }

            var cartJson = JsonConvert.SerializeObject(cart);

            _httpContextAccessor.HttpContext.Response.Cookies.Append("cart", cartJson, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddMonths(1), // Set the cookie expiration time
                HttpOnly = true
            });
        }
    }

}
