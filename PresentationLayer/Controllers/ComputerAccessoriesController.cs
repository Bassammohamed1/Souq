using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "User")]
    public class ComputerAccessoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ComputerAccessoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<ComputerAccessory> data = _unitOfWork.ComputerAccessories.GetAll();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Add(ComputerAccessory data)
        {
            if (ModelState.IsValid)
            {
                if (data.clientFile != null)
                {
                    var stream = new MemoryStream();
                    data.clientFile.CopyTo(stream);
                    data.dbImage = stream.ToArray();
                }
                _unitOfWork.ComputerAccessories.Add(data);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Details));
            }
            else
            {
                return View(data);
            }

        }
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id)
        {
            if (id == null || id == 0)
                return NotFound();
            var result = _unitOfWork.ComputerAccessories.GetById(id);
            if (result == null)
                return NotFound();
            return View(result);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(ComputerAccessory data)
        {
            if (ModelState.IsValid)
            {
                if (data.clientFile != null)
                {
                    var stream = new MemoryStream();
                    data.clientFile.CopyTo(stream);
                    data.dbImage = stream.ToArray();
                }
                _unitOfWork.ComputerAccessories.Update(data);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Details));
            }
            else
            {
                return View(data);
            }
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            if (id == null || id == 0)
                return NotFound();
            var result = _unitOfWork.ComputerAccessories.GetById(id);
            if (result == null)
                return NotFound();
            return View(result);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(ComputerAccessory data)
        {
            _unitOfWork.ComputerAccessories.Delete(data);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Details));
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Details()
        {
            IEnumerable<ComputerAccessory> data = _unitOfWork.ComputerAccessories.GetAll();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DateDetails()
        {
            IEnumerable<ComputerAccessory> data = _unitOfWork.ComputerAccessories.GetByDate();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult PriceDetails()
        {
            IEnumerable<ComputerAccessory> data = _unitOfWork.ComputerAccessories.GetByPrice();
            return View(data);
        }
        public IActionResult DateDetailsForMouse()
        {
            IEnumerable<ComputerAccessory> data = _unitOfWork.ComputerAccessories.GetByDate();
            var myList = new List<ComputerAccessory>();
            foreach (var item in data)
            {
                if (item.Category == ComputerAccessoriesCategory.Mouse)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForMouse()
        {
            IEnumerable<ComputerAccessory> data = _unitOfWork.ComputerAccessories.GetByPrice();
            var myList = new List<ComputerAccessory>();
            foreach (var item in data)
            {
                if (item.Category == ComputerAccessoriesCategory.Mouse)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForKeyboard()
        {
            IEnumerable<ComputerAccessory> data = _unitOfWork.ComputerAccessories.GetByDate();
            var myList = new List<ComputerAccessory>();
            foreach (var item in data)
            {
                if (item.Category == ComputerAccessoriesCategory.Keyboard)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForKeyboard()
        {
            IEnumerable<ComputerAccessory> data = _unitOfWork.ComputerAccessories.GetByPrice();
            var myList = new List<ComputerAccessory>();
            foreach (var item in data)
            {
                if (item.Category == ComputerAccessoriesCategory.Keyboard)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForHeadphone()
        {
            IEnumerable<ComputerAccessory> data = _unitOfWork.ComputerAccessories.GetByDate();
            var myList = new List<ComputerAccessory>();
            foreach (var item in data)
            {
                if (item.Category == ComputerAccessoriesCategory.Headphone)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForHeadphone()
        {
            IEnumerable<ComputerAccessory> data = _unitOfWork.ComputerAccessories.GetByPrice();
            var myList = new List<ComputerAccessory>();
            foreach (var item in data)
            {
                if (item.Category == ComputerAccessoriesCategory.Headphone)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult Keyboard()
        {
            var data = _unitOfWork.ComputerAccessories.GetAll();
            var myList = new List<ComputerAccessory>();
            foreach (var item in data)
            {
                if (item.Category == ComputerAccessoriesCategory.Keyboard)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult KeyboardDetails(int id)
        {
            var data = _unitOfWork.ComputerAccessories.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Mouse()
        {
            var data = _unitOfWork.ComputerAccessories.GetAll();
            var myList = new List<ComputerAccessory>();
            foreach (var item in data)
            {
                if (item.Category == ComputerAccessoriesCategory.Mouse)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult MouseDetails(int id)
        {
            var data = _unitOfWork.ComputerAccessories.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Headphone()
        {
            var data = _unitOfWork.ComputerAccessories.GetAll();
            var myList = new List<ComputerAccessory>();
            foreach (var item in data)
            {
                if (item.Category == ComputerAccessoriesCategory.Headphone)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult HeadphoneDetails(int id)
        {
            var data = _unitOfWork.ComputerAccessories.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
    }
}
