using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models.Wishing_List
{
    public class WishingListDetails
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemType { get; set; }
        public Item? Item { get; set; }
        public int WishingListId { get; set; }
        [ForeignKey(nameof(WishingListId))]
        public WishingList? WishingList { get; set; }
    }
}
