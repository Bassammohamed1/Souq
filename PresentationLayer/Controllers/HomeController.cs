using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Souq.Models;
using Souq.Models.ViewModels;
using Souq.Repository.Interfaces;
namespace Souq.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private List<Item> _items = new List<Item>();
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {

            var data1 = _unitOfWork.ComputerAccessories.GetAllItems();
            var data2 = _unitOfWork.ElectricalDevices.GetAllItems();
            var data3 = _unitOfWork.Laptops.GetAllItems();
            var data4 = _unitOfWork.MobilesAndTablets.GetAllItems();

            List<Item> MyList = new();
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



            var result1 = data1.Where(o => o.ItemType == "Keyboard").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result1 != null)
                _items.Add(result1);

            result1 = data1.Where(o => o.ItemType == "Mouse").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result1 != null)
                _items.Add(result1);

            result1 = data1.Where(o => o.ItemType == "Headphone").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result1 != null)
                _items.Add(result1);


            var result2 = data2.Where(o => o.ItemType == "Fridge").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result2 != null)
                _items.Add(result2);

            result2 = data2.Where(o => o.ItemType == "TV").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result2 != null)
                _items.Add(result2);

            result2 = data2.Where(o => o.ItemType == "WashingMachine").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result2 != null)
                _items.Add(result2);

            result2 = data2.Where(o => o.ItemType == "GasStove").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result2 != null)
                _items.Add(result2);


            var result3 = data3.Where(o => o.ItemType == "Dell").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result3 != null)
                _items.Add(result3);

            result3 = data3.Where(o => o.ItemType == "HP").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result3 != null)
                _items.Add(result3);

            result3 = data3.Where(o => o.ItemType == "Lenovo").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result3 != null)
                _items.Add(result3);

            result3 = data3.Where(o => o.ItemType == "Mac").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result3 != null)
                _items.Add(result3);


            var result4 = data4.Where(o => o.ItemType == "IPhone" || o.ItemType == "IPad").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result4 != null)
                _items.Add(result4);

            result4 = data4.Where(o => o.ItemType == "Samsung").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result4 != null)
                _items.Add(result4);

            result4 = data4.Where(o => o.ItemType == "Xiaomi").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result4 != null)
                _items.Add(result4);

            result4 = data4.Where(o => o.ItemType == "Huawei").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result4 != null)
                _items.Add(result4);

            result4 = data4.Where(o => o.ItemType == "Oppo").OrderByDescending(x => x.AddedOn).FirstOrDefault();
            if (result4 != null)
                _items.Add(result4);


            var allData = new HomePageVM()
            {
                AddedOnItems = _items,
                OfferedItems = MyList
            };

            return View(allData);
        }
        public IActionResult Filter(string searchString)
        {
            var ComputerAccessories = _unitOfWork.ComputerAccessories.GetAllItems();
            var ElectricalDevices = _unitOfWork.ElectricalDevices.GetAllItems();
            var Laptops = _unitOfWork.Laptops.GetAllItems();
            var MobilesAndTablets = _unitOfWork.MobilesAndTablets.GetAllItems();

            if (!string.IsNullOrEmpty(searchString))
            {
                string formattedInput1 = searchString.Trim().ToLower();
                foreach (var item in ComputerAccessories)
                {
                    string itemType1 = item.ItemType.ToString().Trim().ToLower();
                    if (itemType1 == formattedInput1 || itemType1 + 's' == formattedInput1)
                    {
                        IEnumerable<Item> ComputerData = ComputerAccessories.Where(x => x.ItemType == item.ItemType);
                        return View(ComputerData);
                    }
                }
                var data1 = ComputerAccessories.Where(n => string.Equals(n.Name.Trim().ToLower(), searchString.Trim().ToLower())).ToList();
                if (data1.Count != 0)
                    return View(data1);

                string formattedInput2 = searchString.Trim().ToLower();
                foreach (var item in ElectricalDevices)
                {
                    string itemType2 = item.ItemType.ToString().Trim().ToLower();
                    if (itemType2 == formattedInput2 || itemType2 + 's' == formattedInput2)
                    {
                        IEnumerable<Item> ElectricalData = ElectricalDevices.Where(x => x.ItemType == item.ItemType);
                        return View(ElectricalData);
                    }
                }
                var data2 = ElectricalDevices.Where(n => string.Equals(n.Name.Trim().ToLower(), searchString.Trim().ToLower(), StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (data2.Count != 0)
                    return View(data2);

                string formattedInput3 = searchString.Trim().ToLower();
                foreach (var item in Laptops)
                {
                    string itemType3 = item.ItemType.ToString().Trim().ToLower();
                    if (itemType3 == formattedInput3 || itemType3 + 's' == formattedInput3)
                    {
                        IEnumerable<Item> LaptopData = Laptops.Where(x => x.ItemType == item.ItemType);
                        return View(LaptopData);
                    }
                }
                var data3 = Laptops.Where(n => string.Equals(n.Name.Trim().ToLower(), searchString.Trim().ToLower(), StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (data3.Count != 0)
                    return View(data3);

                string formattedInput4 = searchString.Trim().ToLower();
                foreach (var item in MobilesAndTablets)
                {
                    string itemType4 = item.ItemType.ToString().Trim().ToLower();
                    if (itemType4 == formattedInput4 || itemType4 + 's' == formattedInput4)
                    {
                        IEnumerable<Item> MobileData = MobilesAndTablets.Where(x => x.ItemType == item.ItemType);
                        return View(MobileData);
                    }
                }
                var data4 = MobilesAndTablets.Where(n => string.Equals(n.Name.Trim().ToLower(), searchString.Trim().ToLower(), StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (data4.Count != 0)
                    return View(data4);

                return View();
            }

            return RedirectToAction("Index");
        }
    }
}