using DomainLayer.Models.Wishing_List;
using Microsoft.AspNetCore.Identity;
using Souq.Models.Cart_Orders;

namespace DomainLayer.Models
{
    public class AppUser : IdentityUser
    {
        public List<Comment>? Comments { get; set; }
        public List<Rate>? Rates { get; set; }
        public WishingList? WishingList { get; set; }
        public ShoppingCart? ShoppingCart { get; set; }
    }
}
