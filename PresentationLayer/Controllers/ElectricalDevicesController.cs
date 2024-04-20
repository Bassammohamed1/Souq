using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "User")]
    public class ElectricalDevicesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ElectricalDevicesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<ElectricalDevice> data = _unitOfWork.ElectricalDevices.GetAll();
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
        public IActionResult Add(ElectricalDevice data)
        {
            if (ModelState.IsValid)
            {
                if (data.clientFile != null)
                {
                    var stream = new MemoryStream();
                    data.clientFile.CopyTo(stream);
                    data.dbImage = stream.ToArray();
                }
                _unitOfWork.ElectricalDevices.Add(data);
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
            var result = _unitOfWork.ElectricalDevices.GetById(id);
            if (result == null)
                return NotFound();
            return View(result);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(ElectricalDevice data)
        {
            if (ModelState.IsValid)
            {
                if (data.clientFile != null)
                {
                    var stream = new MemoryStream();
                    data.clientFile.CopyTo(stream);
                    data.dbImage = stream.ToArray();
                }
                _unitOfWork.ElectricalDevices.Update(data);
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
            var result = _unitOfWork.ElectricalDevices.GetById(id);
            if (result == null)
                return NotFound();
            return View(result);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(ElectricalDevice data)
        {
            _unitOfWork.ElectricalDevices.Delete(data);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Details));
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Details()
        {
            IEnumerable<ElectricalDevice> data = _unitOfWork.ElectricalDevices.GetAll();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult PriceDetails()
        {
            IEnumerable<ElectricalDevice> data = _unitOfWork.ElectricalDevices.GetByPrice();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DateDetails()
        {
            IEnumerable<ElectricalDevice> data = _unitOfWork.ElectricalDevices.GetByDate();
            return View(data);
        }
        public IActionResult PriceDetailsForTV()
        {
            IEnumerable<ElectricalDevice> data = _unitOfWork.ElectricalDevices.GetByPrice();
            var myList = new List<ElectricalDevice>();
            foreach (var item in data)
            {
                if (item.Category == ElectricalDevicesCategory.TV)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForTV()
        {
            IEnumerable<ElectricalDevice> data = _unitOfWork.ElectricalDevices.GetByDate();
            var myList = new List<ElectricalDevice>();
            foreach (var item in data)
            {
                if (item.Category == ElectricalDevicesCategory.TV)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForWashing()
        {
            IEnumerable<ElectricalDevice> data = _unitOfWork.ElectricalDevices.GetByPrice();
            var myList = new List<ElectricalDevice>();
            foreach (var item in data)
            {
                if (item.Category == ElectricalDevicesCategory.WashingMachine)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForWashing()
        {
            IEnumerable<ElectricalDevice> data = _unitOfWork.ElectricalDevices.GetByDate();
            var myList = new List<ElectricalDevice>();
            foreach (var item in data)
            {
                if (item.Category == ElectricalDevicesCategory.WashingMachine)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForGasStove()
        {
            IEnumerable<ElectricalDevice> data = _unitOfWork.ElectricalDevices.GetByPrice();
            var myList = new List<ElectricalDevice>();
            foreach (var item in data)
            {
                if (item.Category == ElectricalDevicesCategory.GasStove)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForGasStove()
        {
            IEnumerable<ElectricalDevice> data = _unitOfWork.ElectricalDevices.GetByDate();
            var myList = new List<ElectricalDevice>();
            foreach (var item in data)
            {
                if (item.Category == ElectricalDevicesCategory.GasStove)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForFridge()
        {
            IEnumerable<ElectricalDevice> data = _unitOfWork.ElectricalDevices.GetByPrice();
            var myList = new List<ElectricalDevice>();
            foreach (var item in data)
            {
                if (item.Category == ElectricalDevicesCategory.Fridge)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForFridge()
        {
            IEnumerable<ElectricalDevice> data = _unitOfWork.ElectricalDevices.GetByDate();
            var myList = new List<ElectricalDevice>();
            foreach (var item in data)
            {
                if (item.Category == ElectricalDevicesCategory.Fridge)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult WashingMachine()
        {
            var data = _unitOfWork.ElectricalDevices.GetAll();
            var myList = new List<ElectricalDevice>();
            foreach (var item in data)
            {
                if (item.Category == ElectricalDevicesCategory.WashingMachine)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult WashingMachineDetails(int id)
        {
            var data = _unitOfWork.ElectricalDevices.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Tv()
        {
            var data = _unitOfWork.ElectricalDevices.GetAll();
            var myList = new List<ElectricalDevice>();
            foreach (var item in data)
            {
                if (item.Category == ElectricalDevicesCategory.TV)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult TvDetails(int id)
        {
            var data = _unitOfWork.ElectricalDevices.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Fridge()
        {
            var data = _unitOfWork.ElectricalDevices.GetAll();
            var myList = new List<ElectricalDevice>();
            foreach (var item in data)
            {
                if (item.Category == ElectricalDevicesCategory.Fridge)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult FridgeDetails(int id)
        {
            var data = _unitOfWork.ElectricalDevices.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult GasStove()
        {
            var data = _unitOfWork.ElectricalDevices.GetAll();
            var myList = new List<ElectricalDevice>();
            foreach (var item in data)
            {
                if (item.Category == ElectricalDevicesCategory.GasStove)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult GasStoveDetails(int id)
        {
            var data = _unitOfWork.ElectricalDevices.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
    }
}
