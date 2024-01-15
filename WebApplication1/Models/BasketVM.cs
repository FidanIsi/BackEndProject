using WebApplication1.Entities;

namespace WebApplication1.Models
{
    public class BasketVM
    {
        public List<(BasketItem, Product)> Items { get; set; }

    }
}
