namespace WebApplication1.Entities
{
    public class Checkout
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Comment { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}
