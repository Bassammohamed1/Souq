using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Models.Cart
{
    public class CartDetail
    {
        public int Id { get; set; }
        [Required]
        public int ShoppingCartId { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public Item? Item { get; set; }
        public ShoppingCart? ShoppingCart { get; set; }
    }
}
