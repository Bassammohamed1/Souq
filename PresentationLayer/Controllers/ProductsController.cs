using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IItemsService _items;

        public ProductsController(IItemsService items)
        {
            _items = items;
        }

        public async Task<IActionResult> Index(ProductsViewModel? data, int? page, string? filters, string? orderIndex, bool? des)
        {
            if (string.IsNullOrEmpty(filters))
            {
                var result = await _items.GetAllItemsWithSort(page, orderIndex, des);

                var productsVM = new ProductsViewModel()
                {
                    Items = result.Items,
                    CurrentPage = result.CurrentPage,
                    TotalPages = result.TotalPages,
                    OrderIndex = result.OrderIndex,
                    Des = result.Des
                };

                return View(productsVM);
            }
            else
            {
                var productsDTO = new ProductsDTO()
                {
                    Items = data.Items,
                    CurrentPage = data.CurrentPage,
                    OrderIndex = data.OrderIndex,
                    TotalPages = data.TotalPages,
                    SelectedFilters = data.SelectedFilters,
                    Des = data.Des
                };

                var result = await _items.GetAllItemsWithSortAndFilter(productsDTO, page, filters, orderIndex, des);

                var filteredProductsVM = new ProductsViewModel()
                {
                    Items = result.Items,
                    CurrentPage = result.CurrentPage,
                    TotalPages = result.TotalPages,
                    SelectedFilters = result.SelectedFilters,
                    OrderIndex = result.OrderIndex,
                    Des = result.Des
                };

                return View(filteredProductsVM);
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(ProductsViewModel data, string? orderIndex, bool? des)
        {
            var productsDTO = new ProductsDTO()
            {
                Items = data.Items,
                CurrentPage = data.CurrentPage,
                OrderIndex = data.OrderIndex,
                TotalPages = data.TotalPages,
                SelectedFilters = data.SelectedFilters,
                Des = data.Des
            };

            var result = await _items.GetAllItemsWithFilter(productsDTO, orderIndex, des);

            var productsVM = new ProductsViewModel()
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                SelectedFilters = result.SelectedFilters,
                OrderIndex = result.OrderIndex,
                Des = result.Des
            };

            return View(productsVM);
        }
    }
}