using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class HeadPhonesService : IHeadPhonesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public HeadPhonesService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<HeadPhone> GetHeadPhone(int id)
        {
            return await _unitOfWork.HeadPhones.GetById(id);
        }

        public IEnumerable<HeadPhone> GetHeadPhones(int pageNumber, int pageSize)
        {
            return _unitOfWork.HeadPhones.GetAll(pageNumber, pageSize);
        }

        public async Task<Result> Add(HeadPhone headPhone)
        {
            var stream = new MemoryStream();
            await headPhone.clientFile.CopyToAsync(stream);
            headPhone.dbImage = stream.ToArray();

            var result = await _unitOfWork.HeadPhones.Add(headPhone);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while adding." };
        }

        public async Task<Result> Update(HeadPhone headPhone)
        {
            var stream = new MemoryStream();
            await headPhone.clientFile.CopyToAsync(stream);
            headPhone.dbImage = stream.ToArray();

            var result = _unitOfWork.HeadPhones.Update(headPhone);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while updating." };
        }

        public async Task<Result> Delete(HeadPhone headPhone)
        {
            var result = _unitOfWork.HeadPhones.Delete(headPhone);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while deleting." };
        }

        public ItemDTO<HeadPhoneDTO> GetHeadPhonesWithRelatedOnes()
        {
            var headPhonesCategories = _servicesInstanceProvider.GetItemsServiceInstance().GetItemCategories<HeadPhone>();

            var discountedHeadPhones = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<HeadPhone>(1, 10, "ID", false).ToList().
                Select(h => new HeadPhoneDTO
                {
                    Id = h.ID,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice ?? 0,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), h.ID, "HeadPhones").Result,
                    CategoryName = h.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(h.ID, "HeadPhones").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var topRatedHeadPhones = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<HeadPhone>(1, 10, "ID", false).ToList().
                Select(h => new HeadPhoneDTO
                {
                    Id = h.ID,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice ?? 0,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), h.ID, "HeadPhones").Result,
                    CategoryName = h.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(h.ID, "HeadPhones").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var latestHeadPhones = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<HeadPhone>(1, 10, "ID", false).ToList().
                Select(h => new HeadPhoneDTO
                {
                    Id = h.ID,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice ?? 0,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), h.ID, "HeadPhones").Result,
                    CategoryName = h.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(h.ID, "HeadPhones").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            return new ItemDTO<HeadPhoneDTO>()
            {
                ItemCategories = headPhonesCategories,
                DiscountedItems = discountedHeadPhones,
                latestItems = latestHeadPhones,
                TopRatedItems = topRatedHeadPhones,
            };
        }

        public async Task<ItemsDTO> GetBrandsHeadPhones(string? orderIndex, int? page, string name, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<HeadPhone>("Brands", null, null, name) / (double)pageSize);

            var headPhones = _servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<HeadPhone>(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(h => new HeadPhoneDTO
                {
                    Id = h.ID,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice ?? 0,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), h.ID, "HeadPhones").Result,
                    CategoryName = h.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(h.ID, "HeadPhones").Result.Count()
                });

            return new ItemsDTO
            {
                Items = headPhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Brands",
                Brand = name
            };
        }

        public async Task<ItemsDTO> GetDiscountedHeadPhones(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<HeadPhone>("Discounted") / (double)pageSize);

            var discountedHeadPhones = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<HeadPhone>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                  Select(h => new HeadPhoneDTO
                  {
                      Id = h.ID,
                      Name = h.Name,
                      Rate = h.Rate,
                      Price = h.Price,
                      NewPrice = h.NewPrice ?? 0,
                      imageSrc = h.imageSrc,
                      ConnectivityTechnology = h.ConnectivityTechnology,
                      Color = h.Color,
                      NoiseControl = h.NoiseControl,
                      HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                      HeadphonesFormFactor = h.HeadphonesFormFactor,
                      ModelName = h.ModelName,
                      isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), h.ID, "HeadPhones").Result,
                      CategoryName = h.Category.Name,
                      RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(h.ID, "HeadPhones").Result.Count()
                  });

            return new ItemsDTO
            {
                Items = discountedHeadPhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
        }

        public async Task<ItemsDTO> GetTopRatedHeadPhones(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<HeadPhone>("Rated") / (double)pageSize);


            var ratedHeadPhones = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<HeadPhone>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(h => new HeadPhoneDTO
                 {
                     Id = h.ID,
                     Name = h.Name,
                     Rate = h.Rate,
                     Price = h.Price,
                     NewPrice = h.NewPrice ?? 0,
                     imageSrc = h.imageSrc,
                     ConnectivityTechnology = h.ConnectivityTechnology,
                     Color = h.Color,
                     NoiseControl = h.NoiseControl,
                     HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                     HeadphonesFormFactor = h.HeadphonesFormFactor,
                     ModelName = h.ModelName,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), h.ID, "HeadPhones").Result,
                     CategoryName = h.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(h.ID, "HeadPhones").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = ratedHeadPhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
        }

        public async Task<ItemsDTO> GetLatestHeadPhones(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<HeadPhone>("Latest") / (double)pageSize);

            var latestHeadPhones = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<HeadPhone>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(h => new HeadPhoneDTO
                 {
                     Id = h.ID,
                     Name = h.Name,
                     Rate = h.Rate,
                     Price = h.Price,
                     NewPrice = h.NewPrice ?? 0,
                     imageSrc = h.imageSrc,
                     ConnectivityTechnology = h.ConnectivityTechnology,
                     Color = h.Color,
                     NoiseControl = h.NoiseControl,
                     HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                     HeadphonesFormFactor = h.HeadphonesFormFactor,
                     ModelName = h.ModelName,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), h.ID, "HeadPhones").Result,
                     CategoryName = h.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(h.ID, "HeadPhones").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = latestHeadPhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
        }

        public async Task<ItemsDTO> GetHeadPhonesWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<HeadPhone>("Price", price1, price2, null) / (double)pageSize);

            var priceHeadPhones = _servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<HeadPhone>(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(h => new HeadPhoneDTO
                 {
                     Id = h.ID,
                     Name = h.Name,
                     Rate = h.Rate,
                     Price = h.Price,
                     NewPrice = h.NewPrice ?? 0,
                     imageSrc = h.imageSrc,
                     ConnectivityTechnology = h.ConnectivityTechnology,
                     Color = h.Color,
                     NoiseControl = h.NoiseControl,
                     HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                     HeadphonesFormFactor = h.HeadphonesFormFactor,
                     ModelName = h.ModelName,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), h.ID, "HeadPhones").Result,
                     CategoryName = h.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(h.ID, "HeadPhones").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = priceHeadPhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
        }

        public async Task<HeadPhoneDTO> GetHeadPhoneDetails(int id)
        {
            var headPhone = await this.GetHeadPhone(id);

            if (headPhone != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "HeadPhones", "Default");

                var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "HeadPhones")).Count();

                var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<HeadPhone>(id, "HeadPhones");

                var totalQuantity = await _servicesInstanceProvider.GetCartServiceInstance().TotalItemQuantityInCart(id, "HeadPhones");

                var similarPriceHeadPhones = (await _unitOfWork.HeadPhones.GetAll())
                    .Where(h => h.Price == headPhone.Price || Math.Abs(h.Price - headPhone.Price) <= 1000)
                    .Select(h => new HeadPhoneDTO
                    {
                        Id = h.ID,
                        Name = h.Name,
                        Rate = h.Rate,
                        Price = h.Price,
                        NewPrice = h.NewPrice ?? 0,
                        imageSrc = h.imageSrc,
                        ConnectivityTechnology = h.ConnectivityTechnology,
                        Color = h.Color,
                        NoiseControl = h.NoiseControl,
                        HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                        HeadphonesFormFactor = h.HeadphonesFormFactor,
                        ModelName = h.ModelName,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), h.ID, "HeadPhones").Result,
                        CategoryName = h.Category.Name,
                        RateCount = rateCount
                    });

                var relatedHeadPhones = (await _unitOfWork.HeadPhones.GetAll())
                    .Where(h => h.CategoryId == headPhone.CategoryId).Take(10)
                    .Select(h => new HeadPhoneDTO
                    {
                        Id = h.ID,
                        Name = h.Name,
                        Rate = h.Rate,
                        Price = h.Price,
                        NewPrice = h.NewPrice ?? 0,
                        imageSrc = h.imageSrc,
                        ConnectivityTechnology = h.ConnectivityTechnology,
                        Color = h.Color,
                        NoiseControl = h.NoiseControl,
                        HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                        HeadphonesFormFactor = h.HeadphonesFormFactor,
                        ModelName = h.ModelName,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), h.ID, "HeadPhones").Result,
                        CategoryName = h.Category.Name,
                        RateCount = rateCount
                    });

                var offers = _servicesInstanceProvider.GetOffersServiceInstance().GetOffers("Electronics", headPhone.Category?.Name, headPhone.ID);

                var discountValue = string.Empty;

                if (offers.Any())
                    discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                              $"{offers.First().PercentDiscount}%" :
                              offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;

                var BOGOGetItem = await _servicesInstanceProvider.GetOffersServiceInstance().GetBOGOGetItem(headPhone);

                return new HeadPhoneDTO
                {
                    Id = headPhone.ID,
                    Name = headPhone.Name,
                    Rate = headPhone.Rate,
                    Price = headPhone.Price,
                    NewPrice = headPhone.NewPrice ?? 0,
                    IsDiscounted = headPhone.IsDiscounted,
                    DiscountValue = discountValue,
                    IsBOGOBuy = headPhone.IsBOGOBuy,
                    IsBOGOGet = headPhone.IsBOGOGet,
                    imageSrc = headPhone.imageSrc,
                    ConnectivityTechnology = headPhone.ConnectivityTechnology,
                    Color = headPhone.Color,
                    NoiseControl = headPhone.NoiseControl,
                    HeadphonesEarPlacement = headPhone.HeadphonesEarPlacement,
                    HeadphonesFormFactor = headPhone.HeadphonesFormFactor,
                    ModelName = headPhone.ModelName,
                    CategoryName = headPhone.Category.Name,
                    RelatedHeadPhones = relatedHeadPhones,
                    SimilarPriceHeadPhones = similarPriceHeadPhones,
                    Comments = comments,
                    Offers = offers,
                    BOGOGet = BOGOGetItem,
                    StarCounts = starCounts,
                    RateCount = rateCount,
                    ControllerName = "HeadPhones",
                    TotalQuantity = totalQuantity
                };
            }

            else
                return null;
        }

        public async Task<HeadPhoneDTO> GetHeadPhoneAllComments(int id)
        {
            var HeadPhone = await this.GetHeadPhone(id);

            var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "HeadPhones")).Count();

            var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<HeadPhone>(id, "HeadPhones");

            if (HeadPhone != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "HeadPhones", "All");

                if (comments.Any())
                {
                    return new HeadPhoneDTO
                    {
                        Id = HeadPhone.ID,
                        Name = HeadPhone.Name,
                        Rate = HeadPhone.Rate,
                        CategoryName = HeadPhone.Category.Name,
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
