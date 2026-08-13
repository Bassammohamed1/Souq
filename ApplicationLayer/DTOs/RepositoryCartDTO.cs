
namespace ApplicationLayer.DTOs
{
    public class RepositoryCartDTO
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Name { get; set; }
        public string imageSrc { get; set; }
    }
}
