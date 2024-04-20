using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private List<BaseModel> _items = new List<BaseModel>();
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {

            var data1 = _unitOfWork.ComputerAccessories.GetAll();
            var data2 = _unitOfWork.ElectricalDevices.GetAll();
            var data3 = _unitOfWork.Laptops.GetAll();
            var data4 = _unitOfWork.MobilesAndTablets.GetAll();

            List<BaseModel> MyList = new();
            foreach (var item in data1)
            {
                if (item.Amount <= 5)
                {
                    item.Price *= .75;
                    _unitOfWork.SaveChanges();
                    MyList.Add(item);

                }
            }
            foreach (var item in data2)
            {
                if (item.Amount <= 5)
                {
                    item.Price *= .75;
                    _unitOfWork.SaveChanges();
                    MyList.Add(item);
                }
            }
            foreach (var item in data3)
            {
                if (item.Amount <= 5)
                {
                    item.Price *= .75;
                    _unitOfWork.SaveChanges();
                    MyList.Add(item);
                }
            }
            foreach (var item in data4)
            {
                if (item.Amount <= 5)
                {
                    item.Price *= .75;
                    _unitOfWork.SaveChanges();
                    MyList.Add(item);
                }
            }



            var result1 = data1.Where(o => o.Category == ComputerAccessoriesCategory.Keyboard).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result1 != null)
                _items.Add(result1);

            result1 = data1.Where(o => o.Category == ComputerAccessoriesCategory.Mouse).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result1 != null)
                _items.Add(result1);

            result1 = data1.Where(o => o.Category == ComputerAccessoriesCategory.Headphone).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result1 != null)
                _items.Add(result1);


            var result2 = data2.Where(o => o.Category == ElectricalDevicesCategory.Fridge).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result2 != null)
                _items.Add(result2);

            result2 = data2.Where(o => o.Category == ElectricalDevicesCategory.TV).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result2 != null)
                _items.Add(result2);

            result2 = data2.Where(o => o.Category == ElectricalDevicesCategory.WashingMachine).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result2 != null)
                _items.Add(result2);

            result2 = data2.Where(o => o.Category == ElectricalDevicesCategory.GasStove).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result2 != null)
                _items.Add(result2);


            var result3 = data3.Where(o => o.Category == LaptopsCategory.Dell).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result3 != null)
                _items.Add(result3);

            result3 = data3.Where(o => o.Category == LaptopsCategory.HP).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result3 != null)
                _items.Add(result3);

            result3 = data3.Where(o => o.Category == LaptopsCategory.Lenovo).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result3 != null)
                _items.Add(result3);

            result3 = data3.Where(o => o.Category == LaptopsCategory.Mac).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result3 != null)
                _items.Add(result3);


            var result4 = data4.Where(o => o.Category == MobilesAndTabletsCategory.IPhone || o.Category == MobilesAndTabletsCategory.Ipad).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result4 != null)
                _items.Add(result4);

            result4 = data4.Where(o => o.Category == MobilesAndTabletsCategory.Samsung).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result4 != null)
                _items.Add(result4);

            result4 = data4.Where(o => o.Category == MobilesAndTabletsCategory.Xiaomi).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result4 != null)
                _items.Add(result4);

            result4 = data4.Where(o => o.Category == MobilesAndTabletsCategory.Huawei).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result4 != null)
                _items.Add(result4);

            result4 = data4.Where(o => o.Category == MobilesAndTabletsCategory.Oppo).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result4 != null)
                _items.Add(result4);


            var allData = new HomePageVM()
            {
                AddedOnItems = _items,
                OfferedItems = MyList
            };

            return View(allData);
        }
        [AllowAnonymous]
        public IActionResult Filter(string searchString)
        {
            var ComputerAccessories = _unitOfWork.ComputerAccessories.GetAll();
            var ElectricalDevices = _unitOfWork.ElectricalDevices.GetAll();
            var Laptops = _unitOfWork.Laptops.GetAll();
            var MobilesAndTablets = _unitOfWork.MobilesAndTablets.GetAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                string formattedInput1 = searchString.Trim().ToLower();
                foreach (ComputerAccessoriesCategory item in Enum.GetValues(typeof(ComputerAccessoriesCategory)))
                {
                    string formattedEnumValue = item.ToString().Trim().ToLower();
                    var myList = new List<ComputerAccessoriesCategory>();
                    if (formattedInput1 == formattedEnumValue)
                    {
                        myList.Add(item);
                        IEnumerable<ComputerAccessory> ComputerData = ComputerAccessories.Where(x => x.Category == myList.First());
                        return View(ComputerData);
                    }
                }
                var data = ComputerAccessories.Where(n => string.Equals(n.Name.Trim().ToLower(), searchString.Trim().ToLower())).ToList();
                if (data.Count != 0)
                    return View(data);

                string formattedInput2 = searchString.Trim().ToLower();
                foreach (ElectricalDevicesCategory item in Enum.GetValues(typeof(ElectricalDevicesCategory)))
                {
                    string formattedEnumValue = item.ToString().Trim().ToLower();
                    var myList = new List<ElectricalDevicesCategory>();
                    if (formattedInput2 == formattedEnumValue)
                    {
                        myList.Add(item);
                        IEnumerable<ElectricalDevice> ElectricalData = ElectricalDevices.Where(x => x.Category == myList.First());
                        return View(ElectricalData);
                    }
                }
                var data1 = ElectricalDevices.Where(n => string.Equals(n.Name.Trim().ToLower(), searchString.Trim().ToLower(), StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (data1.Count != 0)
                    return View(data1);

                string formattedInput3 = searchString.Trim().ToLower();
                foreach (LaptopsCategory item in Enum.GetValues(typeof(LaptopsCategory)))
                {
                    string formattedEnumValue = item.ToString().Trim().ToLower();
                    var myList = new List<LaptopsCategory>();
                    if (formattedInput3 == formattedEnumValue)
                    {
                        myList.Add(item);
                        IEnumerable<Laptop> LaptopData = Laptops.Where(x => x.Category == myList.First());
                        return View(LaptopData);
                    }
                }
                var data2 = Laptops.Where(n => string.Equals(n.Name.Trim().ToLower(), searchString.Trim().ToLower(), StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (data2.Count != 0)
                    return View(data2);

                string formattedInput4 = searchString.Trim().ToLower();
                foreach (MobilesAndTabletsCategory item in Enum.GetValues(typeof(MobilesAndTabletsCategory)))
                {
                    string formattedEnumValue = item.ToString().Trim().ToLower();
                    var myList = new List<MobilesAndTabletsCategory>();
                    if (formattedInput4 == formattedEnumValue)
                    {
                        myList.Add(item);
                        IEnumerable<MobileAndTablet> MobileData = MobilesAndTablets.Where(x => x.Category == myList.First());
                        return View(MobileData);
                    }
                }
                var data3 = MobilesAndTablets.Where(n => string.Equals(n.Name.Trim().ToLower(), searchString.Trim().ToLower(), StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (data3.Count != 0)
                    return View(data3);

                return View();
            }

            return RedirectToAction("Index");
        }
    }
}