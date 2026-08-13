using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresentationLayer.ViewModels.Categories;
using X.PagedList;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private async Task CreateDepartmentsSelectList()
        {
            var allDepartments =await _departments.GetDepartments();

            var departmentsList = new SelectList(allDepartments.OrderBy(d => d.Name), "ID", "Name");

            ViewBag.departmentsViewBag = departmentsList;
        }

        private readonly ICategoriesService _categories;
        private readonly IDepartmentsService _departments;

        public CategoriesController(ICategoriesService categories, IDepartmentsService departments)
        {
            _categories = categories;
            _departments = departments;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var departments =await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _categories.GetAllCategoriesForIndexPage(page);

            var pagedData = new StaticPagedList<CategoryIndexVM>((IEnumerable<CategoryIndexVM>)result, result.First().CurrentPage, 10, result.First().TotalPages);

            return View(pagedData);
        }

        public async Task<IActionResult> Add()
        {
            var departments =await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            await CreateDepartmentsSelectList();

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Add(CategoryDTO data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _categories.Add(data);

                return RedirectToAction(nameof(Index));
            }

            return View(data);
        }

        public async Task<IActionResult> Update(int id)
        {
            var departments =await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id == null && id != 0)
                throw new ArgumentNullException("Invalid id!!");

            var category = await _categories.GetCategorie(id);

            if (category != null)
            {
                var categoryVM = new CategoryVM()
                {
                    Id = id,
                    Name = category.Name,
                    DepartmentsIds = _categories.GetCategoryDepartments(id).Result
                };

                await CreateDepartmentsSelectList();
                return View(categoryVM);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(CategoryDTO data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _categories.Update(data);

                return RedirectToAction(nameof(Index));
            }

            return View(data);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var departments =await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id == null && id != 0)
                throw new ArgumentNullException("Invalid id!!");

            var category = await _categories.GetCategorie(id);

            if (category != null)
            {
                var categoryVM = new CategoryVM()
                {
                    Id = id,
                    Name = category.Name,
                    DepartmentsIds = _categories.GetCategoryDepartments(id).Result
                };

                return View(categoryVM);
            }
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(Category data)
        {
            await _categories.Delete(data);

            return RedirectToAction(nameof(Index));
        }
    }
}
