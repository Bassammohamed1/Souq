using ApplicationLayer.DTOs;
using X.PagedList;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface ICartService
    {
        Task<int> Add(int itemID, string itemType, int? qty);
        Task<int> Remove(int itemID, string itemType);
        Task<int> TotalItemsInCart();
        Task<CartDTO> GetUserCart();
        Task<IPagedList<RepositoryCartDTO>> GetCartItems();
        Task<int> TotalItemQuantityInCart(int itemID, string itemType);
        Task EmptyCart();
        Task<ApplyPromoCodeResultDTO> ApplyPromoCode(string promoCode);
    }
}
