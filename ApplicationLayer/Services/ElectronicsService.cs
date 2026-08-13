using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class ElectronicsService : IElectronicsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public ElectronicsService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<IndexDTO> GetAllElectronics()
        {
            var categories = new List<Category>();

            var tvsCategories = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemsCategories("Electronics");
            var laptopsCategories = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemsCategories("Electronics");
            var headphonesCategories = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemsCategories("Electronics");

            categories.AddRange(tvsCategories);
            categories.AddRange(laptopsCategories);
            categories.AddRange(headphonesCategories);

            categories = categories.DistinctBy(c => c.ID).ToList();

            var electronicsDepartment = (await _servicesInstanceProvider.GetDepartmentsServiceInstance().GetDepartment("Electronics")).Name;

            var offers = _servicesInstanceProvider.GetOffersServiceInstance().GetOffers(electronicsDepartment, null, null);

            return new IndexDTO()
            {
                Categories = categories,
                Offers = offers
            };
        }

        public async Task<ItemsDTO> GetBrandsElectronics(string? orderIndex, int? page, string name, bool? Des)
        {
            bool desOrder = Des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;

            var laptopsTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Laptop>("Brands", null, null, name) / (double)pageSize);
            var tvsTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<TV>("Brands", null, null, name) / (double)pageSize);
            var headphonesTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<HeadPhone>("Brands", null, null, name) / (double)pageSize);

            var totalPages = laptopsTotalPages + tvsTotalPages + headphonesTotalPages;

            var items = new List<dynamic>();

            var laptops = _servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<Laptop>(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false).ToList()
                .Select(l => new LaptopDTO
                {
                    Id = l.ID,
                    Color = l.Color,
                    CPU = l.CPU,
                    GPU = l.GPU,
                    HardDiskDescription = l.HardDiskDescription,
                    HardDiskSize = l.HardDiskSize,
                    ModelName = l.ModelName,
                    OperatingSystem = l.OperatingSystem,
                    ScreenSize = l.ScreenSize,
                    Name = l.Name,
                    IsDiscounted = l.IsDiscounted,
                    imageSrc = l.imageSrc,
                    Price = l.Price,
                    NewPrice = l.NewPrice,
                    RAM = l.RAM,
                    Rate = l.Rate,
                    CategoryName = l.Category.Name,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), l.ID, "Laptops").Result,
                    ControllerName = "Laptops"
                });

            var tvs = _servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<TV>(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false).ToList()
                .Select(t => new TvDTO
                {
                    Id = t.ID,
                    ConnectivityTechnology = t.ConnectivityTechnology,
                    DisplayTechnology = t.DisplayTechnology,
                    imageSrc = t.imageSrc,
                    IsDiscounted = t.IsDiscounted,
                    Name = t.Name,
                    NewPrice = t.NewPrice,
                    ItemDimensions = t.ItemDimensions,
                    Price = t.Price,
                    Rate = t.Rate,
                    SpecialFeatures = t.SpecialFeatures,
                    Resolution = t.Resolution,
                    ScreenSize = t.ScreenSize,
                    RefreshRate = t.RefreshRate,
                    CategoryName = t.Category.Name,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "TVs").Result,
                    ControllerName = "TVs"
                });

            var headphones = (_servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<HeadPhone>(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false)).ToList()
                .Select(h => new HeadPhoneDTO
                {
                    Id = h.ID,
                    Color = h.Color,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Name = h.Name,
                    imageSrc = h.imageSrc,
                    IsDiscounted = h.IsDiscounted,
                    NewPrice = h.NewPrice,
                    Rate = h.Rate,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    NoiseControl = h.NoiseControl,
                    Price = h.Price,
                    ModelName = h.ModelName,
                    CategoryName = h.Category.Name,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), h.ID, "HeadPhones").Result,
                    ControllerName = "HeadPhones"
                });

            items.AddRange(laptops);
            items.AddRange(tvs);
            items.AddRange(headphones);

            return new ItemsDTO
            {
                Items = items.AsEnumerable(),
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                ActionName = "Brands",
                Brand = name
            };
        }

        public async Task<ItemsDTO> GetElectronicsWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? Des)
        {
            bool desOrder = Des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;

            var laptopsTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Laptop>("Price", price1, price2, null) / (double)pageSize);
            var tvsTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<TV>("Price", price1, price2, null) / (double)pageSize);
            var headphonesTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<HeadPhone>("Price", price1, price2, null) / (double)pageSize);

            var totalPages = laptopsTotalPages + tvsTotalPages + headphonesTotalPages;

            var items = new List<dynamic>();

            var laptops = _servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<Laptop>(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false).ToList()
                 .Select(l => new LaptopDTO
                 {
                     Id = l.ID,
                     Color = l.Color,
                     CPU = l.CPU,
                     GPU = l.GPU,
                     HardDiskDescription = l.HardDiskDescription,
                     HardDiskSize = l.HardDiskSize,
                     ModelName = l.ModelName,
                     OperatingSystem = l.OperatingSystem,
                     ScreenSize = l.ScreenSize,
                     Name = l.Name,
                     IsDiscounted = l.IsDiscounted,
                     imageSrc = l.imageSrc,
                     Price = l.Price,
                     NewPrice = l.NewPrice,
                     RAM = l.RAM,
                     Rate = l.Rate,
                     CategoryName = l.Category.Name,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), l.ID, "Laptops").Result,
                     ControllerName = "Laptops"
                 });

            var tvs = (_servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<TV>(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false)).ToList()
                 .Select(t => new TvDTO
                 {
                     Id = t.ID,
                     ConnectivityTechnology = t.ConnectivityTechnology,
                     DisplayTechnology = t.DisplayTechnology,
                     imageSrc = t.imageSrc,
                     IsDiscounted = t.IsDiscounted,
                     Name = t.Name,
                     NewPrice = t.NewPrice,
                     ItemDimensions = t.ItemDimensions,
                     Price = t.Price,
                     Rate = t.Rate,
                     SpecialFeatures = t.SpecialFeatures,
                     Resolution = t.Resolution,
                     ScreenSize = t.ScreenSize,
                     RefreshRate = t.RefreshRate,
                     CategoryName = t.Category.Name,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), t.ID, "TVs").Result,
                     ControllerName = "TVs"
                 });

            var headphones = _servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<HeadPhone>(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false).ToList()
                 .Select(h => new HeadPhoneDTO
                 {
                     Id = h.ID,
                     Color = h.Color,
                     ConnectivityTechnology = h.ConnectivityTechnology,
                     Name = h.Name,
                     imageSrc = h.imageSrc,
                     IsDiscounted = h.IsDiscounted,
                     NewPrice = h.NewPrice,
                     Rate = h.Rate,
                     HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                     HeadphonesFormFactor = h.HeadphonesFormFactor,
                     NoiseControl = h.NoiseControl,
                     Price = h.Price,
                     ModelName = h.ModelName,
                     CategoryName = h.Category.Name,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), h.ID, "HeadPhones").Result,
                     ControllerName = "HeadPhones"
                 });

            items.AddRange(laptops);
            items.AddRange(tvs);
            items.AddRange(headphones);

            return new ItemsDTO
            {
                Items = items.AsEnumerable(),
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
        }
    }
}
