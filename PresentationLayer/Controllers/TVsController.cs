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
    public class TVsController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _tvs.GetSpecificCategoriesForSelectList();

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly ITVsService _tvs;
        private readonly IDepartmentsService _departments;

        public TVsController(ITVsService tv, IDepartmentsService departments)
        {
            _tvs = tv;
            _departments = departments;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = _tvs.GetTVsWithRelatedOnes();

            var tvsVM = new ItemViewModel<TVViewModel>()
            {
                ItemCategories = result.ItemCategories,
                DiscountedItems = result.DiscountedItems
                .Select(t => new TVViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Rate = t.Rate,
                    Price = t.Price,
                    NewPrice = t.NewPrice ?? 0,
                    imageSrc = t.imageSrc,
                    ConnectivityTechnology = t.ConnectivityTechnology,
                    DisplayTechnology = t.DisplayTechnology,
                    ItemDimensions = t.ItemDimensions,
                    RefreshRate = t.RefreshRate,
                    SpecialFeatures = t.SpecialFeatures,
                    Resolution = t.Resolution,
                    ScreenSize = t.ScreenSize,
                    isLiked = t.isLiked,
                    CategoryName = t.CategoryName,
                    RateCount = t.RateCount
                }),
                latestItems = result.latestItems
                .Select(t => new TVViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Rate = t.Rate,
                    Price = t.Price,
                    NewPrice = t.NewPrice ?? 0,
                    imageSrc = t.imageSrc,
                    ConnectivityTechnology = t.ConnectivityTechnology,
                    DisplayTechnology = t.DisplayTechnology,
                    ItemDimensions = t.ItemDimensions,
                    RefreshRate = t.RefreshRate,
                    SpecialFeatures = t.SpecialFeatures,
                    Resolution = t.Resolution,
                    ScreenSize = t.ScreenSize,
                    isLiked = t.isLiked,
                    CategoryName = t.CategoryName,
                    RateCount = t.RateCount
                }),
                TopRatedItems = result.TopRatedItems
                .Select(t => new TVViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Rate = t.Rate,
                    Price = t.Price,
                    NewPrice = t.NewPrice ?? 0,
                    imageSrc = t.imageSrc,
                    ConnectivityTechnology = t.ConnectivityTechnology,
                    DisplayTechnology = t.DisplayTechnology,
                    ItemDimensions = t.ItemDimensions,
                    RefreshRate = t.RefreshRate,
                    SpecialFeatures = t.SpecialFeatures,
                    Resolution = t.Resolution,
                    ScreenSize = t.ScreenSize,
                    isLiked = t.isLiked,
                    CategoryName = t.CategoryName,
                    RateCount = t.RateCount
                }),
                Offers = result.Offers ?? Enumerable.Empty<Offer>().AsQueryable()
            };

            return View(tvsVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var tvs = _tvs.GetTVs(pageNumber, pageSize);

            return View(tvs);
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
        public async Task<IActionResult> Add(TV data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _tvs.Add(data);

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

            var TV = await _tvs.GetTV(id);

            if (TV != null)
            {
                await CreateCategoriesSelectList();
                return View(TV);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(TV data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _tvs.Update(data);

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

            var TV = await _tvs.GetTV(id);

            if (TV != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(TV data)
        {
            await _tvs.Delete(data);

            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> TVs()
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

                var result = await _tvs.GetBrandsTVs(orderIndex, page, name, des);

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

                return View("TVs", data);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Discounted(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _tvs.GetDiscountedTVs(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("TVs", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _tvs.GetTopRatedTVs(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("TVs", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _tvs.GetLatestTVs(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("TVs", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _tvs.GetTVsWithPriceFilter(orderIndex, page, price1, price2, des);

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

            return View("TVs", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _tvs.GetTVDetails(id);

                if (result != null)
                {
                    var tvs = new TVViewModel
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
                        ConnectivityTechnology = result.ConnectivityTechnology,
                        DisplayTechnology = result.DisplayTechnology,
                        ItemDimensions = result.ItemDimensions,
                        RefreshRate = result.RefreshRate,
                        SpecialFeatures = result.SpecialFeatures,
                        Resolution = result.Resolution,
                        ScreenSize = result.ScreenSize,
                        CategoryName = result.CategoryName,
                        RelatedTVs = result.RelatedTVs
                        .Select(t => new TVViewModel
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Rate = t.Rate,
                            Price = t.Price,
                            NewPrice = t.NewPrice,
                            imageSrc = t.imageSrc,
                            ConnectivityTechnology = t.ConnectivityTechnology,
                            DisplayTechnology = t.DisplayTechnology,
                            ItemDimensions = t.ItemDimensions,
                            RefreshRate = t.RefreshRate,
                            SpecialFeatures = t.SpecialFeatures,
                            Resolution = t.Resolution,
                            ScreenSize = t.ScreenSize,
                            isLiked = t.isLiked,
                            CategoryName = t.CategoryName,
                            RateCount = t.RateCount
                        }),
                        SimilarPriceTVs = result.SimilarPriceTVs
                        .Select(t => new TVViewModel
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Rate = t.Rate,
                            Price = t.Price,
                            NewPrice = t.NewPrice,
                            imageSrc = t.imageSrc,
                            ConnectivityTechnology = t.ConnectivityTechnology,
                            DisplayTechnology = t.DisplayTechnology,
                            ItemDimensions = t.ItemDimensions,
                            RefreshRate = t.RefreshRate,
                            SpecialFeatures = t.SpecialFeatures,
                            Resolution = t.Resolution,
                            ScreenSize = t.ScreenSize,
                            isLiked = t.isLiked,
                            CategoryName = t.CategoryName,
                            RateCount = t.RateCount
                        }),
                        Comments = result.Comments,
                        Offers = result.Offers,
                        BOGOGet = result.BOGOGet,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount,
                        ControllerName = result.ControllerName,
                        TotalQuantity = result.TotalQuantity
                    };

                    return View(tvs);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllTVComments(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _tvs.GetTVAllComments(id);

                if (result is not null)
                {
                    var tv = new TVViewModel
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Rate = result.Rate,
                        CategoryName = result.CategoryName,
                        Comments = result.Comments,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount
                    };

                    return View("AllComments", tv);
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}