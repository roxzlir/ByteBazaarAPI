using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteBazaarAPI.Models
{
    public class CampaignIndex
    {
        [Key]
        public int CampaignIndexId { get; set; }
        [Required]
        [ForeignKey("Campaign")]
        public int FkCampaignId { get; set; }
        public Campaign? Campaign { get; set; }
        [Required]
        [ForeignKey("Product")]
        public int FkProductId { get; set; }
        public Product? Product { get; set; }
    }
}
