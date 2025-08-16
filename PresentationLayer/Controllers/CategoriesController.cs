using DomainLayer.DTOs;
using DomainLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresentationLayer.ViewModels.Categories;
using X.PagedList;
using X.PagedList.Extensions;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private async Task CreateDepartmentsSelectList()
        {
            var allDepartments = await _unitOfWork.Departments.GetAllWithoutPagination();

            var departmentsList = new SelectList(allDepartments.OrderBy(d => d.Name), "ID", "Name");

            ViewBag.departmentsViewBag = departmentsList;
        }

        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var totalCount = _unitOfWork.Categories.GetAllWithoutPagination().Result.Count();

            var categories = await _unitOfWork.Categories.AllCategoriesWithDepartment(pageNumber, pageSize);

            var data = categories.Select(c => new CategoryIndexVM
            {
                Id = c.ID,
                Name = c.Name,
                Image = c.imageSrc,
                Departments = c.CategoryDepartments.Select(cd => cd.Department.Name).ToList(),
                TotalPages = totalCount,
                CurrentPage = pageNumber
            }).ToList();

              var pagedData = new StaticPagedList<CategoryIndexVM>(data, pageNumber, pageSize, totalCount);

            return View(pagedData);
        }

        public async Task<IActionResult> Add()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
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
                await _unitOfWork.Categories.AddCategory(data);
                await _unitOfWork.Commit();
                return RedirectToAction(nameof(Index));
            }

            return View(data);
        }

        public async Task<IActionResult> Update(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id == null && id != 0)
                throw new ArgumentNullException("Invalid id!!");

            var category = await _unitOfWork.Categories.GetById(id);

            if (category != null)
            {
                var categoryVM = new CategoryVM()
                {
                    Id = id,
                    Name = category.Name,
                    DepartmentsIds = _unitOfWork.Categories.GetCategoryDepartments(id).Result
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
                await _unitOfWork.Categories.UpdateCategory(data);
                await _unitOfWork.Commit();
                return RedirectToAction(nameof(Index));
            }

            return View(data);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id == null && id != 0)
                throw new ArgumentNullException("Invalid id!!");

            var category = await _unitOfWork.Categories.GetById(id);

            if (category != null)
            {
                var categoryVM = new CategoryVM()
                {
                    Id = id,
                    Name = category.Name,
                    DepartmentsIds = _unitOfWork.Categories.GetCategoryDepartments(id).Result
                };
                return View(categoryVM);
            }
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(CategoryDTO data)
        {
            await _unitOfWork.Categories.DeleteCategory(data);
            await _unitOfWork.Commit();
            return RedirectToAction(nameof(Index));
        }
    }
}
