using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [AllowAnonymous]
    public class LaptopsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LaptopsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Item> data = _unitOfWork.Laptops.GetAllItems();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Details()
        {
            IEnumerable<Item> data = _unitOfWork.Laptops.GetAllItems();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult PriceDetails()
        {
            IEnumerable<Item> data = _unitOfWork.Laptops.GetByPrice();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DateDetails()
        {
            IEnumerable<Item> data = _unitOfWork.Laptops.GetByDate();
            return View(data);
        }
        public IActionResult DateDetailsForHp()
        {
            IEnumerable<Item> data = _unitOfWork.Laptops.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "HP")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForHp()
        {
            IEnumerable<Item> data = _unitOfWork.Laptops.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "HP")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForDell()
        {
            IEnumerable<Item> data = _unitOfWork.Laptops.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Dell")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForDell()
        {
            IEnumerable<Item> data = _unitOfWork.Laptops.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Dell")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForLenovo()
        {
            IEnumerable<Item> data = _unitOfWork.Laptops.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Lenovo")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForLenovo()
        {
            IEnumerable<Item> data = _unitOfWork.Laptops.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Lenovo")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForMac()
        {
            IEnumerable<Item> data = _unitOfWork.Laptops.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Mac")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForMac()
        {
            IEnumerable<Item> data = _unitOfWork.Laptops.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Mac")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult Hp()
        {
            var data = _unitOfWork.Laptops.GetAllItems();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "HP")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult HpDetails(int id)
        {
            var data = _unitOfWork.Laptops.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Mac()
        {
            var data = _unitOfWork.Laptops.GetAllItems();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Mac")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult MacDetails(int id)
        {
            var data = _unitOfWork.Laptops.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Lenovo()
        {
            var data = _unitOfWork.Laptops.GetAllItems();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Lenovo")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult LenovoDetails(int id)
        {
            var data = _unitOfWork.Laptops.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Dell()
        {
            var data = _unitOfWork.Laptops.GetAllItems();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Dell")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DellDetails(int id)
        {
            var data = _unitOfWork.Laptops.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
    }
}
