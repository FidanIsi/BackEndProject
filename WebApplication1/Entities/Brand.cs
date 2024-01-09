namespace WebApplication1.Entities
{
    public class Brand
    {
        public int Id { get; set; }
        public string? BrandName { get; set; }
        public List<Product>? Products { get; set; }
    }
}
