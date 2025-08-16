using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PresentationLayer.ViewModels;
using PresentationLayer.ViewModels.ItemVMs;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "User")]
    public class AppliancesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public AppliancesController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var categories = new List<Category>();

            var airConditionersCategories = await _unitOfWork.AirConditioners.GetItemsCategories("Appliances").Result.ToListAsync();
            var fridgesCategories = await _unitOfWork.Fridges.GetItemsCategories("Appliances").Result.ToListAsync();
            var cookersCategories = await _unitOfWork.Cookers.GetItemsCategories("Appliances").Result.ToListAsync();
            var washingMachinesCategories = await _unitOfWork.WashingMachines.GetItemsCategories("Appliances").Result.ToListAsync();

            categories.AddRange(airConditionersCategories);
            categories.AddRange(fridgesCategories);
            categories.AddRange(cookersCategories);
            categories.AddRange(washingMachinesCategories);

            categories = categories.DistinctBy(c => c.ID).ToList();

            var appliancesDepartment = _unitOfWork.Departments.GetAllWithoutPagination().Result
               .Where(d => d.Name == "Appliances").FirstOrDefault();

            var offers = await _unitOfWork.Offers.GetOffers(appliancesDepartment.Name, null, null);

            var indexVM = new IndexViewModel()
            {
                Categories = categories,
                Offers = offers
            };

            return View(indexVM);
        }

        public async Task<IActionResult> Brands(string? orderIndex, int? page, string name, bool? Des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (!string.IsNullOrEmpty(name))
            {
                bool desOrder = Des ?? false;
                int pageSize = 9;
                int pageNumber = page ?? 1;

                var airConditionersTotalPages = (int)Math.Ceiling(await _unitOfWork.AirConditioners.TotalItems("Brands", null, null, name) / (double)pageSize);
                var fridgesTotalPages = (int)Math.Ceiling(await _unitOfWork.Fridges.TotalItems("Brands", null, null, name) / (double)pageSize);
                var cookersTotalPages = (int)Math.Ceiling(await _unitOfWork.Cookers.TotalItems("Brands", null, null, name) / (double)pageSize);
                var washingMachinesTotalPages = (int)Math.Ceiling(await _unitOfWork.WashingMachines.TotalItems("Brands", null, null, name) / (double)pageSize);

                var totalPages = airConditionersTotalPages + fridgesTotalPages + cookersTotalPages + washingMachinesTotalPages;

                var items = new List<dynamic>();

                var airConditioners = _unitOfWork.AirConditioners.GetCategoryItems(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                    .Result.Select(a => new AirConditionerViewModel
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
                        isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, a.ID, "AirConditioners"),
                        ControllerName = "AirConditioners"
                    });

                var fridges = _unitOfWork.Fridges.GetCategoryItems(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                    .Result.Select(f => new FridgeViewModel
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
                        isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, f.ID, "Fridges"),
                        ControllerName = "Fridges"
                    });

                var cookers = _unitOfWork.Cookers.GetCategoryItems(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                    .Result.Select(c => new CookerViewModel
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
                        isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, c.ID, "Cookers"),
                        ControllerName = "Cookers"
                    });

                var washingMachines = _unitOfWork.WashingMachines.GetCategoryItems(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                    .Result.Select(w => new WashingMachineViewModel
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
                        isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, w.ID, "WashingMachines"),
                        ControllerName = "WashingMachines"
                    });

                items.AddRange(airConditioners);
                items.AddRange(fridges);
                items.AddRange(cookers);
                items.AddRange(washingMachines);

                var itemsEnumerable1 = items.AsEnumerable();

                var data = new ItemsViewModel
                {
                    Items = itemsEnumerable1.OrderByDescending(i => i.Price),
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    ActionName = "Brands",
                    Brand = name
                };
                return View("Appliances", data);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? Des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = Des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;

            var airConditionersTotalPages = (int)Math.Ceiling(await _unitOfWork.AirConditioners.TotalItems("Price", price1, price2, null) / (double)pageSize);
            var fridgesTotalPages = (int)Math.Ceiling(await _unitOfWork.Fridges.TotalItems("Price", price1, price2, null) / (double)pageSize);
            var cookersTotalPages = (int)Math.Ceiling(await _unitOfWork.Cookers.TotalItems("Price", price1, price2, null) / (double)pageSize);
            var washingMachinesTotalPages = (int)Math.Ceiling(await _unitOfWork.WashingMachines.TotalItems("Price", price1, price2, null) / (double)pageSize);

            var totalPages = airConditionersTotalPages + fridgesTotalPages + cookersTotalPages + washingMachinesTotalPages;

            var items = new List<dynamic>();

            var airConditioners = _unitOfWork.AirConditioners.GetItemsFilteredByPrice(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                 .Result.Select(a => new AirConditionerViewModel
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
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, a.ID, "AirConditioners"),
                     ControllerName = "AirConditioners"
                 });

            var fridges = _unitOfWork.Fridges.GetItemsFilteredByPrice(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                  .Result.Select(f => new FridgeViewModel
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
                      isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, f.ID, "Fridges"),
                      ControllerName = "Fridges"
                  });

            var cookers = _unitOfWork.Cookers.GetItemsFilteredByPrice(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                 .Result.Select(c => new CookerViewModel
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
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, c.ID, "Cookers"),
                     ControllerName = "Cookers"
                 });

            var washingMachines = _unitOfWork.WashingMachines.GetItemsFilteredByPrice(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                .Result.Select(w => new WashingMachineViewModel
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
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, w.ID, "WashingMachines"),
                    ControllerName = "WashingMachines"
                });

            items.AddRange(airConditioners);
            items.AddRange(fridges);
            items.AddRange(cookers);
            items.AddRange(washingMachines);

            var itemsEnumerable1 = items.AsEnumerable();

            var data = new ItemsViewModel
            {
                Items = itemsEnumerable1.OrderByDescending(p => p.Price),
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
            return View("Appliances", data);
        }
    }
}