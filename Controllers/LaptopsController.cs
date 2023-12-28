using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Souq.Data.Enums;
using Souq.Data.IdentitySeeds;
using Souq.Models;
using Souq.Repository.Interfaces;

namespace Souq.Controllers
{
    [Authorize(Roles = "User")]
    public class LaptopsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LaptopsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Laptop> data = _unitOfWork.Laptops.GetAll();
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
        public IActionResult Add(Laptop data)
        {
            if (ModelState.IsValid)
            {
                if (data.clientFile != null)
                {
                    var stream = new MemoryStream();
                    data.clientFile.CopyTo(stream);
                    data.dbImage = stream.ToArray();
                }
                _unitOfWork.Laptops.Add(data);
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
            var result = _unitOfWork.Laptops.GetById(id);
            if (result == null)
                return NotFound();
            return View(result);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(Laptop data)
        {
            if (ModelState.IsValid)
            {
                if (data.clientFile != null)
                {
                    var stream = new MemoryStream();
                    data.clientFile.CopyTo(stream);
                    data.dbImage = stream.ToArray();
                }
                _unitOfWork.Laptops.Update(data);
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
            var result = _unitOfWork.Laptops.GetById(id);
            if (result == null)
                return NotFound();
            return View(result);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Laptop data)
        {
            _unitOfWork.Laptops.Delete(data);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Details));
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Details()
        {
            IEnumerable<Laptop> data = _unitOfWork.Laptops.GetAll();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult PriceDetails()
        {
            IEnumerable<Laptop> data = _unitOfWork.Laptops.GetByPrice();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DateDetails()
        {
            IEnumerable<Laptop> data = _unitOfWork.Laptops.GetByDate();
            return View(data);
        }
        public IActionResult DateDetailsForHp()
        {
            IEnumerable<Laptop> data = _unitOfWork.Laptops.GetByDate();
            var myList = new List<Laptop>();
            foreach (var item in data)
            {
                if (item.Category == LaptopsCategory.HP)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForHp()
        {
            IEnumerable<Laptop> data = _unitOfWork.Laptops.GetByPrice();
            var myList = new List<Laptop>();
            foreach (var item in data)
            {
                if (item.Category == LaptopsCategory.HP)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForDell()
        {
            IEnumerable<Laptop> data = _unitOfWork.Laptops.GetByDate();
            var myList = new List<Laptop>();
            foreach (var item in data)
            {
                if (item.Category == LaptopsCategory.Dell)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForDell()
        {
            IEnumerable<Laptop> data = _unitOfWork.Laptops.GetByPrice();
            var myList = new List<Laptop>();
            foreach (var item in data)
            {
                if (item.Category == LaptopsCategory.Dell)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForLenovo()
        {
            IEnumerable<Laptop> data = _unitOfWork.Laptops.GetByDate();
            var myList = new List<Laptop>();
            foreach (var item in data)
            {
                if (item.Category == LaptopsCategory.Lenovo)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForLenovo()
        {
            IEnumerable<Laptop> data = _unitOfWork.Laptops.GetByPrice();
            var myList = new List<Laptop>();
            foreach (var item in data)
            {
                if (item.Category == LaptopsCategory.Lenovo)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForMac()
        {
            IEnumerable<Laptop> data = _unitOfWork.Laptops.GetByDate();
            var myList = new List<Laptop>();
            foreach (var item in data)
            {
                if (item.Category == LaptopsCategory.Mac)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForMac()
        {
            IEnumerable<Laptop> data = _unitOfWork.Laptops.GetByPrice();
            var myList = new List<Laptop>();
            foreach (var item in data)
            {
                if (item.Category == LaptopsCategory.Mac)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult Hp()
        {
            var data = _unitOfWork.Laptops.GetAll();
            var myList = new List<Laptop>();
            foreach (var item in data)
            {
                if (item.Category == LaptopsCategory.HP)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult HpDetails(int id)
        {
            var data = _unitOfWork.Laptops.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Mac()
        {
            var data = _unitOfWork.Laptops.GetAll();
            var myList = new List<Laptop>();
            foreach (var item in data)
            {
                if (item.Category == LaptopsCategory.Mac)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult MacDetails(int id)
        {
            var data = _unitOfWork.Laptops.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Lenovo()
        {
            var data = _unitOfWork.Laptops.GetAll();
            var myList = new List<Laptop>();
            foreach (var item in data)
            {
                if (item.Category == LaptopsCategory.Lenovo)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult LenovoDetails(int id)
        {
            var data = _unitOfWork.Laptops.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Dell()
        {
            var data = _unitOfWork.Laptops.GetAll();
            var myList = new List<Laptop>();
            foreach (var item in data)
            {
                if (item.Category == LaptopsCategory.Dell)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DellDetails(int id)
        {
            var data = _unitOfWork.Laptops.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
    }
}
