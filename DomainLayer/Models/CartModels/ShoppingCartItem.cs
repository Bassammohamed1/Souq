using DomainLayer.Models;

namespace DomainLayer.Models.CartModels
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public BaseModel? Item { get; set; }
        public int Amount { get; set; }
        public string ShoppingCartId { get; set; }
    }
}
