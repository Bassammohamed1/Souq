using DomainLayer.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Souq.Models.Cart_Orders
{
    public class CartDetails
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public int ItemID { get; set; }
        public string ItemType { get; set; }
        public Item? Item { get; set; }
        public int ShoppingCartID { get; set; }
        [ForeignKey(nameof(ShoppingCartID))]
        public ShoppingCart? ShoppingCart { get; set; }
    }
}
