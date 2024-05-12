using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Souq.Models;
using Souq.Repository.Interfaces;

namespace Souq.Controllers
{
    [AllowAnonymous]
    public class MobilesAndTabletsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MobilesAndTabletsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetAllItems();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Details()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetAllItems();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult PriceDetails()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetByPrice();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DateDetails()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetByDate();
            return View(data);
        }
        public IActionResult PriceDetailsForIPhoneAndIPad()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "IPhone" || item.ItemType == "IPad")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForIPhoneAndIPad()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "IPhone" || item.ItemType == "IPad")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForSamsung()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Samsung")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForSamsung()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Samsung")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForOppo()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Oppo")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForOppo()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Oppo")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForHuawei()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Huawei")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForHuawei()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Huawei")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForXiaomi()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetByPrice();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Xiaomi")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForXiaomi()
        {
            IEnumerable<Item> data = _unitOfWork.MobilesAndTablets.GetByDate();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Xiaomi")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult IPhoneAndIPad()
        {
            var data = _unitOfWork.MobilesAndTablets.GetAllItems();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "IPhone" || item.ItemType == "IPad")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult IPhoneAndIPadDetails(int id)
        {
            var data = _unitOfWork.MobilesAndTablets.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Samsung()
        {
            var data = _unitOfWork.MobilesAndTablets.GetAll();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Samsung")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult SamsungDetails(int id)
        {
            var data = _unitOfWork.MobilesAndTablets.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Xiaomi()
        {
            var data = _unitOfWork.MobilesAndTablets.GetAll();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Xiaomi")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult XiaomiDetails(int id)
        {
            var data = _unitOfWork.MobilesAndTablets.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Huawei()
        {
            var data = _unitOfWork.MobilesAndTablets.GetAll();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Huawei")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult HuaweiDetails(int id)
        {
            var data = _unitOfWork.MobilesAndTablets.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Oppo()
        {
            var data = _unitOfWork.MobilesAndTablets.GetAllItems();
            var myList = new List<Item>();
            foreach (var item in data)
            {
                if (item.ItemType == "Oppo")
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult OppoDetails(int id)
        {
            var data = _unitOfWork.MobilesAndTablets.GetAllItems();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
    }
}