using ByteBazaarAPI.Models;
using System.Reflection.PortableExecutable;

namespace ByteBazaarAPI.ViewModels
{
    public class ProductWithImageVM
    {
        public Product Product { get; set; }
        public string ImageURL { get; set; }
    }
}
