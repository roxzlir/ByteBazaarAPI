using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteBazaarAPI.Models
{
    public class ProductImage
    {
        [Key]
        public int ProductImageId { get; set; }
        [StringLength(250)]
        public string URL { get; set; }
        [Required]
        public int FkProductId { get; set; }

    }
}
