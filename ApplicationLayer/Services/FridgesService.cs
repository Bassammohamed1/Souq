using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class FridgesService : IFridgesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public FridgesService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<Fridge> GetFridge(int id)
        {
            return await _unitOfWork.Fridges.GetById(id);
        }

        public IEnumerable<Fridge> GetFridges(int pageNumber, int pageSize)
        {
            return _unitOfWork.Fridges.GetAll(pageNumber, pageSize);
        }

        public async Task<Result> Add(Fridge fridge)
        {
            var stream = new MemoryStream();
            await fridge.clientFile.CopyToAsync(stream);
            fridge.dbImage = stream.ToArray();

            var result = await _unitOfWork.Fridges.Add(fridge);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while adding." };
        }

        public async Task<Result> Update(Fridge fridge)
        {
            var stream = new MemoryStream();
            await fridge.clientFile.CopyToAsync(stream);
            fridge.dbImage = stream.ToArray();

            var result = _unitOfWork.Fridges.Update(fridge);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while updating." };
        }

        public async Task<Result> Delete(Fridge fridge)
        {
            var result = _unitOfWork.Fridges.Delete(fridge);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while deleting." };
        }

        public ItemDTO<FridgeDTO> GetFridgesWithRelatedOnes()
        {
            var fridgesCategories = _servicesInstanceProvider.GetItemsServiceInstance().GetItemCategories<Fridge>();

            var discountedFridges = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<Fridge>(1, 10, "ID", false).ToList().
                Select(f => new FridgeDTO
                {
                    Id = f.ID,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice ?? 0,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), f.ID, "Fridges").Result,
                    CategoryName = f.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(f.ID, "Fridges").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var topRatedFridges = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<Fridge>(1, 10, "ID", false).ToList().
                Select(f => new FridgeDTO
                {
                    Id = f.ID,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice ?? 0,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), f.ID, "Fridges").Result,
                    CategoryName = f.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(f.ID, "Fridges").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var latestFridges = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<Fridge>(1, 10, "ID", false).ToList().
                Select(f => new FridgeDTO
                {
                    Id = f.ID,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice ?? 0,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), f.ID, "Fridges").Result,
                    CategoryName = f.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(f.ID, "Fridges").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            return new ItemDTO<FridgeDTO>()
            {
                ItemCategories = fridgesCategories,
                DiscountedItems = discountedFridges,
                latestItems = latestFridges,
                TopRatedItems = topRatedFridges,
            };
        }

        public async Task<ItemsDTO> GetBrandsFridges(string? orderIndex, int? page, string name, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Fridge>("Brands", null, null, name) / (double)pageSize);

            var fridges = _servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<Fridge>(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(f => new FridgeDTO
                 {
                     Id = f.ID,
                     Name = f.Name,
                     Rate = f.Rate,
                     Price = f.Price,
                     NewPrice = f.NewPrice ?? 0,
                     imageSrc = f.imageSrc,
                     Capacity = f.Capacity,
                     Color = f.Color,
                     DefrostSystem = f.DefrostSystem,
                     EnergyStar = f.EnergyStar,
                     InstallationType = f.InstallationType,
                     ItemDimensions = f.ItemDimensions,
                     NumberOfDoors = f.NumberOfDoors,
                     SpecialFeatures = f.SpecialFeatures,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), f.ID, "Fridges").Result,
                     CategoryName = f.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(f.ID, "Fridges").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = fridges,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Brands",
                Brand = name
            };
        }

        public async Task<ItemsDTO> GetDiscountedFridges(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Fridge>("Discounted") / (double)pageSize);

            var discountedFridges = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<Fridge>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(f => new FridgeDTO
                 {
                     Id = f.ID,
                     Name = f.Name,
                     Rate = f.Rate,
                     Price = f.Price,
                     NewPrice = f.NewPrice ?? 0,
                     imageSrc = f.imageSrc,
                     Capacity = f.Capacity,
                     Color = f.Color,
                     DefrostSystem = f.DefrostSystem,
                     EnergyStar = f.EnergyStar,
                     InstallationType = f.InstallationType,
                     ItemDimensions = f.ItemDimensions,
                     NumberOfDoors = f.NumberOfDoors,
                     SpecialFeatures = f.SpecialFeatures,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), f.ID, "Fridges").Result,
                     CategoryName = f.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(f.ID, "Fridges").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = discountedFridges,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
        }

        public async Task<ItemsDTO> GetTopRatedFridges(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Fridge>("Rated") / (double)pageSize);


            var ratedFridges = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<Fridge>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(f => new FridgeDTO
                 {
                     Id = f.ID,
                     Name = f.Name,
                     Rate = f.Rate,
                     Price = f.Price,
                     NewPrice = f.NewPrice ?? 0,
                     imageSrc = f.imageSrc,
                     Capacity = f.Capacity,
                     Color = f.Color,
                     DefrostSystem = f.DefrostSystem,
                     EnergyStar = f.EnergyStar,
                     InstallationType = f.InstallationType,
                     ItemDimensions = f.ItemDimensions,
                     NumberOfDoors = f.NumberOfDoors,
                     SpecialFeatures = f.SpecialFeatures,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), f.ID, "Fridges").Result,
                     CategoryName = f.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(f.ID, "Fridges").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = ratedFridges,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
        }

        public async Task<ItemsDTO> GetLatestFridges(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Fridge>("Latest") / (double)pageSize);

            var latestFridges = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<Fridge>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(async f => new FridgeDTO
                {
                    Id = f.ID,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice ?? 0,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = await _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), f.ID, "Fridges"),
                    CategoryName = f.Category.Name,
                    RateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(f.ID, "Fridges")).Count()
                }).ToList();

            return new ItemsDTO
            {
                Items = latestFridges,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
        }

        public async Task<ItemsDTO> GetFridgesWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Fridge>("Price", price1, price2, null) / (double)pageSize);

            var priceFridges = _servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<Fridge>(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                  Select(f => new FridgeDTO
                  {
                      Id = f.ID,
                      Name = f.Name,
                      Rate = f.Rate,
                      Price = f.Price,
                      NewPrice = f.NewPrice ?? 0,
                      imageSrc = f.imageSrc,
                      Capacity = f.Capacity,
                      Color = f.Color,
                      DefrostSystem = f.DefrostSystem,
                      EnergyStar = f.EnergyStar,
                      InstallationType = f.InstallationType,
                      ItemDimensions = f.ItemDimensions,
                      NumberOfDoors = f.NumberOfDoors,
                      SpecialFeatures = f.SpecialFeatures,
                      isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), f.ID, "Fridges").Result,
                      CategoryName = f.Category.Name,
                      RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(f.ID, "Fridges").Result.Count()
                  });

            return new ItemsDTO
            {
                Items = priceFridges,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
        }

        public async Task<FridgeDTO> GetFridgeDetails(int id)
        {
            var fridge = await this.GetFridge(id);

            if (fridge != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "Fridges", "Default");

                var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "Fridges")).Count();

                var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<Fridge>(id, "Fridges");

                var totalQuantity = await _servicesInstanceProvider.GetCartServiceInstance().TotalItemQuantityInCart(id, "Fridges");

                var similarPriceFridges = (await _unitOfWork.Fridges.GetAll())
                    .Where(f => f.Price == fridge.Price || Math.Abs(f.Price - fridge.Price) <= 1000)
                    .Select( f => new FridgeDTO
                    {
                        Id = f.ID,
                        Name = f.Name,
                        Rate = f.Rate,
                        Price = f.Price,
                        NewPrice = f.NewPrice ?? 0,
                        imageSrc = f.imageSrc,
                        Capacity = f.Capacity,
                        Color = f.Color,
                        DefrostSystem = f.DefrostSystem,
                        EnergyStar = f.EnergyStar,
                        InstallationType = f.InstallationType,
                        ItemDimensions = f.ItemDimensions,
                        NumberOfDoors = f.NumberOfDoors,
                        SpecialFeatures = f.SpecialFeatures,
                        isLiked =  _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), f.ID, "Fridges").Result,
                        CategoryName = f.Category.Name,
                        RateCount = rateCount
                    });

                var relatedFridges = (await _unitOfWork.Fridges.GetAll())
                    .Where(f => f.CategoryId == fridge.CategoryId).Take(10)
                    .Select( f => new FridgeDTO
                    {
                        Id = f.ID,
                        Name = f.Name,
                        Rate = f.Rate,
                        Price = f.Price,
                        NewPrice = f.NewPrice ?? 0,
                        imageSrc = f.imageSrc,
                        Capacity = f.Capacity,
                        Color = f.Color,
                        DefrostSystem = f.DefrostSystem,
                        EnergyStar = f.EnergyStar,
                        InstallationType = f.InstallationType,
                        ItemDimensions = f.ItemDimensions,
                        NumberOfDoors = f.NumberOfDoors,
                        SpecialFeatures = f.SpecialFeatures,
                        isLiked =  _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), f.ID, "Fridges").Result,
                        CategoryName = f.Category.Name,
                        RateCount = rateCount
                    });

                var offers = _servicesInstanceProvider.GetOffersServiceInstance().GetOffers("Appliances", fridge.Category?.Name, fridge.ID);

                var discountValue = string.Empty;

                if (offers.Any())
                    discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                              $"{offers.First().PercentDiscount}%" :
                              offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;

                var BOGOGetItem = await _servicesInstanceProvider.GetOffersServiceInstance().GetBOGOGetItem(fridge);

                return new FridgeDTO
                {
                    Id = fridge.ID,
                    Name = fridge.Name,
                    Rate = fridge.Rate,
                    Price = fridge.Price,
                    NewPrice = fridge.NewPrice ?? 0,
                    IsDiscounted = fridge.IsDiscounted,
                    DiscountValue = discountValue,
                    IsBOGOBuy = fridge.IsBOGOBuy,
                    IsBOGOGet = fridge.IsBOGOGet,
                    imageSrc = fridge.imageSrc,
                    Capacity = fridge.Capacity,
                    Color = fridge.Color,
                    DefrostSystem = fridge.DefrostSystem,
                    EnergyStar = fridge.EnergyStar,
                    InstallationType = fridge.InstallationType,
                    ItemDimensions = fridge.ItemDimensions,
                    NumberOfDoors = fridge.NumberOfDoors,
                    SpecialFeatures = fridge.SpecialFeatures,
                    CategoryName = fridge.Category.Name,
                    RelatedFridges = relatedFridges,
                    SimilarPriceFridges = similarPriceFridges,
                    Comments = comments,
                    Offers = offers,
                    BOGOGet = BOGOGetItem,
                    StarCounts = starCounts,
                    RateCount = rateCount,
                    ControllerName = "Fridges",
                    TotalQuantity = totalQuantity
                };
            }

            else
                return null;
        }

        public async Task<FridgeDTO> GetFridgeAllComments(int id)
        {
            var Fridge = await this.GetFridge(id);

            var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "Fridges")).Count();

            var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<Fridge>(id, "Fridges");

            if (Fridge != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "Fridges", "All");

                if (comments.Any())
                {
                    return new FridgeDTO
                    {
                        Id = Fridge.ID,
                        Name = Fridge.Name,
                        Rate = Fridge.Rate,
                        CategoryName = Fridge.Category.Name,
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
