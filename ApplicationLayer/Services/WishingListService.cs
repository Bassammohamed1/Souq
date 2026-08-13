using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Interfaces;
using DomainLayer.Models.Wishing_List;
using X.PagedList.Extensions;

namespace ApplicationLayer.Services
{
    public class WishingListService : IWishingListService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public WishingListService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<int> Add(int itemId, string itemType)
        {
            var userId = _userService.GetUserId();

            if (!string.IsNullOrEmpty(userId))
            {
                var userWishingList = await _unitOfWork.WishLists.GetUserWishingList(userId);

                if (userWishingList is null)
                {
                    var wishingList = new WishingList
                    {
                        UserId = userId
                    };
                    await _unitOfWork.WishLists.Add(wishingList);

                    var wishingListDetails = new WishingListDetails
                    {
                        ItemId = itemId,
                        ItemType = itemType,
                        WishingListId = wishingList.Id
                    };
                    await _unitOfWork.WishLists.AddWishingListDetails(wishingListDetails);


                    await _unitOfWork.Commit();
                }
                else
                {
                    var wishingListDetails = new WishingListDetails
                    {
                        ItemId = itemId,
                        ItemType = itemType,
                        WishingListId = userWishingList.Id
                    };
                    await _unitOfWork.WishLists.AddWishingListDetails(wishingListDetails);
                    await _unitOfWork.Commit();
                }

                return await TotalItemsInWishingList();
            }

            throw new InvalidOperationException();
        }

        public async Task<int> Remove(int itemId, string itemType)
        {
            var userId = _userService.GetUserId();

            if (!string.IsNullOrEmpty(userId))
            {
                var userWishingList = await _unitOfWork.WishLists.GetUserWishingList(userId);

                var wishingListDetails = await _unitOfWork.WishLists.GetUserWishingListDetails(userWishingList.Id, itemId, itemType);

                if (userWishingList is not null && wishingListDetails is not null)
                {
                    _unitOfWork.WishLists.RemoveWishingListDetails(wishingListDetails);
                    await _unitOfWork.Commit();

                    return await TotalItemsInWishingList();
                }
                else
                    throw new InvalidOperationException();
            }

            throw new InvalidOperationException();
        }

        public async Task<int> TotalItemsInWishingList()
        {
            var userId = _userService.GetUserId();

            var userWishingList = await _unitOfWork.WishLists.GetUserWishingList(userId);

            if (userWishingList is not null)
            {
                var totalItemsCount = _unitOfWork.WishLists.GetUserWishingListDetails(userId).Count();

                return totalItemsCount;
            }

            throw new InvalidOperationException();
        }

        public async Task<IEnumerable<WishingListDTO>> UserWishingList(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var userId = _userService.GetUserId();

            var userWishList = await _unitOfWork.WishLists.GetUserWishingList(userId);

            if (userWishList is not null)
            {
                var wishList = userWishList?.WishingListDetails?
                    .Select(wl =>
                    {
                        string? name = null;
                        string? imageSrc = null;
                        double? price = null;

                        switch (wl.ItemType)
                        {
                            case "AirConditioners":
                                var airConditioner = _unitOfWork.AirConditioners.GetById(wl.ItemId).Result;
                                name = airConditioner?.Name;
                                imageSrc = airConditioner?.imageSrc;
                                price = airConditioner?.Price;
                                break;

                            case "Fridges":
                                var fridge = _unitOfWork.Fridges.GetById(wl.ItemId).Result;
                                name = fridge?.Name;
                                imageSrc = fridge?.imageSrc;
                                price = fridge?.Price;
                                break;

                            case "Cookers":
                                var cooker = _unitOfWork.Cookers.GetById(wl.ItemId).Result;
                                name = cooker?.Name;
                                imageSrc = cooker?.imageSrc;
                                price = cooker?.Price;
                                break;

                            case "WashingMachines":
                                var washingMachine = _unitOfWork.WashingMachines.GetById(wl.ItemId).Result;
                                name = washingMachine?.Name;
                                imageSrc = washingMachine?.imageSrc;
                                price = washingMachine?.Price;
                                break;

                            case "Laptops":
                                var laptop = _unitOfWork.Laptops.GetById(wl.ItemId).Result;
                                name = laptop?.Name;
                                imageSrc = laptop?.imageSrc;
                                price = laptop?.Price;
                                break;

                            case "TVs":
                                var tv = _unitOfWork.TVs.GetById(wl.ItemId).Result;
                                name = tv?.Name;
                                imageSrc = tv?.imageSrc;
                                price = tv?.Price;
                                break;

                            case "HeadPhones":
                                var headPhone = _unitOfWork.HeadPhones.GetById(wl.ItemId).Result;
                                name = headPhone?.Name;
                                imageSrc = headPhone?.imageSrc;
                                price = headPhone?.Price;
                                break;

                            case "MobilePhones":
                                var mobilePhone = _unitOfWork.MobilePhones.GetById(wl.ItemId).Result;
                                name = mobilePhone?.Name;
                                imageSrc = mobilePhone?.imageSrc;
                                price = mobilePhone?.Price;
                                break;

                            case "VideoGames":
                                var videoGame = _unitOfWork.VideoGames.GetById(wl.ItemId).Result;
                                name = videoGame?.Name;
                                imageSrc = videoGame?.imageSrc;
                                price = videoGame?.Price;
                                break;
                        }

                        return new WishingListDTO
                        {
                            ItemId = wl.ItemId,
                            ItemType = wl.ItemType,
                            Name = name,
                            Price = price ?? 0.0,
                            imageSrc = imageSrc
                        };
                    }).ToPagedList(pageNumber, pageSize);

                foreach (var wish in wishList)
                {
                    wish.Quantity = await _servicesInstanceProvider.GetCartServiceInstance().TotalItemQuantityInCart(wish.ItemId, wish.ItemType);
                    await _unitOfWork.Commit();
                }

                return wishList ?? Enumerable.Empty<WishingListDTO>();
            }

            var newUserWishList = new WishingList
            {
                UserId = userId
            };

            await _unitOfWork.WishLists.Add(newUserWishList);
            await _unitOfWork.Commit();

            return Enumerable.Empty<WishingListDTO>().ToPagedList();
        }

        public async Task<bool> HasUserLiked(string userID, int itemID, string itemType)
        {
            var userWishingList = await _unitOfWork.WishLists.GetUserWishingList(userID);

            if (userWishingList is not null)
            {
                var userWishingListDetails = await _unitOfWork.WishLists.GetUserWishingListDetails(userWishingList.Id, itemID, itemType);

                return userWishingListDetails is not null ? true : false;
            }

            var userWishList = new WishingList
            {
                UserId = userID
            };
            await _unitOfWork.WishLists.Add(userWishList);
            await _unitOfWork.Commit();

            return false;
        }
    }
}
