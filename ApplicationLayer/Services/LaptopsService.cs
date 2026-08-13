using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class LaptopsService : ILaptopsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public LaptopsService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<Laptop> GetLaptop(int id)
        {
            return await _unitOfWork.Laptops.GetById(id);
        }

        public IEnumerable<Laptop> GetLaptops(int pageNumber, int pageSize)
        {
            return _unitOfWork.Laptops.GetAll(pageNumber, pageSize);
        }

        public async Task<Result> Add(Laptop laptop)
        {
            var stream = new MemoryStream();
            await laptop.clientFile.CopyToAsync(stream);
            laptop.dbImage = stream.ToArray();

            var result = await _unitOfWork.Laptops.Add(laptop);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while adding." };
        }

        public async Task<Result> Update(Laptop laptop)
        {
            var stream = new MemoryStream();
            await laptop.clientFile.CopyToAsync(stream);
            laptop.dbImage = stream.ToArray();

            var result = _unitOfWork.Laptops.Update(laptop);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while updating." };
        }

        public async Task<Result> Delete(Laptop laptop)
        {
            var result = _unitOfWork.Laptops.Delete(laptop);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while deleting." };
        }

        public ItemDTO<LaptopDTO> GetLaptopsWithRelatedOnes()
        {
            var laptopsCategories = _servicesInstanceProvider.GetItemsServiceInstance().GetItemCategories<Laptop>();

            var discountedLaptops = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<Laptop>(1, 10, "ID", false).ToList().
                Select(l => new LaptopDTO
                {
                    Id = l.ID,
                    Name = l.Name,
                    Rate = l.Rate,
                    Price = l.Price,
                    NewPrice = l.NewPrice ?? 0,
                    imageSrc = l.imageSrc,
                    Color = l.Color,
                    CPU = l.CPU,
                    GPU = l.GPU,
                    HardDiskDescription = l.HardDiskDescription,
                    HardDiskSize = l.HardDiskSize,
                    ModelName = l.ModelName,
                    RAM = l.RAM,
                    OperatingSystem = l.OperatingSystem,
                    ScreenSize = l.ScreenSize,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), l.ID, "Laptops").Result,
                    CategoryName = l.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(l.ID, "Laptops").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var topRatedLaptops = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<Laptop>(1, 10, "ID", false).ToList().
                Select(l => new LaptopDTO
                {
                    Id = l.ID,
                    Name = l.Name,
                    Rate = l.Rate,
                    Price = l.Price,
                    NewPrice = l.NewPrice ?? 0,
                    imageSrc = l.imageSrc,
                    Color = l.Color,
                    CPU = l.CPU,
                    GPU = l.GPU,
                    HardDiskDescription = l.HardDiskDescription,
                    HardDiskSize = l.HardDiskSize,
                    ModelName = l.ModelName,
                    RAM = l.RAM,
                    OperatingSystem = l.OperatingSystem,
                    ScreenSize = l.ScreenSize,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), l.ID, "Laptops").Result,
                    CategoryName = l.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(l.ID, "Laptops").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var latestLaptops = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<Laptop>(1, 10, "ID", false).ToList().
                Select(l => new LaptopDTO
                {
                    Id = l.ID,
                    Name = l.Name,
                    Rate = l.Rate,
                    Price = l.Price,
                    NewPrice = l.NewPrice ?? 0,
                    imageSrc = l.imageSrc,
                    Color = l.Color,
                    CPU = l.CPU,
                    GPU = l.GPU,
                    HardDiskDescription = l.HardDiskDescription,
                    HardDiskSize = l.HardDiskSize,
                    ModelName = l.ModelName,
                    RAM = l.RAM,
                    OperatingSystem = l.OperatingSystem,
                    ScreenSize = l.ScreenSize,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), l.ID, "Laptops").Result,
                    CategoryName = l.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(l.ID, "Laptops").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            return new ItemDTO<LaptopDTO>()
            {
                ItemCategories = laptopsCategories,
                DiscountedItems = discountedLaptops,
                latestItems = latestLaptops,
                TopRatedItems = topRatedLaptops,
            };
        }

        public async Task<ItemsDTO> GetBrandsLaptops(string? orderIndex, int? page, string name, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Laptop>("Brands", null, null, name) / (double)pageSize);

            var laptops = _servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<Laptop>(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(l => new LaptopDTO
                {
                    Id = l.ID,
                    Name = l.Name,
                    Rate = l.Rate,
                    Price = l.Price,
                    NewPrice = l.NewPrice ?? 0,
                    imageSrc = l.imageSrc,
                    Color = l.Color,
                    CPU = l.CPU,
                    GPU = l.GPU,
                    HardDiskDescription = l.HardDiskDescription,
                    HardDiskSize = l.HardDiskSize,
                    ModelName = l.ModelName,
                    RAM = l.RAM,
                    OperatingSystem = l.OperatingSystem,
                    ScreenSize = l.ScreenSize,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), l.ID, "Laptops").Result,
                    CategoryName = l.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(l.ID, "Laptops").Result.Count()
                });

            return new ItemsDTO
            {
                Items = laptops,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Brands",
                Brand = name
            };
        }

        public async Task<ItemsDTO> GetDiscountedLaptops(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Laptop>("Discounted") / (double)pageSize);

            var discountedLaptops = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<Laptop>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(l => new LaptopDTO
                {
                    Id = l.ID,
                    Name = l.Name,
                    Rate = l.Rate,
                    Price = l.Price,
                    NewPrice = l.NewPrice ?? 0,
                    imageSrc = l.imageSrc,
                    Color = l.Color,
                    CPU = l.CPU,
                    GPU = l.GPU,
                    HardDiskDescription = l.HardDiskDescription,
                    HardDiskSize = l.HardDiskSize,
                    ModelName = l.ModelName,
                    RAM = l.RAM,
                    OperatingSystem = l.OperatingSystem,
                    ScreenSize = l.ScreenSize,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), l.ID, "Laptops").Result,
                    CategoryName = l.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(l.ID, "Laptops").Result.Count()
                });

            return new ItemsDTO
            {
                Items = discountedLaptops,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
        }

        public async Task<ItemsDTO> GetTopRatedLaptops(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Laptop>("Rated") / (double)pageSize);


            var ratedLaptops = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<Laptop>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(l => new LaptopDTO
                 {
                     Id = l.ID,
                     Name = l.Name,
                     Rate = l.Rate,
                     Price = l.Price,
                     NewPrice = l.NewPrice ?? 0,
                     imageSrc = l.imageSrc,
                     Color = l.Color,
                     CPU = l.CPU,
                     GPU = l.GPU,
                     HardDiskDescription = l.HardDiskDescription,
                     HardDiskSize = l.HardDiskSize,
                     ModelName = l.ModelName,
                     RAM = l.RAM,
                     OperatingSystem = l.OperatingSystem,
                     ScreenSize = l.ScreenSize,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), l.ID, "Laptops").Result,
                     CategoryName = l.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(l.ID, "Laptops").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = ratedLaptops,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
        }

        public async Task<ItemsDTO> GetLatestLaptops(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Laptop>("Latest") / (double)pageSize);

            var latestLaptops = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<Laptop>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(l => new LaptopDTO
                 {
                     Id = l.ID,
                     Name = l.Name,
                     Rate = l.Rate,
                     Price = l.Price,
                     NewPrice = l.NewPrice ?? 0,
                     imageSrc = l.imageSrc,
                     Color = l.Color,
                     CPU = l.CPU,
                     GPU = l.GPU,
                     HardDiskDescription = l.HardDiskDescription,
                     HardDiskSize = l.HardDiskSize,
                     ModelName = l.ModelName,
                     RAM = l.RAM,
                     OperatingSystem = l.OperatingSystem,
                     ScreenSize = l.ScreenSize,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), l.ID, "Laptops").Result,
                     CategoryName = l.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(l.ID, "Laptops").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = latestLaptops,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
        }

        public async Task<ItemsDTO> GetLaptopsWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Laptop>("Price", price1, price2, null) / (double)pageSize);

            var priceLaptops = _servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<Laptop>(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                 Select(l => new LaptopDTO
                 {
                     Id = l.ID,
                     Name = l.Name,
                     Rate = l.Rate,
                     Price = l.Price,
                     NewPrice = l.NewPrice ?? 0,
                     imageSrc = l.imageSrc,
                     Color = l.Color,
                     CPU = l.CPU,
                     GPU = l.GPU,
                     HardDiskDescription = l.HardDiskDescription,
                     HardDiskSize = l.HardDiskSize,
                     ModelName = l.ModelName,
                     RAM = l.RAM,
                     OperatingSystem = l.OperatingSystem,
                     ScreenSize = l.ScreenSize,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), l.ID, "Laptops").Result,
                     CategoryName = l.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(l.ID, "Laptops").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = priceLaptops,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
        }

        public async Task<LaptopDTO> GetLaptopDetails(int id)
        {
            var laptop = await this.GetLaptop(id);

            if (laptop != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "Laptops", "Default");

                var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "Laptops")).Count();

                var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<Laptop>(id, "Laptops");

                var totalQuantity = await _servicesInstanceProvider.GetCartServiceInstance().TotalItemQuantityInCart(id, "Laptops");

                var similarPriceLaptops = (await _unitOfWork.Laptops.GetAll())
                    .Where(l => l.Price == laptop.Price || Math.Abs(l.Price - laptop.Price) <= 1000)
                    .Select(l => new LaptopDTO
                    {
                        Id = l.ID,
                        Name = l.Name,
                        Rate = l.Rate,
                        Price = l.Price,
                        NewPrice = l.NewPrice ?? 0,
                        imageSrc = l.imageSrc,
                        Color = l.Color,
                        CPU = l.CPU,
                        GPU = l.GPU,
                        HardDiskDescription = l.HardDiskDescription,
                        HardDiskSize = l.HardDiskSize,
                        ModelName = l.ModelName,
                        RAM = l.RAM,
                        OperatingSystem = l.OperatingSystem,
                        ScreenSize = l.ScreenSize,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), l.ID, "Laptops").Result,
                        CategoryName = l.Category.Name,
                        RateCount = rateCount
                    });

                var relatedLaptops = (await _unitOfWork.Laptops.GetAll())
                    .Where(l => l.CategoryId == laptop.CategoryId).Take(10)
                    .Select(l => new LaptopDTO
                    {
                        Id = l.ID,
                        Name = l.Name,
                        Rate = l.Rate,
                        Price = l.Price,
                        NewPrice = l.NewPrice ?? 0,
                        imageSrc = l.imageSrc,
                        Color = l.Color,
                        CPU = l.CPU,
                        GPU = l.GPU,
                        HardDiskDescription = l.HardDiskDescription,
                        HardDiskSize = l.HardDiskSize,
                        ModelName = l.ModelName,
                        RAM = l.RAM,
                        OperatingSystem = l.OperatingSystem,
                        ScreenSize = l.ScreenSize,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), l.ID, "Laptops").Result,
                        CategoryName = l.Category.Name,
                        RateCount = rateCount
                    });

                var offers = _servicesInstanceProvider.GetOffersServiceInstance().GetOffers("Electronics", laptop.Category?.Name, laptop.ID);

                var discountValue = string.Empty;

                if (offers.Any())
                    discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                              $"{offers.First().PercentDiscount}%" :
                              offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;

                var BOGOGetItem = await _servicesInstanceProvider.GetOffersServiceInstance().GetBOGOGetItem(laptop);

                return new LaptopDTO
                {
                    Id = laptop.ID,
                    Name = laptop.Name,
                    Rate = laptop.Rate,
                    Price = laptop.Price,
                    NewPrice = laptop.NewPrice ?? 0,
                    IsDiscounted = laptop.IsDiscounted,
                    DiscountValue = discountValue,
                    IsBOGOBuy = laptop.IsBOGOBuy,
                    IsBOGOGet = laptop.IsBOGOGet,
                    imageSrc = laptop.imageSrc,
                    Color = laptop.Color,
                    CPU = laptop.CPU,
                    GPU = laptop.GPU,
                    HardDiskDescription = laptop.HardDiskDescription,
                    HardDiskSize = laptop.HardDiskSize,
                    ModelName = laptop.ModelName,
                    RAM = laptop.RAM,
                    OperatingSystem = laptop.OperatingSystem,
                    ScreenSize = laptop.ScreenSize,
                    CategoryName = laptop.Category.Name,
                    RelatedLaptops = relatedLaptops,
                    SimilarPriceLaptops = similarPriceLaptops,
                    Comments = comments,
                    Offers = offers,
                    BOGOGet = BOGOGetItem,
                    StarCounts = starCounts,
                    RateCount = rateCount,
                    ControllerName = "Laptops",
                    TotalQuantity = totalQuantity
                };
            }

            else
                return null;
        }

        public async Task<LaptopDTO> GetLaptopAllComments(int id)
        {
            var Laptop = await this.GetLaptop(id);

            var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "Laptops")).Count();

            var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<Laptop>(id, "Laptops");

            if (Laptop != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "Laptops", "All");

                if (comments.Any())
                {
                    return new LaptopDTO
                    {
                        Id = Laptop.ID,
                        Name = Laptop.Name,
                        Rate = Laptop.Rate,
                        CategoryName = Laptop.Category.Name,
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
