namespace WebApplication1.Entities
{
    public class Basket
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public List<BasketItem> BasketItems { get; set; } = new();
    }
}
