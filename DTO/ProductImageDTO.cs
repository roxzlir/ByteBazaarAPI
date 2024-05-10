using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ByteBazaarAPI.DTO
{
    public class ProductImageDTO
    {
        public string URL { get; set; }
        public int FkProductId { get; set; }
    }
}
