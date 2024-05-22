using ByteBazaarAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteBazaarAPI.DTO
{
    public class ProductWithImagesDTO
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int FkCategoryId { get; set; }
        public bool IsCampaign { get; set; } = false;
        public decimal CampaignPercent { get; set; } = 1;
        public decimal TempPrice { get; set; } = 0;
        public DateTime? CampaignStart { get; set; }
        public DateTime? CampaignEnd { get; set; }
        public Category Category { get; set; }
        public List<ProductImage> Images { get; set; }
    }
}
