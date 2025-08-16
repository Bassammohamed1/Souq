using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models.Wishing_List
{
    public class WishingList
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser? User { get; set; }
        public List<WishingListDetails>? WishingListDetails { get; set; }
    }
}
