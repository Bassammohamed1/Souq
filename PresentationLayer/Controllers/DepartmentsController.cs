using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var data = await _unitOfWork.Departments.GetAllWithoutPagination();

            return View(data);
        }

        public async Task<ActionResult> Add()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult> Add(Department data)
        {
            if (data is not null)
            {
                await _unitOfWork.Departments.Add(data);
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

            var department = await _unitOfWork.Departments.GetById(id);

            if (department != null)
                return View(department);

            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(Department data)
        {
            if (data is not null)
            {
                await _unitOfWork.Departments.Update(data);
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

            var department = await _unitOfWork.Departments.GetById(id);

            if (department != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(Department data)
        {
            await _unitOfWork.Departments.Delete(data);
            await _unitOfWork.Commit();
            return RedirectToAction(nameof(Index));
        }
    }
}
