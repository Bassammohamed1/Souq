using DomainLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(ProductsViewModel? data, int? page, string? filters, string? orderIndex, bool? des)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            if (string.IsNullOrEmpty(filters))
            {
                var allItems = await _unitOfWork.Items.GetAll(1, int.MaxValue);

                var totalPages = (int)Math.Ceiling(allItems.Count() / (double)pageSize);

                var items = await _unitOfWork.Items.GetAll(1, int.MaxValue);

                var productsVM = new ProductsViewModel()
                {
                    Items = _unitOfWork.Items.SortItems(items, orderIndex ?? "ID", des ?? false).Result.Skip((pageNumber - 1) * pageSize).Take(pageSize),
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    OrderIndex = orderIndex ?? "ID",
                    Des = des ?? false
                };

                return View(productsVM);
            }
            else
            {
                data.SelectedFilters = filters.Split(',').ToList();

                var allItems = await _unitOfWork.Items.GetFilteredItems(data.SelectedFilters, 1, int.MaxValue);

                var totalPages = (int)Math.Ceiling(allItems.Count() / (double)pageSize);

                var filteredItems = await _unitOfWork.Items.GetFilteredItems(data.SelectedFilters, 1, int.MaxValue);

                var filteredProductsVM = new ProductsViewModel()
                {
                    Items = _unitOfWork.Items.SortItems(filteredItems, orderIndex ?? "ID", des ?? false).Result.Skip((pageNumber - 1) * pageSize).Take(pageSize),
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    SelectedFilters = data.SelectedFilters,
                    OrderIndex = orderIndex ?? "ID",
                    Des = des ?? false
                };

                return View(filteredProductsVM);
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(ProductsViewModel data, string? orderIndex, bool? des)
        {
            var allItems = await _unitOfWork.Items.GetFilteredItems(data.SelectedFilters, 1, int.MaxValue);

            var totalPages = (int)Math.Ceiling(allItems.Count() / (double)10);

            var items = await _unitOfWork.Items.GetFilteredItems(data.SelectedFilters, 1, 10);

            var productsVM = new ProductsViewModel()
            {
                Items = items,
                CurrentPage = 1,
                TotalPages = totalPages,
                SelectedFilters = data.SelectedFilters,
                OrderIndex = orderIndex ?? "ID",
                Des = des ?? false
            };

            return View(productsVM);
        }
    }
}