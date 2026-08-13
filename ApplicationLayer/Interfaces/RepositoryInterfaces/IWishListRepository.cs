using DomainLayer.Models.Wishing_List;

namespace DomainLayer.Interfaces
{
    public interface IWishListRepository : IRepository<WishingList>
    {
        Task<WishingList> GetUserWishingList(string userID);
        IQueryable<WishingListDetails> GetUserWishingListDetails(string userID);
        Task<WishingListDetails> GetUserWishingListDetails(int wishingListID, int itemID, string itemType);
        Task<WishingListDetails> AddWishingListDetails(WishingListDetails wishingList);
        WishingListDetails RemoveWishingListDetails(WishingListDetails wishingList);
        List<WishingListDetails> RemoveWishingListDetails(List<WishingListDetails> wishingLists);
    }
}
