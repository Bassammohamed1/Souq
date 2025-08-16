using DomainLayer.Models.Wishing_List;

namespace DomainLayer.Interfaces
{
    public interface IWishListRepository
    {
        Task<int> Add(int itemId, string itemType);
        Task<int> Remove(int itemId, string itemType);
        Task<int> TotalItemsInWishingList();
        Task<IEnumerable<WishingListViewModel>> UserWishingList(int pageNumber, int pageSize);
        bool HasUserLiked(string userID, int itemID, string itemType);
    }
}
