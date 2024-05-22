using System.ComponentModel.DataAnnotations.Schema;

namespace ByteBazaarAPI.DTO
{
    public class ProductDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int FkCategoryId { get; set; }
        public int Quantity { get; set; }
        public string ImageURL { get; set; }
        public bool IsCampaign { get; set; } = false;
        public decimal CampaignPercent { get; set; } = 1;
        public decimal TempPrice { get; set; } = 0;
        public DateTime? CampaignStart { get; set; }
        public DateTime? CampaignEnd { get; set; }
    }
}
