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
        public IActionResult Index()
        {
            IEnumerable<Department> data = _unitOfWork.Departments.GetAll();
            return View(data);
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Add(Department data)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Departments.Add(data);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(data);
            }
        }
        public IActionResult Update(int id)
        {
            if (id == null || id == 0)
                return NotFound();
            var result = _unitOfWork.Departments.GetById(id);
            if (result == null)
                return NotFound();
            return View(result);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Update(Department data)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Departments.Update(data);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(data);
            }
        }
        public IActionResult Delete(int id)
        {
            if (id == null || id == 0)
                return NotFound();
            var result = _unitOfWork.Departments.GetById(id);
            if (result == null)
                return NotFound();
            return View(result);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public IActionResult Delete(Department data)
        {
            _unitOfWork.Departments.Delete(data);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
