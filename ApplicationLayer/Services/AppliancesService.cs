using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class AppliancesService : IAppliancesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public AppliancesService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<IndexDTO> GetAllAppliances()
        {
            var categories = new List<Category>();

            var airConditionersCategories = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemsCategories("Appliances");
            var fridgesCategories = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemsCategories("Appliances");
            var cookersCategories = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemsCategories("Appliances");
            var washingMachinesCategories = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemsCategories("Appliances");

            categories.AddRange(airConditionersCategories);
            categories.AddRange(fridgesCategories);
            categories.AddRange(cookersCategories);
            categories.AddRange(washingMachinesCategories);

            categories = categories
                .DistinctBy(c => c.ID).ToList();

            var appliancesDepartmentName = (await _servicesInstanceProvider.GetDepartmentsServiceInstance().GetDepartment("Appliances")).Name;

            var offers = _servicesInstanceProvider.GetOffersServiceInstance().GetOffers(appliancesDepartmentName, null, null);

            return new IndexDTO()
            {
                Categories = categories,
                Offers = offers
            };
        }

        public async Task<ItemsDTO> GetBrandsAppliances(string? orderIndex, int? page, string name, bool? Des)
        {
            bool desOrder = Des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;

            var airConditionersTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<AirConditioner>("Brands", null, null, name) / (double)pageSize);
            var fridgesTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Fridge>("Brands", null, null, name) / (double)pageSize);
            var cookersTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Cooker>("Brands", null, null, name) / (double)pageSize);
            var washingMachinesTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<WashingMachine>("Brands", null, null, name) / (double)pageSize);

            var totalPages = airConditionersTotalPages + fridgesTotalPages + cookersTotalPages + washingMachinesTotalPages;

            var items = new List<dynamic>();

            var airConditioners = (_servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<AirConditioner>(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false)).ToList()
                .Select(a => new AirConditionerDTO
                {
                    Id = a.ID,
                    Capacity = a.Capacity,
                    Color = a.Color,
                    CategoryName = a.Category.Name,
                    Name = a.Name,
                    CoolingPower = a.CoolingPower,
                    imageSrc = a.imageSrc,
                    IsDiscounted = a.IsDiscounted,
                    ItemDimensions = a.ItemDimensions,
                    Rate = a.Rate,
                    Voltage = a.Voltage,
                    SpecialFeatures = a.SpecialFeatures,
                    NewPrice = a.NewPrice ?? 0,
                    Price = a.Price,
                    NoiseLevel = a.NoiseLevel,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), a.ID, "AirConditioners").Result,
                    ControllerName = "AirConditioners"
                });

            var fridges = (_servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<Fridge>(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false)).ToList()
                .Select(f => new FridgeDTO
                {
                    Id = f.ID,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    imageSrc = f.imageSrc,
                    IsDiscounted = f.IsDiscounted,
                    Capacity = f.Capacity,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    Rate = f.Rate,
                    Name = f.Name,
                    Price = f.Price,
                    NewPrice = f.NewPrice ?? 0,
                    SpecialFeatures = f.SpecialFeatures,
                    NumberOfDoors = f.NumberOfDoors,
                    CategoryName = f.Category.Name,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), f.ID, "AirConditioners").Result,
                    ControllerName = "Fridges"
                });

            var cookers = (_servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<Cooker>(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false)).ToList()
                .Select(c => new CookerDTO
                {
                    Id = c.ID,
                    Color = c.Color,
                    DrawerType = c.DrawerType,
                    ControlsType = c.ControlsType,
                    FinishType = c.FinishType,
                    FormFactor = c.FormFactor,
                    imageSrc = c.imageSrc,
                    ItemDimensions = c.ItemDimensions,
                    ItemWeight = c.ItemWeight,
                    IsDiscounted = c.IsDiscounted,
                    ModelName = c.ModelName,
                    Material = c.Material,
                    Name = c.Name,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    SpecialFeatures = c.SpecialFeatures,
                    Rate = c.Rate,
                    CategoryName = c.Category.Name,
                    NumberOfHeatingElements = c.NumberOfHeatingElements,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "AirConditioners").Result,
                    ControllerName = "Cookers"
                });

            var washingMachines = (_servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<WashingMachine>(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false)).ToList()
                .Select(w => new WashingMachineDTO
                {
                    Id = w.ID,
                    Color = w.Color,
                    Capacity = w.Capacity,
                    CycleOptions = w.CycleOptions,
                    imageSrc = w.imageSrc,
                    ItemDimensions = w.ItemDimensions,
                    IsDiscounted = w.IsDiscounted,
                    ItemWeight = w.ItemWeight,
                    Name = w.Name,
                    NewPrice = w.NewPrice ?? 0,
                    Price = w.Price,
                    Rate = w.Rate,
                    SpecialFeatures = w.SpecialFeatures,
                    CategoryName = w.Category.Name,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), w.ID, "AirConditioners").Result,
                    ControllerName = "WashingMachines"
                });

            items.AddRange(airConditioners);
            items.AddRange(fridges);
            items.AddRange(cookers);
            items.AddRange(washingMachines);

            return new ItemsDTO
            {
                Items = items.AsEnumerable(),
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                ActionName = "Brands",
                Brand = name
            };
        }

        public async Task<ItemsDTO> GetAppliancesWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? Des)
        {
            bool desOrder = Des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;

            var airConditionersTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<AirConditioner>("Price", price1, price2, null) / (double)pageSize);
            var fridgesTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Fridge>("Price", price1, price2, null) / (double)pageSize);
            var cookersTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<Cooker>("Price", price1, price2, null) / (double)pageSize);
            var washingMachinesTotalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<WashingMachine>("Price", price1, price2, null) / (double)pageSize);

            var totalPages = airConditionersTotalPages + fridgesTotalPages + cookersTotalPages + washingMachinesTotalPages;

            var items = new List<dynamic>();

            var airConditioners = (_servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<AirConditioner>(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false)).ToList()
                .Select( a => new AirConditionerDTO
                {
                    Id = a.ID,
                    Capacity = a.Capacity,
                    Color = a.Color,
                    CategoryName = a.Category.Name,
                    Name = a.Name,
                    CoolingPower = a.CoolingPower,
                    imageSrc = a.imageSrc,
                    IsDiscounted = a.IsDiscounted,
                    ItemDimensions = a.ItemDimensions,
                    Rate = a.Rate,
                    Voltage = a.Voltage,
                    SpecialFeatures = a.SpecialFeatures,
                    NewPrice = a.NewPrice ?? 0,
                    Price = a.Price,
                    NoiseLevel = a.NoiseLevel,
                    isLiked =  _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), a.ID, "AirConditioners").Result,
                    ControllerName = "AirConditioners"
                });

            var fridges = (_servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<Fridge>(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false)).ToList()
                .Select( f => new FridgeDTO
                {
                    Id = f.ID,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    imageSrc = f.imageSrc,
                    IsDiscounted = f.IsDiscounted,
                    Capacity = f.Capacity,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    Rate = f.Rate,
                    Name = f.Name,
                    Price = f.Price,
                    NewPrice = f.NewPrice ?? 0,
                    SpecialFeatures = f.SpecialFeatures,
                    NumberOfDoors = f.NumberOfDoors,
                    CategoryName = f.Category.Name,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), f.ID, "AirConditioners").Result,
                    ControllerName = "Fridges"
                });

            var cookers = (_servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<Cooker>(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false)).ToList()
                .Select( c => new CookerDTO
                {
                    Id = c.ID,
                    Color = c.Color,
                    DrawerType = c.DrawerType,
                    ControlsType = c.ControlsType,
                    FinishType = c.FinishType,
                    FormFactor = c.FormFactor,
                    imageSrc = c.imageSrc,
                    ItemDimensions = c.ItemDimensions,
                    ItemWeight = c.ItemWeight,
                    IsDiscounted = c.IsDiscounted,
                    ModelName = c.ModelName,
                    Material = c.Material,
                    Name = c.Name,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    SpecialFeatures = c.SpecialFeatures,
                    Rate = c.Rate,
                    CategoryName = c.Category.Name,
                    NumberOfHeatingElements = c.NumberOfHeatingElements,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), c.ID, "AirConditioners").Result  ,
                    ControllerName = "Cookers"
                });

            var washingMachines = (_servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<WashingMachine>(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false)).ToList()
                .Select( w => new WashingMachineDTO
                {
                    Id = w.ID,
                    Color = w.Color,
                    Capacity = w.Capacity,
                    CycleOptions = w.CycleOptions,
                    imageSrc = w.imageSrc,
                    ItemDimensions = w.ItemDimensions,
                    IsDiscounted = w.IsDiscounted,
                    ItemWeight = w.ItemWeight,
                    Name = w.Name,
                    NewPrice = w.NewPrice ?? 0,
                    Price = w.Price,
                    Rate = w.Rate,
                    SpecialFeatures = w.SpecialFeatures,
                    CategoryName = w.Category.Name,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), w.ID, "AirConditioners").Result,
                    ControllerName = "WashingMachines"
                });

            items.AddRange(airConditioners);
            items.AddRange(fridges);
            items.AddRange(cookers);
            items.AddRange(washingMachines);

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
