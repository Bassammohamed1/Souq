using DomainLayer.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Souq.Models.Cart_Orders
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser? User { get; set; }
        public List<CartDetails>? CartDetails { get; set; }
    }
}
