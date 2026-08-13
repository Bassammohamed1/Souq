using ApplicationLayer.DTOs;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IWishingListService
    {
        Task<int> Add(int itemId, string itemType);
        Task<int> Remove(int itemId, string itemType);
        Task<int> TotalItemsInWishingList();
        Task<IEnumerable<WishingListDTO>> UserWishingList(int? page);
        Task<bool> HasUserLiked(string userID, int itemID, string itemType);
    }
}