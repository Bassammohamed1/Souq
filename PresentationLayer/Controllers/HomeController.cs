using ApplicationLayer.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;
using PresentationLayer.ViewModels.ItemVMs;

namespace PresentationLayer.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IDepartmentsService _departments;
        private readonly IHomePageService _homePageServices;

        public HomeController(IDepartmentsService departments, IHomePageService homePageServices)
        {
            _departments = departments;
            _homePageServices = homePageServices;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _homePageServices.GetHomePageRelatedData();

            var homePageVM = new HomePageViewModel()
            {
                Departments = result.Departments,
                Latest = result.Latest,
                Featured = result.Featured,
                Offers = result.Offers
            };

            return View(homePageVM);
        }

        public async Task<IActionResult> Details(int id)
        {
            var itemType = await _homePageServices.GetItemType(id);

            return itemType is not null ? RedirectToAction("Details", $"{itemType}s", new { id }) :
                RedirectToAction("Index");
        }

        public async Task<IActionResult> OfferDetails(int id)
        {
            var result = await _homePageServices.GetHomePageOfferDetails(id);


            switch (result.ActionName)
            {
                case "Index":

                    if (string.IsNullOrEmpty(result.ControllerName))
                        return RedirectToAction("Index");
                    else
                        return RedirectToAction("Index", result.ControllerName);

                case "Items":

                    return RedirectToAction("Items", new { categoryName = result.CategoryName });

                case "Details":

                    if (result.ItemOneID is not null)
                        return RedirectToAction("Details", result.ItemType, new { id = result.ItemOneID });
                    else
                        return RedirectToAction("Details", result.ItemType, new { id = result.ItemID });
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Items(string categoryName, string? orderIndex, int? page)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _homePageServices.GetAllItems(categoryName, orderIndex, page);

            var items = new ItemsViewModel()
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Brand = result.Brand
            };

            return View(items);
        }

        public async Task<IActionResult> Filter(string key, int? page, string? orderIndex)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (!string.IsNullOrWhiteSpace(key))
            {
                var result = await _homePageServices.GetFilteredItems(key, page, orderIndex);

                var filterVM = new FilterViewModel()
                {
                    SearchPhrase = result.SearchPhrase,
                    CurrentPage = result.CurrentPage,
                    OrderIndex = result.OrderIndex,
                    MatchedItems = result.MatchedItems,
                    TotalPages = result.TotalPages
                };

                return View(filterVM);
            }

            return RedirectToAction("Index");
        }
    }
}