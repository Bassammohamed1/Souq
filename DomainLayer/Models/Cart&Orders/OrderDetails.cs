using DomainLayer.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Souq.Models.Cart_Orders
{
    public class OrderDetails
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        [ForeignKey(nameof(ItemID))]
        public int OrderID { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string ItemType { get; set; }
        public Order? Order { get; set; }
        public Item? Item { get; set; }
    }
}
