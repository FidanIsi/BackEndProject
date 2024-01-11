using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WebApplication1.Entities;

namespace WebApplication1.Areas.Admin.Models
{
    public class ProductUpdateVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool InStock { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        [ValidateNever]
        public List<Category> Categories { get; set; }
        [ValidateNever]
        public List<Brand> Brands { get; set; }
        [ValidateNever]
        public int? ColorId { get; set; }
        [ValidateNever]
        public List<Color>? Colors { get; set; }
        public List<IFormFile> Image { get; set; }
        public List<string> CurrentImage { get; set; }
    }
}
