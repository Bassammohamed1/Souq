using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [AllowAnonymous]
    public class ElectricalDevicesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ElectricalDevicesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Item> data = _unitOfWork.ElectricalDevices.GetAllItems();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Details()
        {
            IEnumerable<Item> data = _unitOfWork.ElectricalDevices.GetAllItems();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult PriceDetails()
        {
            IEnumerable<Item> data = _unitOfWork.ElectricalDevices.GetByPrice();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DateDetails()
        {
            IEnumerable<Item> data = _unitOfWork.ElectricalDevices.GetByDate();
            return View(data);
        }
        public IActionResult PriceDetailsForTV()
        {
            IEnumerable<Item> data = _unitOfWork.ElectricalDevices.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "TV")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForTV()
        {
            IEnumerable<Item> data = _unitOfWork.ElectricalDevices.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "TV")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForWashingMachine()
        {
            IEnumerable<Item> data = _unitOfWork.ElectricalDevices.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "WashingMachine")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForWashingMachine()
        {
            IEnumerable<Item> data = _unitOfWork.ElectricalDevices.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "WashingMachine")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForGasStove()
        {
            IEnumerable<Item> data = _unitOfWork.ElectricalDevices.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "GasStove")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForGasStove()
        {
            IEnumerable<Item> data = _unitOfWork.ElectricalDevices.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "GasStove")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForFridge()
        {
            IEnumerable<Item> data = _unitOfWork.ElectricalDevices.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Fridge")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForFridge()
        {
            IEnumerable<Item> data = _unitOfWork.ElectricalDevices.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Fridge")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult WashingMachine()
        {
            var data = _unitOfWork.ElectricalDevices.GetAllItems();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "WashingMachine")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult WashingMachineDetails(int id)
        {
            var data = _unitOfWork.ElectricalDevices.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Tv()
        {
            var data = _unitOfWork.ElectricalDevices.GetAllItems();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "TV")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult TvDetails(int id)
        {
            var data = _unitOfWork.ElectricalDevices.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Fridge()
        {
            var data = _unitOfWork.ElectricalDevices.GetAllItems();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Fridge")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult FridgeDetails(int id)
        {
            var data = _unitOfWork.ElectricalDevices.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult GasStove()
        {
            var data = _unitOfWork.ElectricalDevices.GetAllItems();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "GasStove")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult GasStoveDetails(int id)
        {
            var data = _unitOfWork.ElectricalDevices.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
    }
}
