using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
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
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        [ValidateNever]
        public List<Category>? Categories { get; set; }
        [ValidateNever]
        public List<Brand>? Brands { get; set; }
        [ValidateNever]
        public int? ColorId { get; set; }
        [ValidateNever]
        public List<Color>? Colors { get; set; }
        [NotMapped]
        public List<int>? ImagesId { get; set; }
        [NotMapped]
        public List<Image>? AllImages { get; set; }
        [NotMapped]
        public List<IFormFile>? Images { get; set; }

        [NotMapped]
        public List<int>? DeletedImageIds { get; set; }
    }
}
