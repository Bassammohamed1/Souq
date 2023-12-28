using Microsoft.AspNetCore.Identity;

namespace Souq.Data.ViewModels
{
    public class Order
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string UserId { get; set; }
        public IdentityUser? User { get; set; }

        public List<OrderItem>? OrderItems { get; set; }
    }
}
