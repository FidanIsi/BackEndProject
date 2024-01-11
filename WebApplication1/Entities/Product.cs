using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool InStock { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
    }
}
