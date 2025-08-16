namespace Souq.Models.Cart_Orders
{
    public class RepositoryCartVM
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Name { get; set; }
        public string imageSrc { get; set; }
    }
}
