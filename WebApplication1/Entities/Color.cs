namespace WebApplication1.Entities
{
    public class Color
    {
        public int Id { get; set; }
        public string? ColorName { get; set; }
        public string? Value { get; set; }
        public List<ProductColor>? ProductColors { get; set;}
    }
}
