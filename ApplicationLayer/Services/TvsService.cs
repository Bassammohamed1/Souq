using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class TVsService : ITVsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public TVsService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<TV> GetTV(int id)
        {
            return await _unitOfWork.TVs.GetById(id);
        }

        public IEnumerable<TV> GetTVs(int pageNumber, int pageSize)
        {
            return _unitOfWork.TVs.GetAll(pageNumber, pageSize);
        }

        public async Task<Result> Add(TV tv)
        {
            var stream = new MemoryStream();
            await tv.clientFile.CopyToAsync(stream);
            tv.dbImage = stream.ToArray();

            var result = await _unitOfWork.TVs.Add(tv);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while adding." };
        }

        public async Task<Result> Update(TV tv)
        {
            var stream = new MemoryStream();
            await tv.clientFile.CopyToAsync(stream);
            tv.dbImage = stream.ToArray();

            var result = _unitOfWork.TVs.Update(tv);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while updating." };
        }

        public async Task<Result> Delete(TV tv)
        {
            var result = _unitOfWork.TVs.Delete(tv);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while deleting." };
        }

        public ItemDTO<TVDTO> GetTVsWithRelatedOnes()
        {
            var tvsCategories = _servicesInstanceProvider.GetItemsServiceInstance().GetItemCategories<TV>();

            var discountedTVs = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<TV>(1, 10, "ID", false).ToList().
                Select(t => new TVDTO
                {
                    Id = t.ID,
                    Name = t.Name,
                    Rate = t.Rate,
                    Price = t.Price,
                    NewPrice = t.NewPrice ?? 0,
                    imageSrc = t.imageSrc,
                    ConnectivityTechnology = t.ConnectivityTechnology,
                    DisplayTechnology = t.DisplayTechnology,
                    ItemDimensions = t.ItemDimensions,
                    RefreshRate = t.RefreshRate,
                    SpecialFeatures = t.SpecialFeatures,
                    Resolution = t.Resolution,
                    ScreenSize = t.ScreenSize,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "TVs").Result,
                    CategoryName = t.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "TVs").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var topRatedTVs = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<TV>(1, 10, "ID", false).ToList().
                Select(t => new TVDTO
                {
                    Id = t.ID,
                    Name = t.Name,
                    Rate = t.Rate,
                    Price = t.Price,
                    NewPrice = t.NewPrice ?? 0,
                    imageSrc = t.imageSrc,
                    ConnectivityTechnology = t.ConnectivityTechnology,
                    DisplayTechnology = t.DisplayTechnology,
                    ItemDimensions = t.ItemDimensions,
                    RefreshRate = t.RefreshRate,
                    SpecialFeatures = t.SpecialFeatures,
                    Resolution = t.Resolution,
                    ScreenSize = t.ScreenSize,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "TVs").Result,
                    CategoryName = t.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "TVs").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var latestTVs = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<TV>(1, 10, "ID", false).ToList().
                Select(t => new TVDTO
                {
                    Id = t.ID,
                    Name = t.Name,
                    Rate = t.Rate,
                    Price = t.Price,
                    NewPrice = t.NewPrice ?? 0,
                    imageSrc = t.imageSrc,
                    ConnectivityTechnology = t.ConnectivityTechnology,
                    DisplayTechnology = t.DisplayTechnology,
                    ItemDimensions = t.ItemDimensions,
                    RefreshRate = t.RefreshRate,
                    SpecialFeatures = t.SpecialFeatures,
                    Resolution = t.Resolution,
                    ScreenSize = t.ScreenSize,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "TVs").Result,
                    CategoryName = t.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "TVs").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            return new ItemDTO<TVDTO>()
            {
                ItemCategories = tvsCategories,
                DiscountedItems = (IEnumerable<TVDTO>)discountedTVs,
                latestItems = (IEnumerable<TVDTO>)latestTVs,
                TopRatedItems = (IEnumerable<TVDTO>)topRatedTVs,
            };
        }

        public async Task<ItemsDTO> GetBrandsTVs(string? orderIndex, int? page, string name, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<TV>("Brands", null, null, name) / (double)pageSize);

            var tvs = _servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<TV>(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(t => new TVDTO
                 {
                     Id = t.ID,
                     Name = t.Name,
                     Rate = t.Rate,
                     Price = t.Price,
                     NewPrice = t.NewPrice ?? 0,
                     imageSrc = t.imageSrc,
                     ConnectivityTechnology = t.ConnectivityTechnology,
                     DisplayTechnology = t.DisplayTechnology,
                     ItemDimensions = t.ItemDimensions,
                     RefreshRate = t.RefreshRate,
                     SpecialFeatures = t.SpecialFeatures,
                     Resolution = t.Resolution,
                     ScreenSize = t.ScreenSize,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "TVs").Result,
                     CategoryName = t.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "TVs").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = tvs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Brands",
                Brand = name
            };
        }

        public async Task<ItemsDTO> GetDiscountedTVs(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<TV>("Discounted") / (double)pageSize);

            var discountedTVs = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<TV>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(t => new TVDTO
                 {
                     Id = t.ID,
                     Name = t.Name,
                     Rate = t.Rate,
                     Price = t.Price,
                     NewPrice = t.NewPrice ?? 0,
                     imageSrc = t.imageSrc,
                     ConnectivityTechnology = t.ConnectivityTechnology,
                     DisplayTechnology = t.DisplayTechnology,
                     ItemDimensions = t.ItemDimensions,
                     RefreshRate = t.RefreshRate,
                     SpecialFeatures = t.SpecialFeatures,
                     Resolution = t.Resolution,
                     ScreenSize = t.ScreenSize,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "TVs").Result,
                     CategoryName = t.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "TVs").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = discountedTVs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
        }

        public async Task<ItemsDTO> GetTopRatedTVs(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<TV>("Rated") / (double)pageSize);


            var ratedTVs = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<TV>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                  Select(t => new TVDTO
                  {
                      Id = t.ID,
                      Name = t.Name,
                      Rate = t.Rate,
                      Price = t.Price,
                      NewPrice = t.NewPrice ?? 0,
                      imageSrc = t.imageSrc,
                      ConnectivityTechnology = t.ConnectivityTechnology,
                      DisplayTechnology = t.DisplayTechnology,
                      ItemDimensions = t.ItemDimensions,
                      RefreshRate = t.RefreshRate,
                      SpecialFeatures = t.SpecialFeatures,
                      Resolution = t.Resolution,
                      ScreenSize = t.ScreenSize,
                      isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "TVs").Result,
                      CategoryName = t.Category.Name,
                      RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "TVs").Result.Count()
                  });

            return new ItemsDTO
            {
                Items = ratedTVs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
        }

        public async Task<ItemsDTO> GetLatestTVs(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<TV>("Latest") / (double)pageSize);

            var latestTVs = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<TV>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(t => new TVDTO
                 {
                     Id = t.ID,
                     Name = t.Name,
                     Rate = t.Rate,
                     Price = t.Price,
                     NewPrice = t.NewPrice ?? 0,
                     imageSrc = t.imageSrc,
                     ConnectivityTechnology = t.ConnectivityTechnology,
                     DisplayTechnology = t.DisplayTechnology,
                     ItemDimensions = t.ItemDimensions,
                     RefreshRate = t.RefreshRate,
                     SpecialFeatures = t.SpecialFeatures,
                     Resolution = t.Resolution,
                     ScreenSize = t.ScreenSize,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "TVs").Result,
                     CategoryName = t.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "TVs").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = latestTVs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
        }

        public async Task<ItemsDTO> GetTVsWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<TV>("Price", price1, price2, null) / (double)pageSize);

            var priceTVs = _servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<TV>(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(t => new TVDTO
                 {
                     Id = t.ID,
                     Name = t.Name,
                     Rate = t.Rate,
                     Price = t.Price,
                     NewPrice = t.NewPrice ?? 0,
                     imageSrc = t.imageSrc,
                     ConnectivityTechnology = t.ConnectivityTechnology,
                     DisplayTechnology = t.DisplayTechnology,
                     ItemDimensions = t.ItemDimensions,
                     RefreshRate = t.RefreshRate,
                     SpecialFeatures = t.SpecialFeatures,
                     Resolution = t.Resolution,
                     ScreenSize = t.ScreenSize,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "TVs").Result,
                     CategoryName = t.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "TVs").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = priceTVs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
        }

        public async Task<TVDTO> GetTVDetails(int id)
        {
            var tv = await this.GetTV(id);

            if (tv != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "TVs", "Default");

                var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "TVs")).Count();

                var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<TV>(id, "TVs");

                var totalQuantity = await _servicesInstanceProvider.GetCartServiceInstance().TotalItemQuantityInCart(id, "TVs");

                var similarPriceTVs = (await _unitOfWork.TVs.GetAll())
                    .Where(t => t.Price == tv.Price || Math.Abs(t.Price - tv.Price) <= 1000)
                    .Select(t => new TVDTO
                    {
                        Id = t.ID,
                        Name = t.Name,
                        Rate = t.Rate,
                        Price = t.Price,
                        NewPrice = t.NewPrice ?? 0,
                        imageSrc = t.imageSrc,
                        ConnectivityTechnology = t.ConnectivityTechnology,
                        DisplayTechnology = t.DisplayTechnology,
                        ItemDimensions = t.ItemDimensions,
                        RefreshRate = t.RefreshRate,
                        SpecialFeatures = t.SpecialFeatures,
                        Resolution = t.Resolution,
                        ScreenSize = t.ScreenSize,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "TVs").Result,
                        CategoryName = t.Category.Name,
                        RateCount = rateCount
                    });

                var relatedTVs = (await _unitOfWork.TVs.GetAll())
                    .Where(t => t.CategoryId == tv.CategoryId).Take(10)
                    .Select(t => new TVDTO
                    {
                        Id = t.ID,
                        Name = t.Name,
                        Rate = t.Rate,
                        Price = t.Price,
                        NewPrice = t.NewPrice ?? 0,
                        imageSrc = t.imageSrc,
                        ConnectivityTechnology = t.ConnectivityTechnology,
                        DisplayTechnology = t.DisplayTechnology,
                        ItemDimensions = t.ItemDimensions,
                        RefreshRate = t.RefreshRate,
                        SpecialFeatures = t.SpecialFeatures,
                        Resolution = t.Resolution,
                        ScreenSize = t.ScreenSize,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "TVs").Result,
                        CategoryName = t.Category.Name,
                        RateCount = rateCount
                    });

                var offers = _servicesInstanceProvider.GetOffersServiceInstance().GetOffers("Electronics", tv.Category?.Name, tv.ID);

                var discountValue = string.Empty;

                if (offers.Any())
                    discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                              $"{offers.First().PercentDiscount}%" :
                              offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;

                var BOGOGetItem = await _servicesInstanceProvider.GetOffersServiceInstance().GetBOGOGetItem(tv);

                return new TVDTO
                {
                    Id = tv.ID,
                    Name = tv.Name,
                    Rate = tv.Rate,
                    Price = tv.Price,
                    NewPrice = tv.NewPrice ?? 0,
                    IsDiscounted = tv.IsDiscounted,
                    DiscountValue = discountValue,
                    IsBOGOBuy = tv.IsBOGOBuy,
                    IsBOGOGet = tv.IsBOGOGet,
                    imageSrc = tv.imageSrc,
                    ConnectivityTechnology = tv.ConnectivityTechnology,
                    DisplayTechnology = tv.DisplayTechnology,
                    ItemDimensions = tv.ItemDimensions,
                    RefreshRate = tv.RefreshRate,
                    SpecialFeatures = tv.SpecialFeatures,
                    Resolution = tv.Resolution,
                    ScreenSize = tv.ScreenSize,
                    CategoryName = tv.Category.Name,
                    RelatedTVs = relatedTVs,
                    SimilarPriceTVs = similarPriceTVs,
                    Comments = comments,
                    Offers = offers,
                    BOGOGet = BOGOGetItem,
                    StarCounts = starCounts,
                    RateCount = rateCount,
                    ControllerName = "TVs",
                    TotalQuantity = totalQuantity
                };
            }

            else
                return null;
        }

        public async Task<TVDTO> GetTVAllComments(int id)
        {
            var TV = await this.GetTV(id);

            var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "TVs")).Count();

            var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<TV>(id, "TVs");

            if (TV != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "TVs", "All");

                if (comments.Any())
                {
                    return new TVDTO
                    {
                        Id = TV.ID,
                        Name = TV.Name,
                        Rate = TV.Rate,
                        CategoryName = TV.Category.Name,
                        Comments = comments,
                        StarCounts = starCounts,
                        RateCount = rateCount
                    };
                }
                else
                    return null;
            }
            else
                return null;
        }

        public async Task<IEnumerable<Category>> GetSpecificCategoriesForSelectList()
        {
            return await _servicesInstanceProvider.GetCategoriesServiceInstance().GetSpecificCategories("Electronics");
        }
    }
}
