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
    public class WashingMachinesController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _washingMachine.GetSpecificCategoriesForSelectList();

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IWashingMachinesService _washingMachine;
        private readonly IDepartmentsService _departments;

        public WashingMachinesController(IWashingMachinesService washingMachine, IDepartmentsService departments)
        {
            _washingMachine = washingMachine;
            _departments = departments;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = _washingMachine.GetWashingMachinesWithRelatedOnes();

            var washingMachinesVM = new ItemViewModel<WashingMachineViewModel>()
            {
                ItemCategories = result.ItemCategories,
                DiscountedItems = result.DiscountedItems
                .Select(w => new WashingMachineViewModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Rate = w.Rate,
                    Price = w.Price,
                    NewPrice = w.NewPrice,
                    imageSrc = w.imageSrc,
                    Capacity = w.Capacity,
                    Color = w.Color,
                    CycleOptions = w.CycleOptions,
                    ItemDimensions = w.ItemDimensions,
                    ItemWeight = w.ItemWeight,
                    SpecialFeatures = w.SpecialFeatures,
                    isLiked = w.isLiked,
                    CategoryName = w.CategoryName,
                    RateCount = w.RateCount
                }),
                latestItems = result.latestItems
                .Select(w => new WashingMachineViewModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Rate = w.Rate,
                    Price = w.Price,
                    NewPrice = w.NewPrice,
                    imageSrc = w.imageSrc,
                    Capacity = w.Capacity,
                    Color = w.Color,
                    CycleOptions = w.CycleOptions,
                    ItemDimensions = w.ItemDimensions,
                    ItemWeight = w.ItemWeight,
                    SpecialFeatures = w.SpecialFeatures,
                    isLiked = w.isLiked,
                    CategoryName = w.CategoryName,
                    RateCount = w.RateCount
                }),
                TopRatedItems = result.TopRatedItems
                .Select(w => new WashingMachineViewModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Rate = w.Rate,
                    Price = w.Price,
                    NewPrice = w.NewPrice,
                    imageSrc = w.imageSrc,
                    Capacity = w.Capacity,
                    Color = w.Color,
                    CycleOptions = w.CycleOptions,
                    ItemDimensions = w.ItemDimensions,
                    ItemWeight = w.ItemWeight,
                    SpecialFeatures = w.SpecialFeatures,
                    isLiked = w.isLiked,
                    CategoryName = w.CategoryName,
                    RateCount = w.RateCount
                }),
                Offers = result.Offers ?? Enumerable.Empty<Offer>().AsQueryable()
            };

            return View(washingMachinesVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var washingMachines = _washingMachine.GetWashingMachines(pageNumber, pageSize);

            return View(washingMachines);
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
        public async Task<IActionResult> Add(WashingMachine data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _washingMachine.Add(data);

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

            var WashingMachine = await _washingMachine.GetWashingMachine(id);

            if (WashingMachine != null)
            {
                await CreateCategoriesSelectList();
                return View(WashingMachine);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(WashingMachine data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _washingMachine.Update(data);

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

            var WashingMachine = await _washingMachine.GetWashingMachine(id);

            if (WashingMachine != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(WashingMachine data)
        {
            await _washingMachine.Delete(data);

            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> WashingMachines()
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

                var result = await _washingMachine.GetBrandsWashingMachines(orderIndex, page, name, des);

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

                return View("WashingMachines", data);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Discounted(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _washingMachine.GetDiscountedWashingMachines(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("WashingMachines", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _washingMachine.GetTopRatedWashingMachines(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("WashingMachines", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _washingMachine.GetLatestWashingMachines(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("WashingMachines", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _washingMachine.GetWashingMachinesWithPriceFilter(orderIndex, page, price1, price2, des);

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

            return View("WashingMachines", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _washingMachine.GetWashingMachineDetails(id);

                if (result != null)
                {
                    var washingMachines = new WashingMachineViewModel
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
                        CycleOptions = result.CycleOptions,
                        ItemDimensions = result.ItemDimensions,
                        ItemWeight = result.ItemWeight,
                        SpecialFeatures = result.SpecialFeatures,
                        CategoryName = result.CategoryName,
                        RelatedWashingMachines = result.RelatedWashingMachines
                        .Select(w => new WashingMachineViewModel
                        {
                            Id = w.Id,
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
                            isLiked = w.isLiked,
                            CategoryName = w.CategoryName,
                            RateCount = w.RateCount
                        }),
                        SimilarPriceWashingMachines = result.SimilarPriceWashingMachines
                         .Select(w => new WashingMachineViewModel
                         {
                             Id = w.Id,
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
                             isLiked = w.isLiked,
                             CategoryName = w.CategoryName,
                             RateCount = w.RateCount
                         }),
                        Comments = result.Comments,
                        Offers = result.Offers,
                        BOGOGet = result.BOGOGet,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount,
                        ControllerName = result.ControllerName,
                        TotalQuantity = result.TotalQuantity
                    };

                    return View(washingMachines);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllWashingMachineComments(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _washingMachine.GetWashingMachineAllComments(id);

                if (result is not null)
                {
                    var washingMachine = new WashingMachineViewModel
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Rate = result.Rate,
                        CategoryName = result.CategoryName,
                        Comments = result.Comments,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount
                    };

                    return View("AllComments", washingMachine);
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}
