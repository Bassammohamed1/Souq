using DomainLayer.Enums;

namespace ApplicationLayer.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public double TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public OrderStatus? Status { get; set; }
        public string CreatedAt { get; set; }
    }
}
