namespace WebApplication1.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        List<Checkout> Checkouts { get; set;}
    }
}
