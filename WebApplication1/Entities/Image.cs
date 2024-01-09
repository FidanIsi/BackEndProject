namespace WebApplication1.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
    }
}
