using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ItemsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ItemsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        private void CreateDepartmentsSelectList()
        {
            var data = _unitOfWork.Departments.GetAll();
            SelectList List = new SelectList(data, "Id", "Name");
            ViewBag.MyBag = List;
        }
        public IActionResult Index()
        {
            IEnumerable<Item> data = _unitOfWork.Items.GetAllItems();
            return View(data);
        }
        public IActionResult Add()
        {
            CreateDepartmentsSelectList();
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Add(Item data)
        {
            if (ModelState.IsValid)
            {
                var stream = new MemoryStream();
                data.clientFile.CopyTo(stream);
                data.dbImage = stream.ToArray();
                _unitOfWork.Items.Add(data);
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
            var result = _unitOfWork.Items.GetById(id);
            if (result == null)
                return NotFound();
            CreateDepartmentsSelectList();
            return View(result);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Update(Item data)
        {
            if (ModelState.IsValid)
            {
                var stream = new MemoryStream();
                data.clientFile.CopyTo(stream);
                data.dbImage = stream.ToArray();
                _unitOfWork.Items.Update(data);
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
            var result = _unitOfWork.Items.GetById(id);
            if (result == null)
                return NotFound();
            return View(result);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public IActionResult Delete(Item data)
        {
            _unitOfWork.Items.Delete(data);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
