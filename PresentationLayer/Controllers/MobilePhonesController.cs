using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresentationLayer.ViewModels;
using PresentationLayer.ViewModels.ItemVMs;

namespace PresentationLayer.Controllers
{
    [AllowAnonymous]
    public class MobilePhonesController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _mobilePhone.GetSpecificCategoriesForSelectList();

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IMobilePhonesService _mobilePhone;
        private readonly IDepartmentsService _departments;

        public MobilePhonesController(IMobilePhonesService mobilePhone, IDepartmentsService departments)
        {
            _mobilePhone = mobilePhone;
            _departments = departments;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = _mobilePhone.GetMobilePhonesWithRelatedOnes();

            var mobilePhonesVM = new ItemViewModel<MobilePhoneViewModel>()
            {
                ItemCategories = result.ItemCategories,
                DiscountedItems = result.DiscountedItems
                .Select(p => new MobilePhoneViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Rate = p.Rate,
                    Price = p.Price,
                    NewPrice = p.NewPrice,
                    IsDiscounted = p.IsDiscounted,
                    imageSrc = p.imageSrc,
                    RAM = p.RAM,
                    OperatingSystem = p.OperatingSystem,
                    CPUModel = p.CPUModel,
                    MemoryStorageCapacity = p.MemoryStorageCapacity,
                    Color = p.Color,
                    isLiked = p.isLiked,
                    CategoryName = p.CategoryName,
                    RateCount = p.RateCount
                }),
                latestItems = result.latestItems
                .Select(p => new MobilePhoneViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Rate = p.Rate,
                    Price = p.Price,
                    NewPrice = p.NewPrice,
                    IsDiscounted = p.IsDiscounted,
                    imageSrc = p.imageSrc,
                    RAM = p.RAM,
                    OperatingSystem = p.OperatingSystem,
                    CPUModel = p.CPUModel,
                    MemoryStorageCapacity = p.MemoryStorageCapacity,
                    Color = p.Color,
                    isLiked = p.isLiked,
                    CategoryName = p.CategoryName,
                    RateCount = p.RateCount
                }),
                TopRatedItems = result.TopRatedItems
                .Select(p => new MobilePhoneViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Rate = p.Rate,
                    Price = p.Price,
                    NewPrice = p.NewPrice,
                    IsDiscounted = p.IsDiscounted,
                    imageSrc = p.imageSrc,
                    RAM = p.RAM,
                    OperatingSystem = p.OperatingSystem,
                    CPUModel = p.CPUModel,
                    MemoryStorageCapacity = p.MemoryStorageCapacity,
                    Color = p.Color,
                    isLiked = p.isLiked,
                    CategoryName = p.CategoryName,
                    RateCount = p.RateCount
                }),
                Offers = result.Offers ?? Enumerable.Empty<Offer>().AsQueryable()
            };

            return View(mobilePhonesVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var mobilePhones = _mobilePhone.GetMobilePhones(pageNumber, pageSize);

            return View(mobilePhones);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            await CreateCategoriesSelectList();

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Add(MobilePhone data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _mobilePhone.Add(data);

                return RedirectToAction(nameof(IndexAdmin));
            }

            return View(data);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id == null && id != 0)
                throw new ArgumentNullException("Invalid id!!");

            var MobilePhone = await _mobilePhone.GetMobilePhone(id);

            if (MobilePhone != null)
            {
                await CreateCategoriesSelectList();
                return View(MobilePhone);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(MobilePhone data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _mobilePhone.Update(data);

                return RedirectToAction(nameof(IndexAdmin));
            }

            return View(data);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id == null && id != 0)
                throw new ArgumentNullException("Invalid id!!");

            var MobilePhone = await _mobilePhone.GetMobilePhone(id);

            if (MobilePhone != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(MobilePhone data)
        {
            await _mobilePhone.Delete(data);

            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> MobilePhones()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            return View();
        }

        public async Task<IActionResult> Brands(string? orderIndex, int? page, string name, bool? des)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var departments = await _departments.GetDepartments();
                ViewData["Departments"] = departments;

                var result = await _mobilePhone.GetBrandsMobilePhones(orderIndex, page, name, des);

                var data = new ItemsViewModel
                {
                    Items = result.Items,
                    CurrentPage = result.CurrentPage,
                    TotalPages = result.TotalPages,
                    OrderIndex = result.OrderIndex,
                    Des = result.Des,
                    ActionName = result.ActionName,
                    Brand = result.Brand
                };

                return View("MobilePhones", data);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Discounted(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _mobilePhone.GetDiscountedMobilePhones(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("MobilePhones", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _mobilePhone.GetTopRatedMobilePhones(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("MobilePhones", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _mobilePhone.GetLatestMobilePhones(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("MobilePhones", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _mobilePhone.GetMobilePhonesWithPriceFilter(orderIndex, page, price1, price2, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
                Price1 = result.Price1,
                Price2 = result.Price2
            };

            return View("MobilePhones", data);
        }

        public async Task<IActionResult> StorageFilter(string? orderIndex, int? page, int storage, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _mobilePhone.GetMobilePhonesWithStorageFilter(orderIndex, page, storage, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
                Price1 = result.Price1,
                Price2 = result.Price2
            };

            return View("MobilePhones", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _mobilePhone.GetMobilePhoneDetails(id);

                if (result != null)
                {
                    var mobilePhones = new MobilePhoneViewModel
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Rate = result.Rate,
                        Price = result.Price,
                        NewPrice = result.NewPrice,
                        IsDiscounted = result.IsDiscounted,
                        DiscountValue = result.DiscountValue,
                        IsBOGOBuy = result.IsBOGOBuy,
                        IsBOGOGet = result.IsBOGOGet,
                        imageSrc = result.imageSrc,
                        RAM = result.RAM,
                        OperatingSystem = result.OperatingSystem,
                        CPUModel = result.CPUModel,
                        MemoryStorageCapacity = result.MemoryStorageCapacity,
                        Color = result.Color,
                        CategoryName = result.CategoryName,
                        RelatedPhones = result.RelatedPhones
                        .Select(p => new MobilePhoneViewModel
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Rate = p.Rate,
                            Price = p.Price,
                            NewPrice = p.NewPrice,
                            IsDiscounted = p.IsDiscounted,
                            imageSrc = p.imageSrc,
                            RAM = p.RAM,
                            OperatingSystem = p.OperatingSystem,
                            CPUModel = p.CPUModel,
                            MemoryStorageCapacity = p.MemoryStorageCapacity,
                            Color = p.Color,
                            isLiked = p.isLiked,
                            CategoryName = p.CategoryName,
                            RateCount = p.RateCount
                        }),
                        SimilarPricePhones = result.SimilarPricePhones
                        .Select(p => new MobilePhoneViewModel
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Rate = p.Rate,
                            Price = p.Price,
                            NewPrice = p.NewPrice,
                            IsDiscounted = p.IsDiscounted,
                            imageSrc = p.imageSrc,
                            RAM = p.RAM,
                            OperatingSystem = p.OperatingSystem,
                            CPUModel = p.CPUModel,
                            MemoryStorageCapacity = p.MemoryStorageCapacity,
                            Color = p.Color,
                            isLiked = p.isLiked,
                            CategoryName = p.CategoryName,
                            RateCount = p.RateCount
                        }),
                        Comments = result.Comments,
                        Offers = result.Offers,
                        BOGOGet = result.BOGOGet,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount,
                        ControllerName = result.ControllerName,
                        TotalQuantity = result.TotalQuantity
                    };

                    return View(mobilePhones);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllMobilePhoneComments(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _mobilePhone.GetMobilePhoneAllComments(id);

                if (result is not null)
                {
                    var mobilePhone = new MobilePhoneViewModel
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Rate = result.Rate,
                        CategoryName = result.CategoryName,
                        Comments = result.Comments,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount
                    };

                    return View("AllComments", mobilePhone);
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}