using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresentationLayer.ViewModels;
using PresentationLayer.ViewModels.ItemVMs;

namespace PresentationLayer.Controllers
{
    [AllowAnonymous]
    public class LaptopsController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _laptops.GetSpecificCategoriesForSelectList();

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly ILaptopsService _laptops;
        private readonly IDepartmentsService _departments;

        public LaptopsController(ILaptopsService laptops, IDepartmentsService departments)
        {
            _laptops = laptops;
            _departments = departments;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = _laptops.GetLaptopsWithRelatedOnes();

            var laptopsVM = new ItemViewModel<LaptopViewModel>()
            {
                ItemCategories = result.ItemCategories,
                DiscountedItems = result.DiscountedItems
                .Select(l => new LaptopViewModel
                {
                    Id = l.Id,
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
                    isLiked = l.isLiked,
                    CategoryName = l.CategoryName,
                    RateCount = l.RateCount
                }),
                latestItems = result.latestItems
                .Select(l => new LaptopViewModel
                {
                    Id = l.Id,
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
                    isLiked = l.isLiked,
                    CategoryName = l.CategoryName,
                    RateCount = l.RateCount
                }),
                TopRatedItems = result.TopRatedItems
                .Select(l => new LaptopViewModel
                {
                    Id = l.Id,
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
                    isLiked = l.isLiked,
                    CategoryName = l.CategoryName,
                    RateCount = l.RateCount
                }),
                Offers = result.Offers ?? Enumerable.Empty<Offer>().AsQueryable()
            };

            return View(laptopsVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var laptops = _laptops.GetLaptops(pageNumber, pageSize);

            return View(laptops);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            await CreateCategoriesSelectList();

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Add(Laptop data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _laptops.Add(data);

                return RedirectToAction(nameof(IndexAdmin));
            }

            return View(data);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id == null && id != 0)
                throw new ArgumentNullException("Invalid id!!");

            var Laptop = await _laptops.GetLaptop(id);

            if (Laptop != null)
            {
                await CreateCategoriesSelectList();
                return View(Laptop);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(Laptop data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _laptops.Update(data);

                return RedirectToAction(nameof(IndexAdmin));
            }

            return View(data);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id == null && id != 0)
                throw new ArgumentNullException("Invalid id!!");

            var Laptop = await _laptops.GetLaptop(id);

            if (Laptop != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(Laptop data)
        {
            await _laptops.Delete(data);

            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> Laptops()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            return View();
        }

        public async Task<IActionResult> Brands(string? orderIndex, int? page, string name, bool? des)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var departments = await _departments.GetDepartments();
                ViewData["Departments"] = departments;

                var result = await _laptops.GetBrandsLaptops(orderIndex, page, name, des);

                var data = new ItemsViewModel
                {
                    Items = result.Items,
                    CurrentPage = result.CurrentPage,
                    TotalPages = result.TotalPages,
                    OrderIndex = result.OrderIndex,
                    Des = result.Des,
                    ActionName = result.ActionName,
                    Brand = result.Brand
                };

                return View("Laptops", data);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Discounted(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _laptops.GetDiscountedLaptops(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("Laptops", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _laptops.GetTopRatedLaptops(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("Laptops", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _laptops.GetLatestLaptops(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("Laptops", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _laptops.GetLaptopsWithPriceFilter(orderIndex, page, price1, price2, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
                Price1 = result.Price1,
                Price2 = result.Price2
            };

            return View("Laptops", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _laptops.GetLaptopDetails(id);

                if (result != null)
                {
                    var laptops = new LaptopViewModel
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Rate = result.Rate,
                        Price = result.Price,
                        NewPrice = result.NewPrice,
                        IsDiscounted = result.IsDiscounted,
                        DiscountValue = result.DiscountValue,
                        IsBOGOBuy = result.IsBOGOBuy,
                        IsBOGOGet = result.IsBOGOGet,
                        imageSrc = result.imageSrc,
                        Color = result.Color,
                        CPU = result.CPU,
                        GPU = result.GPU,
                        HardDiskDescription = result.HardDiskDescription,
                        HardDiskSize = result.HardDiskSize,
                        ModelName = result.ModelName,
                        RAM = result.RAM,
                        OperatingSystem = result.OperatingSystem,
                        ScreenSize = result.ScreenSize,
                        CategoryName = result.CategoryName,
                        RelatedLaptops = result.RelatedLaptops
                        .Select(l => new LaptopViewModel
                        {
                            Id = l.Id,
                            Name = l.Name,
                            Rate = l.Rate,
                            Price = l.Price,
                            NewPrice = l.NewPrice,
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
                            isLiked = l.isLiked,
                            CategoryName = l.CategoryName,
                            RateCount = l.RateCount
                        }),
                        SimilarPriceLaptops = result.SimilarPriceLaptops
                           .Select(l => new LaptopViewModel
                           {
                               Id = l.Id,
                               Name = l.Name,
                               Rate = l.Rate,
                               Price = l.Price,
                               NewPrice = l.NewPrice,
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
                               isLiked = l.isLiked,
                               CategoryName = l.CategoryName,
                               RateCount = l.RateCount
                           }),
                        Comments = result.Comments,
                        Offers = result.Offers,
                        BOGOGet = result.BOGOGet,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount,
                        ControllerName = result.ControllerName,
                        TotalQuantity = result.TotalQuantity
                    };

                    return View(laptops);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllLaptopComments(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _laptops.GetLaptopAllComments(id);

                if (result is not null)
                {
                    var laptop = new LaptopViewModel
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Rate = result.Rate,
                        CategoryName = result.CategoryName,
                        Comments = result.Comments,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount
                    };

                    return View("AllComments", laptop);
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}