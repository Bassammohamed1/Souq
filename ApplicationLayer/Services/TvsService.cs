using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class TvsService : ITvsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public TvsService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<TV> GetTv(int id)
        {
            return await _unitOfWork.TVs.GetById(id);
        }

        public IEnumerable<TV> GetTvs(int pageNumber, int pageSize)
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

        public ItemDTO<TvDTO> GetTvsWithRelatedOnes()
        {
            var tvsCategories = _servicesInstanceProvider.GetItemsServiceInstance().GetItemCategories<TV>();

            var discountedTvs = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<TV>(1, 10, "ID", false).ToList().
                Select(t => new TvDTO
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
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "Tvs").Result,
                    CategoryName = t.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "Tvs").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var topRatedTvs = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<TV>(1, 10, "ID", false).ToList().
                Select(t => new TvDTO
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
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "Tvs").Result,
                    CategoryName = t.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "Tvs").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var latestTvs = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<TV>(1, 10, "ID", false).ToList().
                Select(t => new TvDTO
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
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "Tvs").Result,
                    CategoryName = t.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "Tvs").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            return new ItemDTO<TvDTO>()
            {
                ItemCategories = tvsCategories,
                DiscountedItems = (IEnumerable<TvDTO>)discountedTvs,
                latestItems = (IEnumerable<TvDTO>)latestTvs,
                TopRatedItems = (IEnumerable<TvDTO>)topRatedTvs,
            };
        }

        public async Task<ItemsDTO> GetBrandsTvs(string? orderIndex, int? page, string name, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<TV>("Brands", null, null, name) / (double)pageSize);

            var tvs = _servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<TV>(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(t => new TvDTO
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
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "Tvs").Result,
                     CategoryName = t.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "Tvs").Result.Count()
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

        public async Task<ItemsDTO> GetDiscountedTvs(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<TV>("Discounted") / (double)pageSize);

            var discountedTvs = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<TV>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(t => new TvDTO
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
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "Tvs").Result,
                     CategoryName = t.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "Tvs").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = discountedTvs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
        }

        public async Task<ItemsDTO> GetTopRatedTvs(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<TV>("Rated") / (double)pageSize);


            var ratedTvs = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<TV>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                  Select(t => new TvDTO
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
                      isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "Tvs").Result,
                      CategoryName = t.Category.Name,
                      RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "Tvs").Result.Count()
                  });

            return new ItemsDTO
            {
                Items = ratedTvs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
        }

        public async Task<ItemsDTO> GetLatestTvs(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<TV>("Latest") / (double)pageSize);

            var latestTvs = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<TV>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(t => new TvDTO
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
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "Tvs").Result,
                     CategoryName = t.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "Tvs").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = latestTvs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
        }

        public async Task<ItemsDTO> GetTvsWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<TV>("Price", price1, price2, null) / (double)pageSize);

            var priceTvs = _servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<TV>(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(t => new TvDTO
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
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "Tvs").Result,
                     CategoryName = t.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(t.ID, "Tvs").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = priceTvs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
        }

        public async Task<TvDTO> GetTvDetails(int id)
        {
            var tv = await this.GetTv(id);

            if (tv != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "Tvs", "Default");

                var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "Tvs")).Count();

                var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<TV>(id, "Tvs");

                var totalQuantity = await _servicesInstanceProvider.GetCartServiceInstance().TotalItemQuantityInCart(id, "Tvs");

                var similarPriceTvs = (await _unitOfWork.TVs.GetAll())
                    .Where(t => t.Price == tv.Price || Math.Abs(t.Price - tv.Price) <= 1000)
                    .Select(t => new TvDTO
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
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "Tvs").Result,
                        CategoryName = t.Category.Name,
                        RateCount = rateCount
                    });

                var relatedTvs = (await _unitOfWork.TVs.GetAll())
                    .Where(t => t.CategoryId == tv.CategoryId).Take(10)
                    .Select(t => new TvDTO
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
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "Tvs").Result,
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

                return new TvDTO
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
                    RelatedTVs = relatedTvs,
                    SimilarPriceTVs = similarPriceTvs,
                    Comments = comments,
                    Offers = offers,
                    BOGOGet = BOGOGetItem,
                    StarCounts = starCounts,
                    RateCount = rateCount,
                    ControllerName = "Tvs",
                    TotalQuantity = totalQuantity
                };
            }

            else
                return null;
        }

        public async Task<TvDTO> GetTvAllComments(int id)
        {
            var Tv = await this.GetTv(id);

            var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "Tvs")).Count();

            var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<TV>(id, "Tvs");

            if (Tv != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "Tvs", "All");

                if (comments.Any())
                {
                    return new TvDTO
                    {
                        Id = Tv.ID,
                        Name = Tv.Name,
                        Rate = Tv.Rate,
                        CategoryName = Tv.Category.Name,
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
