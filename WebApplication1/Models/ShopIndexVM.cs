using WebApplication1.Entities;

namespace WebApplication1.Models
{
    public class ShopIndexVM
    {
        public List<Product>? Products { get; set; }
        public Product Product { get; set; }
        public int TotalPageCount { get; set; }
        public int CurrentPage { get; set; }
        public string Order { get; set; }
    }
}
