using ApplicationLayer.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;
using PresentationLayer.ViewModels.ItemVMs;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "User")]
    public class ElectronicsController : Controller
    {
        private readonly IElectronicsService _electronics;
        private readonly IDepartmentsService _departments;
        public ElectronicsController(IElectronicsService electronics, IDepartmentsService departments)
        {
            _electronics = electronics;
            _departments = departments;
        }

        public async Task<IActionResult> Index()
        {
            var departments =await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _electronics.GetAllElectronics();

            var indexVM = new IndexViewModel()
            {
                Categories = result.Categories,
                Offers = result.Offers
            };

            return View(indexVM);
        }

        public async Task<IActionResult> Brands(string? orderIndex, int? page, string name, bool? Des)
        {
            var departments =await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (!string.IsNullOrEmpty(name))
            {
                var result = await _electronics.GetBrandsElectronics(orderIndex, page, name, Des);

                var data = new ItemsViewModel
                {
                    Items = result.Items,
                    CurrentPage = result.CurrentPage,
                    TotalPages = result.TotalPages,
                    ActionName = result.ActionName,
                    Brand = result.Brand
                };

                return View("Electronics", data);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? Des)
        {
            var departments =await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _electronics.GetElectronicsWithPriceFilter(orderIndex, page, price1, price2, Des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                ActionName = result.ActionName,
                Price1 = result.Price1,
                Price2 = result.Price2
            };

            return View("Electronics", data);
        }
    }
}