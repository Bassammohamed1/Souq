using Souq.Models;

namespace Souq.Data.ViewModels
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public BaseModel? Item { get; set; }
        public int Amount { get; set; }
        public string ShoppingCartId { get; set; }
    }
}
