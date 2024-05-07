using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Models.Cart
{
    public class OrderDetail
    {
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double UnitPrice { get; set; }
        public Order? Order { get; set; }
        public Item? Item { get; set; }
    }
}
