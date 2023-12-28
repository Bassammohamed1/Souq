using Souq.Models;

namespace Souq.Data.ViewModels
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int Amount { get; set; }
        public double Price { get; set; }

        public int itemId { get; set; }
        public BaseModel? Item { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
