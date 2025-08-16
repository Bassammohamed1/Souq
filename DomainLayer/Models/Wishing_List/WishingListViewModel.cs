namespace DomainLayer.Models.Wishing_List
{
    public class WishingListViewModel
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; }
        public double Price { get; set; }
        public string Name { get; set; }
        public string imageSrc { get; set; }
        public int Quantity { get; set; }

    }
}