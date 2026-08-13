using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class VideoGamesService : IVideoGamesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public VideoGamesService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<VideoGame> GetVideoGame(int id)
        {
            return await _unitOfWork.VideoGames.GetById(id);
        }

        public IEnumerable<VideoGame> GetVideoGames(int pageNumber, int pageSize)
        {
            return _unitOfWork.VideoGames.GetAll(pageNumber, pageSize);
        }

        public async Task<Result> Add(VideoGame videoGame)
        {
            var stream = new MemoryStream();
            await videoGame.clientFile.CopyToAsync(stream);
            videoGame.dbImage = stream.ToArray();

            var result = await _unitOfWork.VideoGames.Add(videoGame);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while adding." };
        }

        public async Task<Result> Update(VideoGame videoGame)
        {
            var stream = new MemoryStream();
            await videoGame.clientFile.CopyToAsync(stream);
            videoGame.dbImage = stream.ToArray();

            var result = _unitOfWork.VideoGames.Update(videoGame);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while updating." };
        }

        public async Task<Result> Delete(VideoGame videoGame)
        {
            var result = _unitOfWork.VideoGames.Delete(videoGame);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while deleting." };
        }

        public ItemDTO<VideoGameDTO> GetVideoGamesWithRelatedOnes()
        {
            var videoGamesCategories = _servicesInstanceProvider.GetItemsServiceInstance().GetItemCategories<VideoGame>();

            var discountedVideoGames = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<VideoGame>(1, 10, "ID", false).ToList().
                Select(c => new VideoGameDTO
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "VideoGames").Result,
                    CategoryName = c.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "VideoGames").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var topRatedVideoGames = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<VideoGame>(1, 10, "ID", false).ToList().
                Select(c => new VideoGameDTO
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "VideoGames").Result,
                    CategoryName = c.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "VideoGames").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var latestVideoGames = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<VideoGame>(1, 10, "ID", false).ToList().
                Select(c => new VideoGameDTO
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "VideoGames").Result,
                    CategoryName = c.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "VideoGames").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            return new ItemDTO<VideoGameDTO>()
            {
                ItemCategories = videoGamesCategories,
                DiscountedItems = discountedVideoGames,
                latestItems = latestVideoGames,
                TopRatedItems = topRatedVideoGames
            };
        }

        public async Task<ItemsDTO> GetBrandsVideoGames(string? orderIndex, int? page, string name, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<VideoGame>("Brands", null, null, name) / (double)pageSize);

            var videoGames = _servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<VideoGame>(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(c => new VideoGameDTO
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "VideoGames").Result,
                    CategoryName = c.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "VideoGames").Result.Count()
                });

            return new ItemsDTO
            {
                Items = videoGames,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Brands",
                Brand = name
            };
        }

        public async Task<ItemsDTO> GetDiscountedVideoGames(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<VideoGame>("Discounted") / (double)pageSize);

            var discountedVideoGames = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<VideoGame>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(c => new VideoGameDTO
                 {
                     Id = c.ID,
                     Name = c.Name,
                     Rate = c.Rate,
                     Price = c.Price,
                     NewPrice = c.NewPrice ?? 0,
                     imageSrc = c.imageSrc,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "VideoGames").Result,
                     CategoryName = c.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "VideoGames").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = discountedVideoGames,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
        }

        public async Task<ItemsDTO> GetTopRatedVideoGames(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<VideoGame>("Rated") / (double)pageSize);


            var ratedVideoGames = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<VideoGame>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(c => new VideoGameDTO
                 {
                     Id = c.ID,
                     Name = c.Name,
                     Rate = c.Rate,
                     Price = c.Price,
                     NewPrice = c.NewPrice ?? 0,
                     imageSrc = c.imageSrc,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "VideoGames").Result,
                     CategoryName = c.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "VideoGames").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = ratedVideoGames,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
        }

        public async Task<ItemsDTO> GetLatestVideoGames(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<VideoGame>("Latest") / (double)pageSize);

            var latestVideoGames = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<VideoGame>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(c => new VideoGameDTO
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "VideoGames").Result,
                    CategoryName = c.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "VideoGames").Result.Count()
                });

            return new ItemsDTO
            {
                Items = latestVideoGames,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
        }

        public async Task<ItemsDTO> GetVideoGamesWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<VideoGame>("Price", price1, price2, null) / (double)pageSize);

            var priceVideoGames = _servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<VideoGame>(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(c => new VideoGameDTO
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "VideoGames").Result,
                    CategoryName = c.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "VideoGames").Result.Count()
                });

            return new ItemsDTO
            {
                Items = priceVideoGames,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
        }

        public async Task<VideoGameDTO> GetVideoGameDetails(int id)
        {
            var videoGame = await this.GetVideoGame(id);

            if (videoGame != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "VideoGames", "Default");

                var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "VideoGames")).Count();

                var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<VideoGame>(id, "VideoGames");

                var totalQuantity = await _servicesInstanceProvider.GetCartServiceInstance().TotalItemQuantityInCart(id, "VideoGames");

                var similarPriceVideoGames = (await _unitOfWork.VideoGames.GetAll())
                    .Where(c => c.Price == videoGame.Price || Math.Abs(c.Price - videoGame.Price) <= 1000)
                    .Select(c => new VideoGameDTO
                    {
                        Id = c.ID,
                        Name = c.Name,
                        Rate = c.Rate,
                        Price = c.Price,
                        NewPrice = c.NewPrice ?? 0,
                        imageSrc = c.imageSrc,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "VideoGames").Result,
                        CategoryName = c.Category.Name,
                        RateCount = rateCount
                    });

                var relatedVideoGames = (await _unitOfWork.VideoGames.GetAll())
                    .Where(c => c.CategoryId == videoGame.CategoryId).Take(10)
                    .Select(c => new VideoGameDTO
                    {
                        Id = c.ID,
                        Name = c.Name,
                        Rate = c.Rate,
                        Price = c.Price,
                        NewPrice = c.NewPrice ?? 0,
                        imageSrc = c.imageSrc,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "VideoGames").Result,
                        CategoryName = c.Category.Name,
                        RateCount = rateCount
                    });

                var offers = _servicesInstanceProvider.GetOffersServiceInstance().GetOffers("Video Games", videoGame.Category?.Name, videoGame.ID);

                var discountValue = string.Empty;

                if (offers.Any())
                    discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                              $"{offers.First().PercentDiscount}%" :
                              offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;

                var BOGOGetItem = await _servicesInstanceProvider.GetOffersServiceInstance().GetBOGOGetItem(videoGame);

                return new VideoGameDTO
                {
                    Id = videoGame.ID,
                    Name = videoGame.Name,
                    Rate = videoGame.Rate,
                    Price = videoGame.Price,
                    NewPrice = videoGame.NewPrice ?? 0,
                    IsDiscounted = videoGame.IsDiscounted,
                    DiscountValue = discountValue,
                    IsBOGOBuy = videoGame.IsBOGOBuy,
                    IsBOGOGet = videoGame.IsBOGOGet,
                    imageSrc = videoGame.imageSrc,
                    CategoryName = videoGame.Category.Name,
                    RelatedVideoGames = relatedVideoGames,
                    SimilarPriceVideoGames = similarPriceVideoGames,
                    Comments = comments,
                    Offers = offers,
                    BOGOGet = BOGOGetItem,
                    StarCounts = starCounts,
                    RateCount = rateCount,
                    ControllerName = "VideoGames",
                    TotalQuantity = totalQuantity
                };
            }

            else
                return null;
        }

        public async Task<VideoGameDTO> GetVideoGameAllComments(int id)
        {
            var VideoGame = await this.GetVideoGame(id);

            var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "VideoGames")).Count();

            var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<VideoGame>(id, "VideoGames");

            if (VideoGame != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "VideoGames", "All");

                if (comments.Any())
                {
                    return new VideoGameDTO
                    {
                        Id = VideoGame.ID,
                        Name = VideoGame.Name,
                        Rate = VideoGame.Rate,
                        CategoryName = VideoGame.Category.Name,
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
            return await _servicesInstanceProvider.GetCategoriesServiceInstance().GetSpecificCategories("Video Games");
        }
    }
}
