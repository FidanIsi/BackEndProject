using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WebApplication1.Entities;

namespace WebApplication1.Models
{
    public class CheckoutVM
    {
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
        [ValidateNever]
        public List<Country> Countries { get; set; }
        public Country Country { get; set; }  
    }
}
