using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class WashingMachinesService : IWashingMachinesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public WashingMachinesService(IUnitOfWork unitOfWork, IItemsService items, IUsersService userService, IWishingListService wishingList, ICartService carts, IOffersService offers, ICategoriesService categories, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<WashingMachine> GetWashingMachine(int id)
        {
            return await _unitOfWork.WashingMachines.GetById(id);
        }

        public IEnumerable<WashingMachine> GetWashingMachines(int pageNumber, int pageSize)
        {
            return _unitOfWork.WashingMachines.GetAll(pageNumber, pageSize);
        }

        public async Task<Result> Add(WashingMachine washingMachine)
        {
            var stream = new MemoryStream();
            await washingMachine.clientFile.CopyToAsync(stream);
            washingMachine.dbImage = stream.ToArray();

            var result = await _unitOfWork.WashingMachines.Add(washingMachine);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while adding." };
        }

        public async Task<Result> Update(WashingMachine washingMachine)
        {
            var stream = new MemoryStream();
            await washingMachine.clientFile.CopyToAsync(stream);
            washingMachine.dbImage = stream.ToArray();

            var result = _unitOfWork.WashingMachines.Update(washingMachine);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while updating." };
        }

        public async Task<Result> Delete(WashingMachine washingMachine)
        {
            var result = _unitOfWork.WashingMachines.Delete(washingMachine);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while deleting." };
        }

        public ItemDTO<WashingMachineDTO> GetWashingMachinesWithRelatedOnes()
        {
            var washingMachinesCategories = _servicesInstanceProvider.GetItemsServiceInstance().GetItemCategories<WashingMachine>();

            var discountedWashingMachines = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<WashingMachine>(1, 10, "ID", false).ToList().
                Select(w => new WashingMachineDTO
                {
                    Id = w.ID,
                    Name = w.Name,
                    Rate = w.Rate,
                    Price = w.Price,
                    NewPrice = w.NewPrice ?? 0,
                    imageSrc = w.imageSrc,
                    Capacity = w.Capacity,
                    Color = w.Color,
                    CycleOptions = w.CycleOptions,
                    ItemDimensions = w.ItemDimensions,
                    ItemWeight = w.ItemWeight,
                    SpecialFeatures = w.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), w.ID, "WashingMachines").Result,
                    CategoryName = w.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(w.ID, "WashingMachines").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var topRatedWashingMachines = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<WashingMachine>(1, 10, "ID", false).ToList().
                Select(w => new WashingMachineDTO
                {
                    Id = w.ID,
                    Name = w.Name,
                    Rate = w.Rate,
                    Price = w.Price,
                    NewPrice = w.NewPrice ?? 0,
                    imageSrc = w.imageSrc,
                    Capacity = w.Capacity,
                    Color = w.Color,
                    CycleOptions = w.CycleOptions,
                    ItemDimensions = w.ItemDimensions,
                    ItemWeight = w.ItemWeight,
                    SpecialFeatures = w.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), w.ID, "WashingMachines").Result,
                    CategoryName = w.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(w.ID, "WashingMachines").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var latestWashingMachines = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<WashingMachine>(1, 10, "ID", false).ToList().
                Select(w => new WashingMachineDTO
                {
                    Id = w.ID,
                    Name = w.Name,
                    Rate = w.Rate,
                    Price = w.Price,
                    NewPrice = w.NewPrice ?? 0,
                    imageSrc = w.imageSrc,
                    Capacity = w.Capacity,
                    Color = w.Color,
                    CycleOptions = w.CycleOptions,
                    ItemDimensions = w.ItemDimensions,
                    ItemWeight = w.ItemWeight,
                    SpecialFeatures = w.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), w.ID, "WashingMachines").Result,
                    CategoryName = w.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(w.ID, "WashingMachines").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            return new ItemDTO<WashingMachineDTO>()
            {
                ItemCategories = washingMachinesCategories,
                DiscountedItems = discountedWashingMachines,
                latestItems = latestWashingMachines,
                TopRatedItems = topRatedWashingMachines,
            };
        }

        public async Task<ItemsDTO> GetBrandsWashingMachines(string? orderIndex, int? page, string name, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<WashingMachine>("Brands", null, null, name) / (double)pageSize);

            var washingMachines = _servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<WashingMachine>(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                  Select(w => new WashingMachineDTO
                  {
                      Id = w.ID,
                      Name = w.Name,
                      Rate = w.Rate,
                      Price = w.Price,
                      NewPrice = w.NewPrice ?? 0,
                      imageSrc = w.imageSrc,
                      Capacity = w.Capacity,
                      Color = w.Color,
                      CycleOptions = w.CycleOptions,
                      ItemDimensions = w.ItemDimensions,
                      ItemWeight = w.ItemWeight,
                      SpecialFeatures = w.SpecialFeatures,
                      isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), w.ID, "WashingMachines").Result,
                      CategoryName = w.Category.Name,
                      RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(w.ID, "WashingMachines").Result.Count()
                  });

            return new ItemsDTO
            {
                Items = washingMachines,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Brands",
                Brand = name
            };
        }

        public async Task<ItemsDTO> GetDiscountedWashingMachines(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<WashingMachine>("Discounted") / (double)pageSize);

            var discountedWashingMachines = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<WashingMachine>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                  Select(w => new WashingMachineDTO
                  {
                      Id = w.ID,
                      Name = w.Name,
                      Rate = w.Rate,
                      Price = w.Price,
                      NewPrice = w.NewPrice ?? 0,
                      imageSrc = w.imageSrc,
                      Capacity = w.Capacity,
                      Color = w.Color,
                      CycleOptions = w.CycleOptions,
                      ItemDimensions = w.ItemDimensions,
                      ItemWeight = w.ItemWeight,
                      SpecialFeatures = w.SpecialFeatures,
                      isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), w.ID, "WashingMachines").Result,
                      CategoryName = w.Category.Name,
                      RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(w.ID, "WashingMachines").Result.Count()
                  });

            return new ItemsDTO
            {
                Items = discountedWashingMachines,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
        }

        public async Task<ItemsDTO> GetTopRatedWashingMachines(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<WashingMachine>("Rated") / (double)pageSize);


            var ratedWashingMachines = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<WashingMachine>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(w => new WashingMachineDTO
                 {
                     Id = w.ID,
                     Name = w.Name,
                     Rate = w.Rate,
                     Price = w.Price,
                     NewPrice = w.NewPrice ?? 0,
                     imageSrc = w.imageSrc,
                     Capacity = w.Capacity,
                     Color = w.Color,
                     CycleOptions = w.CycleOptions,
                     ItemDimensions = w.ItemDimensions,
                     ItemWeight = w.ItemWeight,
                     SpecialFeatures = w.SpecialFeatures,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), w.ID, "WashingMachines").Result,
                     CategoryName = w.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(w.ID, "WashingMachines").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = ratedWashingMachines,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
        }

        public async Task<ItemsDTO> GetLatestWashingMachines(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<WashingMachine>("Latest") / (double)pageSize);

            var latestWashingMachines = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<WashingMachine>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(w => new WashingMachineDTO
                 {
                     Id = w.ID,
                     Name = w.Name,
                     Rate = w.Rate,
                     Price = w.Price,
                     NewPrice = w.NewPrice ?? 0,
                     imageSrc = w.imageSrc,
                     Capacity = w.Capacity,
                     Color = w.Color,
                     CycleOptions = w.CycleOptions,
                     ItemDimensions = w.ItemDimensions,
                     ItemWeight = w.ItemWeight,
                     SpecialFeatures = w.SpecialFeatures,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), w.ID, "WashingMachines").Result,
                     CategoryName = w.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(w.ID, "WashingMachines").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = latestWashingMachines,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
        }

        public async Task<ItemsDTO> GetWashingMachinesWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<WashingMachine>("Price", price1, price2, null) / (double)pageSize);

            var priceWashingMachines = _servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<WashingMachine>(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(w => new WashingMachineDTO
                {
                    Id = w.ID,
                    Name = w.Name,
                    Rate = w.Rate,
                    Price = w.Price,
                    NewPrice = w.NewPrice ?? 0,
                    imageSrc = w.imageSrc,
                    Capacity = w.Capacity,
                    Color = w.Color,
                    CycleOptions = w.CycleOptions,
                    ItemDimensions = w.ItemDimensions,
                    ItemWeight = w.ItemWeight,
                    SpecialFeatures = w.SpecialFeatures,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), w.ID, "WashingMachines").Result,
                    CategoryName = w.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(w.ID, "WashingMachines").Result.Count()
                });

            return new ItemsDTO
            {
                Items = priceWashingMachines,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
        }

        public async Task<WashingMachineDTO> GetWashingMachineDetails(int id)
        {
            var washingMachine = await this.GetWashingMachine(id);

            if (washingMachine != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "WashingMachines", "Default");

                var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "WashingMachines")).Count();

                var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<WashingMachine>(id, "WashingMachines");

                var totalQuantity = await _servicesInstanceProvider.GetCartServiceInstance().TotalItemQuantityInCart(id, "WashingMachines");

                var similarPriceWashingMachines = (await _unitOfWork.WashingMachines.GetAll())
                    .Where(w => w.Price == washingMachine.Price || Math.Abs(w.Price - washingMachine.Price) <= 1000)
                    .Select(w => new WashingMachineDTO
                    {
                        Id = w.ID,
                        Name = w.Name,
                        Rate = w.Rate,
                        Price = w.Price,
                        NewPrice = w.NewPrice ?? 0,
                        imageSrc = w.imageSrc,
                        Capacity = w.Capacity,
                        Color = w.Color,
                        CycleOptions = w.CycleOptions,
                        ItemDimensions = w.ItemDimensions,
                        ItemWeight = w.ItemWeight,
                        SpecialFeatures = w.SpecialFeatures,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), w.ID, "WashingMachines").Result,
                        CategoryName = w.Category.Name,
                        RateCount = rateCount
                    });

                var relatedWashingMachines = (await _unitOfWork.WashingMachines.GetAll())
                    .Where(w => w.CategoryId == washingMachine.CategoryId).Take(10)
                    .Select(w => new WashingMachineDTO
                    {
                        Id = w.ID,
                        Name = w.Name,
                        Rate = w.Rate,
                        Price = w.Price,
                        NewPrice = w.NewPrice ?? 0,
                        imageSrc = w.imageSrc,
                        Capacity = w.Capacity,
                        Color = w.Color,
                        CycleOptions = w.CycleOptions,
                        ItemDimensions = w.ItemDimensions,
                        ItemWeight = w.ItemWeight,
                        SpecialFeatures = w.SpecialFeatures,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), w.ID, "WashingMachines").Result,
                        CategoryName = w.Category.Name,
                        RateCount = rateCount
                    });

                var offers = _servicesInstanceProvider.GetOffersServiceInstance().GetOffers("Appliances", washingMachine.Category?.Name, washingMachine.ID);

                var discountValue = string.Empty;

                if (offers.Any())
                    discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                              $"{offers.First().PercentDiscount}%" :
                              offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;

                var BOGOGetItem = await _servicesInstanceProvider.GetOffersServiceInstance().GetBOGOGetItem(washingMachine);

                return new WashingMachineDTO
                {
                    Id = washingMachine.ID,
                    Name = washingMachine.Name,
                    Rate = washingMachine.Rate,
                    Price = washingMachine.Price,
                    NewPrice = washingMachine.NewPrice ?? 0,
                    IsDiscounted = washingMachine.IsDiscounted,
                    DiscountValue = discountValue,
                    IsBOGOBuy = washingMachine.IsBOGOBuy,
                    IsBOGOGet = washingMachine.IsBOGOGet,
                    imageSrc = washingMachine.imageSrc,
                    Capacity = washingMachine.Capacity,
                    Color = washingMachine.Color,
                    CycleOptions = washingMachine.CycleOptions,
                    ItemDimensions = washingMachine.ItemDimensions,
                    ItemWeight = washingMachine.ItemWeight,
                    SpecialFeatures = washingMachine.SpecialFeatures,
                    CategoryName = washingMachine.Category.Name,
                    RelatedWashingMachines = relatedWashingMachines,
                    SimilarPriceWashingMachines = similarPriceWashingMachines,
                    Comments = comments,
                    Offers = offers,
                    BOGOGet = BOGOGetItem,
                    StarCounts = starCounts,
                    RateCount = rateCount,
                    ControllerName = "WashingMachines",
                    TotalQuantity = totalQuantity
                };
            }

            else
                return null;
        }

        public async Task<WashingMachineDTO> GetWashingMachineAllComments(int id)
        {
            var WashingMachine = await this.GetWashingMachine(id);

            var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "WashingMachines")).Count();

            var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<WashingMachine>(id, "WashingMachines");

            if (WashingMachine != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "WashingMachines", "All");

                if (comments.Any())
                {
                    return new WashingMachineDTO
                    {
                        Id = WashingMachine.ID,
                        Name = WashingMachine.Name,
                        Rate = WashingMachine.Rate,
                        CategoryName = WashingMachine.Category.Name,
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
