using DomainLayer.Models;
using Souq.Models.Cart_Orders;

namespace DomainLayer.Interfaces
{
    public interface ICartRepository
    {
        Task<int> Add(int itemID, string itemType, int? qty);
        Task<int> Remove(int itemID, string itemType);
        Task<int> TotalItemsInCart();
        Task<CartViewModel> GetUserCart();
        Task<int> TotalItemQuantityInCart(int itemID, string itemType);
        Task EmptyCart();
        Task<CartViewModel> ApplyPromoCode(CartViewModel cart, Offer promoCodeOffer);
    }
}
