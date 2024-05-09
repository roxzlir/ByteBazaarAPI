namespace ByteBazaarAPI.DTO
{
    public class ProductWithImagesDTO
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public List<string> Images { get; set; }
    }
}
