using DomainLayer.Interfaces;
using DomainLayer.Models.Wishing_List;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace InfrastructureLayer.Repository
{
    public class WishListRepository : IWishListRepository
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;

        public WishListRepository(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public bool HasUserLiked(string userID, int itemID, string itemType)
        {
            var userWishingList = _context.WishingLists.AsNoTracking().Where(w => w.UserId == userID)
                .FirstOrDefault();

            if (userWishingList is not null)
            {
                var userWishingListDetails = _context.WishingListsDetails.AsNoTracking()
                    .Where(wl => wl.WishingListId == userWishingList.Id && wl.ItemId == itemID && wl.ItemType == itemType);

                if (userWishingListDetails.Any())
                    return true;
                else
                    return false;
            }

            var userWishList = new WishingList
            {
                UserId = userID
            };
            _context.WishingLists.Add(userWishList);
            _context.SaveChanges();

            return false;
        }

        public async Task<int> Add(int itemId, string itemType)
        {
            var userId = await _userService.GetUserId();

            if (!string.IsNullOrEmpty(userId))
            {
                var userWishingList = await _context.WishingLists.AsNoTracking().Where(w => w.UserId == userId)
                .FirstOrDefaultAsync();

                if (userWishingList is null)
                {
                    var transaction = _context.Database.BeginTransaction();
                    try
                    {
                        var wishingList = new WishingList
                        {
                            UserId = userId
                        };
                        await _context.WishingLists.AddAsync(wishingList);
                        await _context.SaveChangesAsync();

                        var wishingListDetails = new WishingListDetails
                        {
                            ItemId = itemId,
                            ItemType = itemType,
                            WishingListId = wishingList.Id
                        };
                        await _context.WishingListsDetails.AddAsync(wishingListDetails);
                        await _context.SaveChangesAsync();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception(ex.Message);
                    }
                }
                else
                {
                    var wishingListDetails = new WishingListDetails
                    {
                        ItemId = itemId,
                        ItemType = itemType,
                        WishingListId = userWishingList.Id
                    };
                    await _context.WishingListsDetails.AddAsync(wishingListDetails);
                    await _context.SaveChangesAsync();
                }
                return await TotalItemsInWishingList();
            }
            throw new InvalidOperationException();
        }

        public async Task<int> Remove(int itemId, string itemType)
        {
            var userId = await _userService.GetUserId();

            if (!string.IsNullOrEmpty(userId))
            {
                var userWishingList = await _context.WishingLists.AsNoTracking()
                    .Where(w => w.UserId == userId)
                    .FirstOrDefaultAsync();

                var wishingListDetails = await _context.WishingListsDetails.AsNoTracking()
                    .Where(w => w.ItemId == itemId && w.ItemType == itemType)
                    .FirstOrDefaultAsync();

                if (userWishingList is not null && wishingListDetails is not null)
                {
                    _context.WishingListsDetails.Remove(wishingListDetails);
                    await _context.SaveChangesAsync();

                    return await TotalItemsInWishingList();
                }
                else
                    throw new InvalidOperationException();
            }
            throw new InvalidOperationException();
        }

        public async Task<int> TotalItemsInWishingList()
        {
            var userId = await _userService.GetUserId();

            var userWishingList = await _context.WishingLists.AsNoTracking()
                .Where(w => w.UserId == userId).FirstOrDefaultAsync();

            if (userWishingList is not null)
            {
                var totalItemsCount = _context.WishingListsDetails.AsNoTracking().Where(w => w.WishingList.UserId == userId).Count();

                return totalItemsCount;
            }

            throw new InvalidOperationException();
        }

        public async Task<IEnumerable<WishingListViewModel>> UserWishingList(int pageNumber, int pageSize)
        {
            var userId = await _userService.GetUserId();

            var wishList = _context.WishingLists.AsNoTracking().AsSplitQuery()
                .Include(w => w.WishingListDetails)
                .FirstOrDefault(w => w.UserId == userId);

            if (wishList is not null)
            {
                var viewModel = wishList?.WishingListDetails?
                    .Select(wl =>
                    {
                        string? name = null;
                        string? imageSrc = null;
                        double? price = null;

                        switch (wl.ItemType)
                        {
                            case "AirConditioners":
                                var airConditioner = _context.AirConditioners.AsNoTracking().FirstOrDefault(a => a.ID == wl.ItemId);
                                name = airConditioner?.Name;
                                imageSrc = airConditioner?.imageSrc;
                                price = airConditioner?.Price;
                                break;

                            case "Fridges":
                                var fridge = _context.Fridges.AsNoTracking().FirstOrDefault(f => f.ID == wl.ItemId);
                                name = fridge?.Name;
                                imageSrc = fridge?.imageSrc;
                                price = fridge?.Price;
                                break;

                            case "Cookers":
                                var cooker = _context.Cookers.AsNoTracking().FirstOrDefault(c => c.ID == wl.ItemId);
                                name = cooker?.Name;
                                imageSrc = cooker?.imageSrc;
                                price = cooker?.Price;
                                break;

                            case "WashingMachines":
                                var washingMachine = _context.WashingMachines.AsNoTracking().FirstOrDefault(wm => wm.ID == wl.ItemId);
                                name = washingMachine?.Name;
                                imageSrc = washingMachine?.imageSrc;
                                price = washingMachine?.Price;
                                break;

                            case "Laptops":
                                var laptop = _context.Laptops.AsNoTracking().FirstOrDefault(l => l.ID == wl.ItemId);
                                name = laptop?.Name;
                                imageSrc = laptop?.imageSrc;
                                price = laptop?.Price;
                                break;

                            case "TVs":
                                var tv = _context.TVs.AsNoTracking().FirstOrDefault(t => t.ID == wl.ItemId);
                                name = tv?.Name;
                                imageSrc = tv?.imageSrc;
                                price = tv?.Price;
                                break;

                            case "HeadPhones":
                                var headPhone = _context.HeadPhones.AsNoTracking().FirstOrDefault(hp => hp.ID == wl.ItemId);
                                name = headPhone?.Name;
                                imageSrc = headPhone?.imageSrc;
                                price = headPhone?.Price;
                                break;

                            case "MobilePhones":
                                var mobilePhone = _context.MobilePhones.AsNoTracking().FirstOrDefault(mp => mp.ID == wl.ItemId);
                                name = mobilePhone?.Name;
                                imageSrc = mobilePhone?.imageSrc;
                                price = mobilePhone?.Price;
                                break;

                            case "VideoGames":
                                var videoGame = _context.VideoGames.AsNoTracking().FirstOrDefault(vg => vg.ID == wl.ItemId);
                                name = videoGame?.Name;
                                imageSrc = videoGame?.imageSrc;
                                price = videoGame?.Price;
                                break;
                        }

                        return new WishingListViewModel
                        {
                            ItemId = wl.ItemId,
                            ItemType = wl.ItemType,
                            Name = name,
                            Price = price ?? 0.0,
                            imageSrc = imageSrc
                        };
                    }).ToPagedList(pageNumber, pageSize);

                return viewModel ?? Enumerable.Empty<WishingListViewModel>();
            }

            var userWishList = new WishingList
            {
                UserId = userId
            };
            await _context.WishingLists.AddAsync(userWishList);
            await _context.SaveChangesAsync();

            return Enumerable.Empty<WishingListViewModel>().ToPagedList();
        }
    }
}