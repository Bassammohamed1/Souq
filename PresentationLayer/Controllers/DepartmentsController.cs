using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentsService _departments;

        public DepartmentsController(IDepartmentsService departments)
        {
            _departments = departments;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            return View(departments);
        }

        public async Task<ActionResult> Add()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult> Add(Department data)
        {
            if (data is not null)
            {
                await _departments.Add(data);
              
                return RedirectToAction(nameof(Index));
            }

            return View(data);
        }

        public async Task<IActionResult> Update(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id == null && id != 0)
                throw new ArgumentNullException("Invalid id!!");

            var department = await _departments.GetDepartment(id);

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
                await _departments.Update(data);
            
                return RedirectToAction(nameof(Index));
            }

            return View(data);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id == null && id != 0)
                throw new ArgumentNullException("Invalid id!!");

            var department = await _departments.GetDepartment(id);

            if (department != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(Department data)
        {
            await _departments.Delete(data);
         
            return RedirectToAction(nameof(Index));
        }
    }
}
