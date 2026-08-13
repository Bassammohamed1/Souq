using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class CookersService : ICookersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public CookersService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<Cooker> GetCooker(int id)
        {
            return await _unitOfWork.Cookers.GetById(id);
        }

        public IEnumerable<Cooker> GetCookers(int pageNumber, int pageSize)
        {
            return _unitOfWork.Cookers.GetAll(pageNumber, pageSize);
        }

        public async Task<Result> Add(Cooker cooker)
        {
            var stream = new MemoryStream();
            await cooker.clientFile.CopyToAsync(stream);
            cooker.dbImage = stream.ToArray();

            var result = await _unitOfWork.Cookers.Add(cooker);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while adding." };
        }

        public async Task<Result> Update(Cooker cooker)
        {
            var stream = new MemoryStream();
            await cooker.clientFile.CopyToAsync(stream);
            cooker.dbImage = stream.ToArray();

            var result = _unitOfWork.Cookers.Update(cooker);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while updating." };
        }

        public async Task<Result> Delete(Cooker cooker)
        {
            var result = _unitOfWork.Cookers.Delete(cooker);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while deleting." };
        }

        public ItemDTO<CookerDTO> GetCookersWithRelatedOnes()
        {
            var cookersCategories = _servicesInstanceProvider.GetItemsServiceInstance().GetItemCategories<Cooker>();

            var discountedCookers = (_servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<Cooker>(1, 10, "ID", false)).ToList().
                Select(c => new CookerDTO
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    ModelName = c.ModelName,
                    Material = c.Material,
                    ItemWeight = c.ItemWeight,
                    Color = c.Color,
                    ItemDimensions = c.ItemDimensions,
                    DrawerType = c.DrawerType,
                    ControlsType = c.ControlsType,
                    FinishType = c.FinishType,
                    FormFactor = c.FormFactor,
                    NumberOfHeatingElements = c.NumberOfHeatingElements,
                    SpecialFeatures = c.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "Cookers").Result,
                    CategoryName = c.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "Cookers").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var topRatedCookers = (_servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<Cooker>(1, 10, "ID", false)).ToList().
                Select(c => new CookerDTO
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    ModelName = c.ModelName,
                    Material = c.Material,
                    ItemWeight = c.ItemWeight,
                    Color = c.Color,
                    ItemDimensions = c.ItemDimensions,
                    DrawerType = c.DrawerType,
                    ControlsType = c.ControlsType,
                    FinishType = c.FinishType,
                    FormFactor = c.FormFactor,
                    NumberOfHeatingElements = c.NumberOfHeatingElements,
                    SpecialFeatures = c.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "Cookers").Result,
                    CategoryName = c.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "Cookers").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var latestCookers = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<Cooker>(1, 10, "ID", false).ToList().
                Select(c => new CookerDTO
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    ModelName = c.ModelName,
                    Material = c.Material,
                    ItemWeight = c.ItemWeight,
                    Color = c.Color,
                    ItemDimensions = c.ItemDimensions,
                    DrawerType = c.DrawerType,
                    ControlsType = c.ControlsType,
                    FinishType = c.FinishType,
                    FormFactor = c.FormFactor,
                    NumberOfHeatingElements = c.NumberOfHeatingElements,
                    SpecialFeatures = c.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "Cookers").Result,
                    CategoryName = c.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "Cookers").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            return new ItemDTO<CookerDTO>()
            {
                ItemCategories = cookersCategories,
                DiscountedItems = discountedCookers,
                latestItems = latestCookers,
                TopRatedItems = topRatedCookers
            };
        }

        public async Task<ItemsDTO> GetBrandsCookers(string? orderIndex, int? page, string name, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Cooker>("Brands", null, null, name) / (double)pageSize);

            var cookers = (_servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<Cooker>(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false)).ToList().
                  Select(c => new CookerDTO
                  {
                      Id = c.ID,
                      Name = c.Name,
                      Rate = c.Rate,
                      Price = c.Price,
                      NewPrice = c.NewPrice ?? 0,
                      imageSrc = c.imageSrc,
                      ModelName = c.ModelName,
                      Material = c.Material,
                      ItemWeight = c.ItemWeight,
                      Color = c.Color,
                      ItemDimensions = c.ItemDimensions,
                      DrawerType = c.DrawerType,
                      ControlsType = c.ControlsType,
                      FinishType = c.FinishType,
                      FormFactor = c.FormFactor,
                      NumberOfHeatingElements = c.NumberOfHeatingElements,
                      SpecialFeatures = c.SpecialFeatures,
                      isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "Cookers").Result,
                      CategoryName = c.Category.Name,
                      RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "Cookers").Result.Count()
                  });

            return new ItemsDTO
            {
                Items = cookers,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Brands",
                Brand = name
            };
        }

        public async Task<ItemsDTO> GetDiscountedCookers(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Cooker>("Discounted") / (double)pageSize);

            var discountedCookers = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<Cooker>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(c => new CookerDTO
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    ModelName = c.ModelName,
                    Material = c.Material,
                    ItemWeight = c.ItemWeight,
                    Color = c.Color,
                    ItemDimensions = c.ItemDimensions,
                    DrawerType = c.DrawerType,
                    ControlsType = c.ControlsType,
                    FinishType = c.FinishType,
                    FormFactor = c.FormFactor,
                    NumberOfHeatingElements = c.NumberOfHeatingElements,
                    SpecialFeatures = c.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "Cookers").Result,
                    CategoryName = c.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "Cookers").Result.Count()
                });

            return new ItemsDTO
            {
                Items = discountedCookers,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
        }

        public async Task<ItemsDTO> GetTopRatedCookers(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Cooker>("Rated") / (double)pageSize);


            var ratedCookers = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<Cooker>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(c => new CookerDTO
                 {
                     Id = c.ID,
                     Name = c.Name,
                     Rate = c.Rate,
                     Price = c.Price,
                     NewPrice = c.NewPrice ?? 0,
                     imageSrc = c.imageSrc,
                     ModelName = c.ModelName,
                     Material = c.Material,
                     ItemWeight = c.ItemWeight,
                     Color = c.Color,
                     ItemDimensions = c.ItemDimensions,
                     DrawerType = c.DrawerType,
                     ControlsType = c.ControlsType,
                     FinishType = c.FinishType,
                     FormFactor = c.FormFactor,
                     NumberOfHeatingElements = c.NumberOfHeatingElements,
                     SpecialFeatures = c.SpecialFeatures,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "Cookers").Result,
                     CategoryName = c.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "Cookers").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = ratedCookers,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
        }

        public async Task<ItemsDTO> GetLatestCookers(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Cooker>("Latest") / (double)pageSize);

            var latestCookers = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<Cooker>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(c => new CookerDTO
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    ModelName = c.ModelName,
                    Material = c.Material,
                    ItemWeight = c.ItemWeight,
                    Color = c.Color,
                    ItemDimensions = c.ItemDimensions,
                    DrawerType = c.DrawerType,
                    ControlsType = c.ControlsType,
                    FinishType = c.FinishType,
                    FormFactor = c.FormFactor,
                    NumberOfHeatingElements = c.NumberOfHeatingElements,
                    SpecialFeatures = c.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "Cookers").Result,
                    CategoryName = c.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "Cookers").Result.Count()
                });

            return new ItemsDTO
            {
                Items = latestCookers,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
        }

        public async Task<ItemsDTO> GetCookersWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Cooker>("Price", price1, price2, null) / (double)pageSize);

            var priceCookers = _servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<Cooker>(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(c => new CookerDTO
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    ModelName = c.ModelName,
                    Material = c.Material,
                    ItemWeight = c.ItemWeight,
                    Color = c.Color,
                    ItemDimensions = c.ItemDimensions,
                    DrawerType = c.DrawerType,
                    ControlsType = c.ControlsType,
                    FinishType = c.FinishType,
                    FormFactor = c.FormFactor,
                    NumberOfHeatingElements = c.NumberOfHeatingElements,
                    SpecialFeatures = c.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "Cookers").Result,
                    CategoryName = c.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(c.ID, "Cookers").Result.Count()
                });

            return new ItemsDTO
            {
                Items = priceCookers,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
        }

        public async Task<CookerDTO> GetCookerDetails(int id)
        {
            var cooker = await this.GetCooker(id);

            if (cooker != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "Cookers", "Default");

                var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "Cookers")).Count();

                var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<Cooker>(id, "Cookers");

                var totalQuantity = await _servicesInstanceProvider.GetCartServiceInstance().TotalItemQuantityInCart(id, "Cookers");

                var similarPriceCookers = (await _unitOfWork.Cookers.GetAll())
                    .Where(c => c.Price == cooker.Price || Math.Abs(c.Price - cooker.Price) <= 1000)
                    .Select(c => new CookerDTO
                    {
                        Id = c.ID,
                        Name = c.Name,
                        Rate = c.Rate,
                        Price = c.Price,
                        NewPrice = c.NewPrice ?? 0,
                        imageSrc = c.imageSrc,
                        ModelName = c.ModelName,
                        Material = c.Material,
                        ItemWeight = c.ItemWeight,
                        Color = c.Color,
                        ItemDimensions = c.ItemDimensions,
                        DrawerType = c.DrawerType,
                        ControlsType = c.ControlsType,
                        FinishType = c.FinishType,
                        FormFactor = c.FormFactor,
                        NumberOfHeatingElements = c.NumberOfHeatingElements,
                        SpecialFeatures = c.SpecialFeatures,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "Cookers").Result,
                        CategoryName = c.Category.Name,
                        RateCount = rateCount
                    });

                var relatedCookers = (await _unitOfWork.Cookers.GetAll())
                    .Where(c => c.CategoryId == cooker.CategoryId).Take(10)
                    .Select(c => new CookerDTO
                    {
                        Id = c.ID,
                        Name = c.Name,
                        Rate = c.Rate,
                        Price = c.Price,
                        NewPrice = c.NewPrice ?? 0,
                        imageSrc = c.imageSrc,
                        ModelName = c.ModelName,
                        Material = c.Material,
                        ItemWeight = c.ItemWeight,
                        Color = c.Color,
                        ItemDimensions = c.ItemDimensions,
                        DrawerType = c.DrawerType,
                        ControlsType = c.ControlsType,
                        FinishType = c.FinishType,
                        FormFactor = c.FormFactor,
                        NumberOfHeatingElements = c.NumberOfHeatingElements,
                        SpecialFeatures = c.SpecialFeatures,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "Cookers").Result,
                        CategoryName = c.Category.Name,
                        RateCount = rateCount
                    });

                var offers = _servicesInstanceProvider.GetOffersServiceInstance().GetOffers("Appliances", cooker.Category?.Name, cooker.ID);

                var discountValue = string.Empty;

                if (offers.Any())
                    discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                              $"{offers.First().PercentDiscount}%" :
                              offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;

                var BOGOGetItem = await _servicesInstanceProvider.GetOffersServiceInstance().GetBOGOGetItem(cooker);

                return new CookerDTO
                {
                    Id = cooker.ID,
                    Name = cooker.Name,
                    Rate = cooker.Rate,
                    Price = cooker.Price,
                    NewPrice = cooker.NewPrice ?? 0,
                    IsDiscounted = cooker.IsDiscounted,
                    DiscountValue = discountValue,
                    IsBOGOBuy = cooker.IsBOGOBuy,
                    IsBOGOGet = cooker.IsBOGOGet,
                    imageSrc = cooker.imageSrc,
                    ModelName = cooker.ModelName,
                    Material = cooker.Material,
                    ItemWeight = cooker.ItemWeight,
                    Color = cooker.Color,
                    ItemDimensions = cooker.ItemDimensions,
                    DrawerType = cooker.DrawerType,
                    ControlsType = cooker.ControlsType,
                    FinishType = cooker.FinishType,
                    FormFactor = cooker.FormFactor,
                    NumberOfHeatingElements = cooker.NumberOfHeatingElements,
                    SpecialFeatures = cooker.SpecialFeatures,
                    CategoryName = cooker.Category.Name,
                    RelatedCookers = relatedCookers,
                    SimilarPriceCookers = similarPriceCookers,
                    Comments = comments,
                    Offers = offers,
                    BOGOGet = BOGOGetItem,
                    StarCounts = starCounts,
                    RateCount = rateCount,
                    ControllerName = "Cookers",
                    TotalQuantity = totalQuantity
                };
            }

            else
                return null;
        }

        public async Task<CookerDTO> GetCookerAllComments(int id)
        {
            var Cooker = await this.GetCooker(id);

            var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "Cookers")).Count();

            var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<Cooker>(id, "Cookers");

            if (Cooker != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "Cookers", "All");

                if (comments.Any())
                {
                    return new CookerDTO
                    {
                        Id = Cooker.ID,
                        Name = Cooker.Name,
                        Rate = Cooker.Rate,
                        CategoryName = Cooker.Category.Name,
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
