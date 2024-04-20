using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "User")]
    public class MobilesAndTabletsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MobilesAndTabletsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetAll();
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
        public IActionResult Add(MobileAndTablet data)
        {
            if (ModelState.IsValid)
            {
                if (data.clientFile != null)
                {
                    var stream = new MemoryStream();
                    data.clientFile.CopyTo(stream);
                    data.dbImage = stream.ToArray();
                }
                _unitOfWork.MobilesAndTablets.Add(data);
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
            var result = _unitOfWork.MobilesAndTablets.GetById(id);
            if (result == null)
                return NotFound();
            return View(result);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(MobileAndTablet data)
        {
            if (ModelState.IsValid)
            {
                if (data.clientFile != null)
                {
                    var stream = new MemoryStream();
                    data.clientFile.CopyTo(stream);
                    data.dbImage = stream.ToArray();
                }
                _unitOfWork.MobilesAndTablets.Update(data);
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
            var result = _unitOfWork.MobilesAndTablets.GetById(id);
            if (result == null)
                return NotFound();
            return View(result);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(MobileAndTablet data)
        {
            _unitOfWork.MobilesAndTablets.Delete(data);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Details));
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Details()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetAll();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult PriceDetails()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetByPrice();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DateDetails()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetByDate();
            return View(data);
        }
        public IActionResult PriceDetailsForIPhoneAndIPad()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetByPrice();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.IPhone || item.Category == MobilesAndTabletsCategory.Ipad)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForIPhoneAndIPad()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetByDate();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.IPhone || item.Category == MobilesAndTabletsCategory.Ipad)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForSamsung()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetByPrice();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.Samsung)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForSamsung()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetByDate();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.Samsung)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForOppo()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetByPrice();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.Oppo)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForOppo()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetByDate();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.Oppo)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForHuawei()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetByPrice();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.Huawei)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForHuawei()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetByDate();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.Huawei)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult PriceDetailsForXiaomi()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetByPrice();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.Xiaomi)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult DateDetailsForXiaomi()
        {
            IEnumerable<MobileAndTablet> data = _unitOfWork.MobilesAndTablets.GetByDate();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.Xiaomi)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult IPhoneAndIPad()
        {
            var data = _unitOfWork.MobilesAndTablets.GetAll();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.IPhone || item.Category == MobilesAndTabletsCategory.Ipad)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult IPhoneAndIPadDetails(int id)
        {
            var data = _unitOfWork.MobilesAndTablets.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Samsung()
        {
            var data = _unitOfWork.MobilesAndTablets.GetAll();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.Samsung)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult SamsungDetails(int id)
        {
            var data = _unitOfWork.MobilesAndTablets.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Xiaomi()
        {
            var data = _unitOfWork.MobilesAndTablets.GetAll();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.Xiaomi)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult XiaomiDetails(int id)
        {
            var data = _unitOfWork.MobilesAndTablets.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Huawei()
        {
            var data = _unitOfWork.MobilesAndTablets.GetAll();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.Huawei)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult HuaweiDetails(int id)
        {
            var data = _unitOfWork.MobilesAndTablets.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
        public IActionResult Oppo()
        {
            var data = _unitOfWork.MobilesAndTablets.GetAll();
            var myList = new List<MobileAndTablet>();
            foreach (var item in data)
            {
                if (item.Category == MobilesAndTabletsCategory.Oppo)
                    myList.Add(item);
            }
            return View(myList);
        }
        public IActionResult OppoDetails(int id)
        {
            var data = _unitOfWork.MobilesAndTablets.GetAll();
            var result = data.SingleOrDefault(x => x.Id == id);
            return View(result);
        }
    }
}
