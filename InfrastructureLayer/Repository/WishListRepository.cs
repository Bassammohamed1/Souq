using DomainLayer.Interfaces;
using DomainLayer.Models.Wishing_List;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repository
{
    public class WishListRepository : Repository<WishingList>, IWishListRepository
    {
        private readonly AppDbContext _context;

        public WishListRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<WishingList> GetUserWishingList(string userID)
        {
            return await _context.WishingLists.AsSplitQuery()
                .Include(cd => cd.WishingListDetails)
                .FirstOrDefaultAsync(c => c.UserId == userID);
        }

        public IQueryable<WishingListDetails> GetUserWishingListDetails(string userID)
        {
            return _context.WishingListsDetails
               .Where(cd => cd.WishingList.UserId == userID);
        }

        public async Task<WishingListDetails> GetUserWishingListDetails(int wishingListID, int itemID, string itemType)
        {
            return await _context.WishingListsDetails
               .FirstOrDefaultAsync(cd => cd.WishingListId == wishingListID && cd.ItemId == itemID && cd.ItemType == itemType);
        }

        public async Task<WishingListDetails> AddWishingListDetails(WishingListDetails wishingList)
        {
            await _context.WishingListsDetails.AddAsync(wishingList);

            return wishingList;
        }

        public WishingListDetails RemoveWishingListDetails(WishingListDetails wishingList)
        {
            _context.WishingListsDetails.Remove(wishingList);

            return wishingList;
        }

        public List<WishingListDetails> RemoveWishingListDetails(List<WishingListDetails> wishingLists)
        {
            _context.WishingListsDetails.RemoveRange(wishingLists);

            return wishingLists;
        }
    }
}