using DomainLayer.Enums;
using DomainLayer.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Souq.Models.Cart_Orders
{
    public class Order
    {
        public int ID { get; set; }
        [ForeignKey(nameof(UserID))]
        public string UserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PaymentMethod { get; set; }
        public OrderStatus? Status { get; set; }
        [NotMapped]
        public double TotalPrice { get; set; }
        public string? PromoCodeDiscountType { get; set; }
        public double? PromoCodeDiscountValue { get; set; }
        public AppUser? User { get; set; }
        public List<OrderDetails>? OrderDetails { get; set; }
    }
}
