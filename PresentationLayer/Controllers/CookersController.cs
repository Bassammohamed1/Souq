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
    public class CookersController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _cooker.GetSpecificCategoriesForSelectList();

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly ICookersService _cooker;
        private readonly IDepartmentsService _departments;

        public CookersController(ICookersService cooker, IDepartmentsService departments)
        {
            _cooker = cooker;
            _departments = departments;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = _cooker.GetCookersWithRelatedOnes();

            var cookersVM = new ItemViewModel<CookerViewModel>()
            {
                ItemCategories = result.ItemCategories,
                DiscountedItems = result.DiscountedItems
                .Select(c => new CookerViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice,
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
                    isLiked = c.isLiked,
                    CategoryName = c.CategoryName,
                    RateCount = c.RateCount
                }),
                latestItems = result.latestItems
                .Select(c => new CookerViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice,
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
                    isLiked = c.isLiked,
                    CategoryName = c.CategoryName,
                    RateCount = c.RateCount
                }),
                TopRatedItems = result.TopRatedItems
                .Select(c => new CookerViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice,
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
                    isLiked = c.isLiked,
                    CategoryName = c.CategoryName,
                    RateCount = c.RateCount
                }),
                Offers = result.Offers ?? Enumerable.Empty<Offer>().AsQueryable()
            };

            return View(cookersVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var cookers = _cooker.GetCookers(pageNumber, pageSize);

            return View(cookers);
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
        public async Task<IActionResult> Add(Cooker data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _cooker.Add(data);

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

            var Cooker = await _cooker.GetCooker(id);

            if (Cooker != null)
            {
                await CreateCategoriesSelectList();
                return View(Cooker);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(Cooker data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _cooker.Update(data);

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

            var Cooker = await _cooker.GetCooker(id);

            if (Cooker != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(Cooker data)
        {
            await _cooker.Delete(data);

            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> Cookers()
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

                var result = await _cooker.GetBrandsCookers(orderIndex, page, name, des);

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

                return View("Cookers", data);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Discounted(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _cooker.GetDiscountedCookers(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("Cookers", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _cooker.GetTopRatedCookers(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("Cookers", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _cooker.GetLatestCookers(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("Cookers", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _cooker.GetCookersWithPriceFilter(orderIndex, page, price1, price2, des);

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

            return View("Cookers", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _cooker.GetCookerDetails(id);

                if (result != null)
                {
                    var cookers = new CookerViewModel
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
                        ModelName = result.ModelName,
                        Material = result.Material,
                        ItemWeight = result.ItemWeight,
                        Color = result.Color,
                        ItemDimensions = result.ItemDimensions,
                        DrawerType = result.DrawerType,
                        ControlsType = result.ControlsType,
                        FinishType = result.FinishType,
                        FormFactor = result.FormFactor,
                        NumberOfHeatingElements = result.NumberOfHeatingElements,
                        SpecialFeatures = result.SpecialFeatures,
                        CategoryName = result.CategoryName,
                        RelatedCookers = result.RelatedCookers
                        .Select(c => new CookerViewModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Rate = c.Rate,
                            Price = c.Price,
                            NewPrice = c.NewPrice,
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
                            isLiked = c.isLiked,
                            CategoryName = c.CategoryName,
                            RateCount = c.RateCount
                        }),
                        SimilarPriceCookers = result.SimilarPriceCookers
                        .Select(c => new CookerViewModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Rate = c.Rate,
                            Price = c.Price,
                            NewPrice = c.NewPrice,
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
                            isLiked = c.isLiked,
                            CategoryName = c.CategoryName,
                            RateCount = c.RateCount
                        }),
                        Comments = result.Comments,
                        Offers = result.Offers,
                        BOGOGet = result.BOGOGet,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount,
                        ControllerName = result.ControllerName,
                        TotalQuantity = result.TotalQuantity
                    };

                    return View(cookers);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllCookerComments(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _cooker.GetCookerAllComments(id);

                if (result is not null)
                {
                    var cooker = new CookerViewModel
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Rate = result.Rate,
                        CategoryName = result.CategoryName,
                        Comments = result.Comments,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount
                    };

                    return View("AllComments", cooker);
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}