using DomainLayer.DTOs;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;
using PresentationLayer.ViewModels.ItemVMs;

namespace PresentationLayer.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var items = await _unitOfWork.Items.GetAll(1, int.MaxValue);
            var latestItems = items.OrderByDescending(i => i.AddedOn).Take(8).OrderBy(i => Guid.NewGuid());
            var featuredItems = items.OrderByDescending(i => i.Rate).Take(8).OrderBy(i => Guid.NewGuid());

            var offers = _unitOfWork.Offers.GetAllOffers().Result
                .Where(o => o.OfferType != OfferType.PromoCode);

            var homePageVM = new HomePageViewModel()
            {
                Departments = departments ?? Enumerable.Empty<Department>(),
                Latest = latestItems ?? Enumerable.Empty<Item>(),
                Featured = featuredItems ?? Enumerable.Empty<Item>(),
                Offers = offers ?? Enumerable.Empty<OfferDTO>()
            };

            return View(homePageVM);
        }

        public async Task<IActionResult> Details(int id)
        {
            var items = await _unitOfWork.Items.GetAll(1, int.MaxValue);

            var itemType = items.FirstOrDefault(i => i.ID == id)?.GetType().Name;

            if (itemType is not null)
            {
                return RedirectToAction("Details", $"{itemType}s", new { id });
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> OfferDetails(int id)
        {
            var offer = await _unitOfWork.Offers.FindOfferByID(id);

            if (offer is not null)
            {
                if (offer.OfferType == OfferType.BuyOneGetOne)
                {
                    var items = await _unitOfWork.Items.GetAll(1, int.MaxValue);

                    var itemType = items.FirstOrDefault(i => i.ID == offer.ItemOneID)?.GetType().Name;

                    if (itemType is not null)
                    {
                        return RedirectToAction("Details", $"{itemType}s", new { id = offer.ItemOneID });
                    }
                }
                else if (offer.OfferType == OfferType.FixedDiscount || offer.OfferType == OfferType.PercentDiscount)
                {
                    if (offer.DepartmentName is not null)
                    {
                        var nameAfterSplit = offer.DepartmentName.Split(' ');
                        string controllerName = nameAfterSplit[0];
                        for (int i = 1; i < nameAfterSplit.Length; i++)
                        {
                            controllerName += nameAfterSplit[i];
                        }

                        return RedirectToAction("Index", controllerName);
                    }
                    else if (offer.CategoryName is not null)
                    {
                        return RedirectToAction("Items", new { categoryName = offer.CategoryName });
                    }
                    else
                    {
                        var items = await _unitOfWork.Items.GetAll(1, int.MaxValue);

                        var itemType = items.FirstOrDefault(i => i.ID == offer.ItemID)?.GetType().Name;

                        if (itemType is not null)
                        {
                            return RedirectToAction("Details", $"{itemType}s", new { id = offer.ItemID });
                        }
                    }
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Items(string categoryName, string? orderIndex, int? page)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            int pageNumber = page ?? 1;
            int pageSize = 9;

            var allItems = _unitOfWork.Items.GetAll(1, int.MaxValue)
                .Result.Where(i => i.Category?.Name == categoryName);

            allItems = await _unitOfWork.Items.SortItems(allItems, orderIndex ?? "ID", false);

            var totalPages = (int)Math.Ceiling(allItems.Count() / (double)pageSize);

            var items = new ItemsViewModel()
            {
                Items = allItems.Skip((pageNumber - 1) * pageSize).Take(pageSize),
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Brand = categoryName
            };

            return await Task.FromResult(View(items));
        }

        public async Task<IActionResult> Filter(string key, int? page, string? orderIndex)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (!string.IsNullOrWhiteSpace(key))
            {
                int pageNumber = page ?? 1;
                int pageSize = 10;

                var filterVM = new FilterViewModel()
                {
                    SearchPhrase = key,
                    CurrentPage = pageNumber,
                    OrderIndex = orderIndex ?? "ID"
                };

                var adjusted = key.Split(' ');
                var items = await _unitOfWork.Items.GetAll(1, int.MaxValue);

                foreach (var word in adjusted)
                {
                    filterVM.MatchedItems = items.Where(i => i.Name.ToLower().Contains(word.ToLower()));
                    items = filterVM.MatchedItems;
                }

                if (filterVM.MatchedItems.Any())
                {
                    var totalPages = (int)Math.Ceiling(filterVM.MatchedItems.Count() / (double)pageSize);
                    filterVM.TotalPages = totalPages;

                    filterVM.MatchedItems = filterVM.MatchedItems
                        .OrderBy(i => i.GetType().GetProperty(orderIndex ?? "ID").GetValue(i, null))
                        .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                    return View(filterVM);
                }

                if (!filterVM.MatchedItems.Any())
                {
                    foreach (var word in adjusted)
                    {
                        foreach (var department in departments)
                        {
                            if (department.Name.ToLower().Contains(word.ToLower()))
                            {
                                var matched = await _unitOfWork.Departments.GetDepartmentItems(department);

                                var totalPages = (int)Math.Ceiling(matched.Count() / (double)pageSize);
                                filterVM.TotalPages = totalPages;

                                filterVM.MatchedItems = matched
                                    .OrderBy(i => i.GetType().GetProperty(orderIndex ?? "ID").GetValue(i, null))
                                    .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                                return View(filterVM);
                            }
                        }
                    }
                }

                if (!filterVM.MatchedItems.Any())
                {
                    var categories = await _unitOfWork.Categories.GetAllWithoutPagination();
                    foreach (var word in adjusted)
                    {
                        foreach (var category in categories)
                        {
                            if (category.Name.ToLower().Contains(word.ToLower()))
                            {
                                var matched = await _unitOfWork.Categories.GetCategoryItems(category);

                                var totalPages = (int)Math.Ceiling(matched.Count() / (double)pageSize);
                                filterVM.TotalPages = totalPages;

                                filterVM.MatchedItems = matched
                                    .OrderBy(i => i.GetType().GetProperty(orderIndex ?? "ID").GetValue(i, null))
                                    .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                                return View(filterVM);
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}