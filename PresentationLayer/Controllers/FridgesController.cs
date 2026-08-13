using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Models;
using InfrastructureLayer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresentationLayer.ViewModels;
using PresentationLayer.ViewModels.ItemVMs;
using Stripe;
using System.Drawing;

namespace PresentationLayer.Controllers
{
    [AllowAnonymous]
    public class FridgesController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _fridge.GetSpecificCategoriesForSelectList();

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IFridgesService _fridge;
        private readonly IDepartmentsService _departments;

        public FridgesController(IFridgesService fridge, IDepartmentsService departments)
        {
            _fridge = fridge;
            _departments = departments;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = _fridge.GetFridgesWithRelatedOnes();

            var fridgesVM = new ItemViewModel<FridgeViewModel>()
            {
                ItemCategories = result.ItemCategories,
                DiscountedItems = result.DiscountedItems
                .Select(f => new FridgeViewModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = f.isLiked,
                    CategoryName = f.CategoryName,
                    RateCount = f.RateCount
                }),
                latestItems = result.latestItems
                .Select(f => new FridgeViewModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = f.isLiked,
                    CategoryName = f.CategoryName,
                    RateCount = f.RateCount
                }),
                TopRatedItems = result.TopRatedItems
                .Select(f => new FridgeViewModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = f.isLiked,
                    CategoryName = f.CategoryName,
                    RateCount = f.RateCount
                }),
                Offers = result.Offers ?? Enumerable.Empty<Offer>().AsQueryable()
            };

            return View(fridgesVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var fridges = _fridge.GetFridges(pageNumber, pageSize);

            return View(fridges);
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
        public async Task<IActionResult> Add(Fridge data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _fridge.Add(data);

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

            var Fridge = await _fridge.GetFridge(id);

            if (Fridge != null)
            {
                await CreateCategoriesSelectList();
                return View(Fridge);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(Fridge data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _fridge.Update(data);

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

            var Fridge = await _fridge.GetFridge(id);

            if (Fridge != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(Fridge data)
        {
            await _fridge.Delete(data);

            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> Fridges()
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

                var result = await _fridge.GetBrandsFridges(orderIndex, page, name, des);

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

                return View("Fridges", data);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Discounted(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _fridge.GetDiscountedFridges(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("Fridges", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _fridge.GetTopRatedFridges(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("Fridges", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _fridge.GetLatestFridges(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("Fridges", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _fridge.GetFridgesWithPriceFilter(orderIndex, page, price1, price2, des);

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

            return View("Fridges", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _fridge.GetFridgeDetails(id);

                if (result != null)
                {
                    var fridges = new FridgeViewModel
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
                        Capacity = result.Capacity,
                        Color = result.Color,
                        DefrostSystem = result.DefrostSystem,
                        EnergyStar = result.EnergyStar,
                        InstallationType = result.InstallationType,
                        ItemDimensions = result.ItemDimensions,
                        NumberOfDoors = result.NumberOfDoors,
                        SpecialFeatures = result.SpecialFeatures,
                        CategoryName = result.CategoryName,
                        RelatedFridges = result.RelatedFridges
                        .Select(f => new FridgeViewModel
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Rate = f.Rate,
                            Price = f.Price,
                            NewPrice = f.NewPrice,
                            imageSrc = f.imageSrc,
                            Capacity = f.Capacity,
                            Color = f.Color,
                            DefrostSystem = f.DefrostSystem,
                            EnergyStar = f.EnergyStar,
                            InstallationType = f.InstallationType,
                            ItemDimensions = f.ItemDimensions,
                            NumberOfDoors = f.NumberOfDoors,
                            SpecialFeatures = f.SpecialFeatures,
                            isLiked = f.isLiked,
                            CategoryName = f.CategoryName,
                            RateCount = f.RateCount
                        }),
                        SimilarPriceFridges = result.SimilarPriceFridges
                        .Select(f => new FridgeViewModel
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Rate = f.Rate,
                            Price = f.Price,
                            NewPrice = f.NewPrice,
                            imageSrc = f.imageSrc,
                            Capacity = f.Capacity,
                            Color = f.Color,
                            DefrostSystem = f.DefrostSystem,
                            EnergyStar = f.EnergyStar,
                            InstallationType = f.InstallationType,
                            ItemDimensions = f.ItemDimensions,
                            NumberOfDoors = f.NumberOfDoors,
                            SpecialFeatures = f.SpecialFeatures,
                            isLiked = f.isLiked,
                            CategoryName = f.CategoryName,
                            RateCount = f.RateCount
                        }),
                        Comments = result.Comments,
                        Offers = result.Offers,
                        BOGOGet = result.BOGOGet,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount,
                        ControllerName = result.ControllerName,
                        TotalQuantity = result.TotalQuantity
                    };

                    return View(fridges);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllFridgeComments(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _fridge.GetFridgeAllComments(id);

                if (result is not null)
                {
                    var fridge = new FridgeViewModel
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Rate = result.Rate,
                        CategoryName = result.CategoryName,
                        Comments = result.Comments,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount
                    };

                    return View("AllComments", fridge);
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}
