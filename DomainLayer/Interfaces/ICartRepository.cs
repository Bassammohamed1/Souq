using DomainLayer.Models.Cart;

namespace DomainLayer.Interfaces
{
    public interface ICartRepository
    {
        Task<int> AddItem(int itemId, int qty);
        Task<int> RemoveItem(int itemId);
        Task<ShoppingCart> GetUserCart();
        Task<int> GetCartItemCount(string userId = "");
        Task<bool> DoCheckout();
    }
}
