using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteBazaarAPI.Models
{
    public class Campaign
    {
        [Key]
        public int CampaignId { get; set; }
        public string Title { get; set; }
        [Column(TypeName = "decimal(1,2)")]
        public decimal Percent { get; set; }
            
    }
}
