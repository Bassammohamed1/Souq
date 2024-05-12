using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Souq.Models;
using Souq.Repository.Interfaces;

namespace Souq.Controllers
{
    [AllowAnonymous]
    public class ComputerAccessoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ComputerAccessoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Item> data = _unitOfWork.ComputerAccessories.GetAllItems();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Details()
        {
            IEnumerable<Item> data = _unitOfWork.ComputerAccessories.GetAllItems();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DateDetails()
        {
            IEnumerable<Item> data = _unitOfWork.ComputerAccessories.GetByDate();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult PriceDetails()
        {
            IEnumerable<Item> data = _unitOfWork.ComputerAccessories.GetByPrice();
            return View(data);
        }
        public IActionResult DateDetailsForMouse()
        {
            IEnumerable<Item> data = _unitOfWork.ComputerAccessories.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Mouse")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForMouse()
        {
            IEnumerable<Item> data = _unitOfWork.ComputerAccessories.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Mouse")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForKeyboard()
        {
            IEnumerable<Item> data = _unitOfWork.ComputerAccessories.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Keyboards")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForKeyboard()
        {
            IEnumerable<Item> data = _unitOfWork.ComputerAccessories.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Keyboard")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForHeadphone()
        {
            IEnumerable<Item> data = _unitOfWork.ComputerAccessories.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Headphone")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForHeadphone()
        {
            IEnumerable<Item> data = _unitOfWork.ComputerAccessories.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Headphone")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult Keyboard()
        {
            var data = _unitOfWork.ComputerAccessories.GetAllItems();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Keyboard")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult KeyboardDetails(int id)
        {
            var data = _unitOfWork.ComputerAccessories.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Mouse()
        {
            var data = _unitOfWork.ComputerAccessories.GetAllItems();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Mouse")
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
            var data = _unitOfWork.ComputerAccessories.GetAllItems();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Headphone")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult HeadphoneDetails(int id)
        {
            var data = _unitOfWork.ComputerAccessories.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
    }
}