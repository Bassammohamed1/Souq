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
    public class ElectronicsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public ElectronicsController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var categories = new List<Category>();

            var tvsCategories = await _unitOfWork.TVs.GetItemsCategories("Electronics").Result.ToListAsync();
            var laptopsCategories = await _unitOfWork.Laptops.GetItemsCategories("Electronics").Result.ToListAsync();
            var headphonesCategories = await _unitOfWork.HeadPhones.GetItemsCategories("Electronics").Result.ToListAsync();

            categories.AddRange(tvsCategories);
            categories.AddRange(laptopsCategories);
            categories.AddRange(headphonesCategories);

            categories = categories.DistinctBy(c => c.ID).ToList();

            var electronicsDepartment = _unitOfWork.Departments.GetAllWithoutPagination().Result
               .Where(d => d.Name == "Electronics").FirstOrDefault();

            var offers = await _unitOfWork.Offers.GetOffers(electronicsDepartment.Name, null, null);

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

                var laptopsTotalPages = (int)Math.Ceiling(await _unitOfWork.Laptops.TotalItems("Brands", null, null, name) / (double)pageSize);
                var tvsTotalPages = (int)Math.Ceiling(await _unitOfWork.TVs.TotalItems("Brands", null, null, name) / (double)pageSize);
                var headphonesTotalPages = (int)Math.Ceiling(await _unitOfWork.HeadPhones.TotalItems("Brands", null, null, name) / (double)pageSize);

                var totalPages = laptopsTotalPages + tvsTotalPages + headphonesTotalPages;

                var items = new List<dynamic>();

                var laptops = _unitOfWork.Laptops.GetCategoryItems(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                    .Result.Select(l => new LaptopViewModel
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
                        isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, l.ID, "Laptops"),
                        ControllerName = "Laptops"
                    });

                var tvs = _unitOfWork.TVs.GetCategoryItems(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                    .Result.Select(t => new TVViewModel
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
                        isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, t.ID, "TVs"),
                        ControllerName = "TVs"
                    });

                var headphones = _unitOfWork.HeadPhones.GetCategoryItems(name, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                    .Result.Select(h => new HeadPhoneViewModel
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
                        isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, h.ID, "HeadPhones"),
                        ControllerName = "HeadPhones"
                    });

                items.AddRange(laptops);
                items.AddRange(tvs);
                items.AddRange(headphones);

                var itemsEnumerable1 = items.AsEnumerable();

                var data = new ItemsViewModel
                {
                    Items = itemsEnumerable1.OrderByDescending(i => i.Price),
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    ActionName = "Brands",
                    Brand = name
                };
                return View("Electronics", data);
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

            var laptopsTotalPages = (int)Math.Ceiling(await _unitOfWork.Laptops.TotalItems("Price", price1, price2, null) / (double)pageSize);
            var tvsTotalPages = (int)Math.Ceiling(await _unitOfWork.TVs.TotalItems("Price", price1, price2, null) / (double)pageSize);
            var headphonesTotalPages = (int)Math.Ceiling(await _unitOfWork.HeadPhones.TotalItems("Price", price1, price2, null) / (double)pageSize);

            var totalPages = laptopsTotalPages + tvsTotalPages + headphonesTotalPages;

            var items = new List<dynamic>();

            var laptops = _unitOfWork.Laptops.GetItemsFilteredByPrice(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                 .Result.Select(l => new LaptopViewModel
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
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, l.ID, "Laptops"),
                     ControllerName = "Laptops"
                 });

            var tvs = _unitOfWork.TVs.GetItemsFilteredByPrice(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                 .Result.Select(t => new TVViewModel
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
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, t.ID, "TVs"),
                     ControllerName = "TVs"
                 });

            var headphones = _unitOfWork.HeadPhones.GetItemsFilteredByPrice(price1, price2, pageNumber, 3, orderIndex ?? "ID", Des ?? false)
                 .Result.Select(h => new HeadPhoneViewModel
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
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, h.ID, "HeadPhones"),
                     ControllerName = "HeadPhones"
                 });

            items.AddRange(laptops);
            items.AddRange(tvs);
            items.AddRange(headphones);

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
            return View("Electronics", data);
        }
    }
}