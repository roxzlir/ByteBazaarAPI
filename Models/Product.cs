using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteBazaarAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [StringLength(30)]
        public string Title { get; set; }
        [StringLength(250)]
        public string Description { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        [StringLength(250)]
        public string Image { get; set; }
        public int? FkCategoryId { get; set; }
    }
}
