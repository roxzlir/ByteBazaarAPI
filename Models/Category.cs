using System.ComponentModel.DataAnnotations;

namespace ByteBazaarAPI.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [StringLength(50)]
        public string Title { get; set; }
    }
}
