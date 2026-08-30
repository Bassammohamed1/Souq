using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class AirConditionersService : IAirConditionersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public AirConditionersService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _servicesInstanceProvider = servicesInstanceProvider;
            _userService = userService;
        }

        public async Task<AirConditioner> GetAirConditioner(int id)
        {
            return await _unitOfWork.AirConditioners.GetById(id);
        }

        public IEnumerable<AirConditioner> GetAirConditioners(int pageNumber, int pageSize)
        {
            return _unitOfWork.AirConditioners.GetAll(pageNumber, pageSize);
        }

        public async Task<Result> Add(AirConditioner airConditioner)
        {
            var stream = new MemoryStream();
            await airConditioner.clientFile.CopyToAsync(stream);
            airConditioner.dbImage = stream.ToArray();

            var result = await _unitOfWork.AirConditioners.Add(airConditioner);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while adding." };
        }

        public async Task<Result> Update(AirConditioner airConditioner)
        {
            var stream = new MemoryStream();
            await airConditioner.clientFile.CopyToAsync(stream);
            airConditioner.dbImage = stream.ToArray();

            var result = _unitOfWork.AirConditioners.Update(airConditioner);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while updating." };
        }

        public async Task<Result> Delete(AirConditioner airConditioner)
        {
            var result = _unitOfWork.AirConditioners.Delete(airConditioner);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while deleting." };
        }

        public ItemDTO<AirConditionerDTO> GetAirConditionersWithRelatedOnes()
        {
            var airConditionersCategories = _servicesInstanceProvider.GetItemsServiceInstance().GetItemCategories<AirConditioner>();

            var discountedAirConditioners = (_servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<AirConditioner>(1, 10, "ID", false)).ToList().
                Select(a => new AirConditionerDTO
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), a.ID, "AirConditioners").Result,
                    CategoryName = a.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(a.ID, "AirConditioners").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var topRatedAirConditioners = (_servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<AirConditioner>(1, 10, "ID", false)).ToList().
                Select(a => new AirConditionerDTO
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), a.ID, "AirConditioners").Result,
                    CategoryName = a.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(a.ID, "AirConditioners").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var latestAirConditioners = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<AirConditioner>(1, 10, "ID", false).ToList().
                Select(a => new AirConditionerDTO
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), a.ID, "AirConditioners").Result,
                    CategoryName = a.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(a.ID, "AirConditioners").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            return new ItemDTO<AirConditionerDTO>()
            {
                ItemCategories = airConditionersCategories,
                DiscountedItems = discountedAirConditioners,
                latestItems = latestAirConditioners,
                TopRatedItems = topRatedAirConditioners,
            };
        }

        public async Task<ItemsDTO> GetBrandsAirConditioners(string? orderIndex, int? page, string name, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<AirConditioner>("Brands", null, null, name) / (double)pageSize);

            var airConditioners = (_servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<AirConditioner>(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false)).ToList().
                Select(a => new AirConditionerDTO
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), a.ID, "AirConditioners").Result,
                    CategoryName = a.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(a.ID, "AirConditioners").Result.Count()
                });

            return new ItemsDTO
            {
                Items = airConditioners,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Brands",
                Brand = name
            };
        }

        public async Task<ItemsDTO> GetDiscountedAirConditioners(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<AirConditioner>("Discounted") / (double)pageSize);

            var discountedAirConditioners = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<AirConditioner>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(a => new AirConditionerDTO
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), a.ID, "AirConditioners").Result,
                    CategoryName = a.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(a.ID, "AirConditioners").Result.Count()
                });

            return new ItemsDTO
            {
                Items = discountedAirConditioners,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
        }

        public async Task<ItemsDTO> GetTopRatedAirConditioners(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<AirConditioner>("Rated") / (double)pageSize);


            var ratedAirConditioners = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<AirConditioner>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(a => new AirConditionerDTO
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), a.ID, "AirConditioners").Result,
                    CategoryName = a.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(a.ID, "AirConditioners").Result.Count()
                });

            return new ItemsDTO
            {
                Items = ratedAirConditioners,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
        }

        public async Task<ItemsDTO> GetLatestAirConditioners(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<AirConditioner>("Latest") / (double)pageSize);

            var latestAirConditioners = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<AirConditioner>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(a => new AirConditionerDTO
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), a.ID, "AirConditioners").Result,
                    CategoryName = a.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(a.ID, "AirConditioners").Result.Count()
                });

            return new ItemsDTO
            {
                Items = latestAirConditioners,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
        }

        public async Task<ItemsDTO> GetAirConditionersWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<AirConditioner>("Price", price1, price2, null) / (double)pageSize);

            var priceAirConditioners = _servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<AirConditioner>(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(a => new AirConditionerDTO
                 {
                     Id = a.ID,
                     Name = a.Name,
                     Rate = a.Rate,
                     Price = a.Price,
                     NewPrice = a.NewPrice ?? 0,
                     imageSrc = a.imageSrc,
                     Color = a.Color,
                     Capacity = a.Capacity,
                     CoolingPower = a.CoolingPower,
                     Voltage = a.Voltage,
                     ItemDimensions = a.ItemDimensions,
                     NoiseLevel = a.NoiseLevel,
                     SpecialFeatures = a.SpecialFeatures,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), a.ID, "AirConditioners").Result,
                     CategoryName = a.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(a.ID, "AirConditioners").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = priceAirConditioners,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
        }

        public async Task<AirConditionerDTO> GetAirConditionerDetails(int id)
        {
            var airConditioner = await this.GetAirConditioner(id);

            if (airConditioner != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "AirConditioners", "Default");

                var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "AirConditioners")).Count();

                var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<AirConditioner>(id, "AirConditioners");

                var totalQuantity = await _servicesInstanceProvider.GetCartServiceInstance().TotalItemQuantityInCart(id, "AirConditioners");

                var similarPriceAirConditioners = (await _unitOfWork.AirConditioners.GetAll())
                    .Where(a => a.Price == airConditioner.Price || Math.Abs(a.Price - airConditioner.Price) <= 1000)
                    .Select(a => new AirConditionerDTO
                    {
                        Id = a.ID,
                        Name = a.Name,
                        Rate = a.Rate,
                        Price = a.Price,
                        NewPrice = a.NewPrice ?? 0,
                        imageSrc = a.imageSrc,
                        Color = a.Color,
                        Capacity = a.Capacity,
                        CoolingPower = a.CoolingPower,
                        Voltage = a.Voltage,
                        ItemDimensions = a.ItemDimensions,
                        NoiseLevel = a.NoiseLevel,
                        SpecialFeatures = a.SpecialFeatures,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), a.ID, "AirConditioners").Result,
                        CategoryName = a.Category.Name,
                        RateCount = rateCount
                    });

                var relatedAirConditioners = (await _unitOfWork.AirConditioners.GetAll())
                    .Where(a => a.CategoryId == airConditioner.CategoryId).Take(10)
                    .Select(a => new AirConditionerDTO
                    {
                        Id = a.ID,
                        Name = a.Name,
                        Rate = a.Rate,
                        Price = a.Price,
                        NewPrice = a.NewPrice ?? 0,
                        imageSrc = a.imageSrc,
                        Color = a.Color,
                        Capacity = a.Capacity,
                        CoolingPower = a.CoolingPower,
                        Voltage = a.Voltage,
                        ItemDimensions = a.ItemDimensions,
                        NoiseLevel = a.NoiseLevel,
                        SpecialFeatures = a.SpecialFeatures,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), a.ID, "AirConditioners").Result,
                        CategoryName = a.Category.Name,
                        RateCount = rateCount
                    });

                var offers = _servicesInstanceProvider.GetOffersServiceInstance().GetOffers("Appliances", airConditioner.Category?.Name, airConditioner.ID);

                var discountValue = string.Empty;

                if (offers.Any())
                    discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                              $"{offers.First().PercentDiscount}%" :
                              offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;

                var BOGOGetItem = await _servicesInstanceProvider.GetOffersServiceInstance().GetBOGOGetItem(airConditioner);

                return new AirConditionerDTO
                {
                    Id = airConditioner.ID,
                    Name = airConditioner.Name,
                    Rate = airConditioner.Rate,
                    Price = airConditioner.Price,
                    NewPrice = airConditioner.NewPrice ?? 0,
                    IsDiscounted = airConditioner.IsDiscounted,
                    DiscountValue = discountValue,
                    IsBOGOBuy = airConditioner.IsBOGOBuy,
                    IsBOGOGet = airConditioner.IsBOGOGet,
                    imageSrc = airConditioner.imageSrc,
                    Color = airConditioner.Color,
                    Capacity = airConditioner.Capacity,
                    CoolingPower = airConditioner.CoolingPower,
                    Voltage = airConditioner.Voltage,
                    ItemDimensions = airConditioner.ItemDimensions,
                    NoiseLevel = airConditioner.NoiseLevel,
                    SpecialFeatures = airConditioner.SpecialFeatures,
                    CategoryName = airConditioner.Category.Name,
                    RelatedAirConditioners = relatedAirConditioners,
                    SimilarPriceAirConditioners = similarPriceAirConditioners,
                    Comments = comments,
                    Offers = offers,
                    BOGOGet = BOGOGetItem,
                    StarCounts = starCounts,
                    RateCount = rateCount,
                    ControllerName = "AirConditioners",
                    TotalQuantity = totalQuantity
                };
            }

            else
                return null;
        }

        public async Task<AirConditionerDTO> GetAirConditionerAllComments(int id)
        {
            var AirConditioner = await this.GetAirConditioner(id);

            var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "AirConditioners")).Count();

            var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<AirConditioner>(id, "AirConditioners");

            if (AirConditioner != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "AirConditioners", "All");

                if (comments.Any())
                {
                    return new AirConditionerDTO
                    {
                        Id = AirConditioner.ID,
                        Name = AirConditioner.Name,
                        Rate = AirConditioner.Rate,
                        CategoryName = AirConditioner.Category.Name,
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
            return await _servicesInstanceProvider.GetCategoriesServiceInstance().GetSpecificCategories("Appliances");
        }
    }
}