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
    public class AirConditionersController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _airConditioner.GetSpecificCategoriesForSelectList();

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IAirConditionersService _airConditioner;
        private readonly IDepartmentsService _departments;

        public AirConditionersController(IAirConditionersService airConditioner, IDepartmentsService departments)
        {
            _airConditioner = airConditioner;
            _departments = departments;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = _airConditioner.GetAirConditionersWithRelatedOnes();

            var airConditionersVM = new ItemViewModel<AirConditionerViewModel>()
            {
                ItemCategories = result.ItemCategories,
                DiscountedItems = result.DiscountedItems
                .Select(a => new AirConditionerViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = a.isLiked,
                    CategoryName = a.CategoryName,
                    RateCount = a.RateCount,
                }),
                latestItems = result.latestItems
                .Select(a => new AirConditionerViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = a.isLiked,
                    CategoryName = a.CategoryName,
                    RateCount = a.RateCount,
                }),
                TopRatedItems = result.TopRatedItems
                .Select(a => new AirConditionerViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = a.isLiked,
                    CategoryName = a.CategoryName,
                    RateCount = a.RateCount,
                }),
                Offers = result.Offers ?? Enumerable.Empty<Offer>().AsQueryable()
            };

            return View(airConditionersVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var airConditioners = _airConditioner.GetAirConditioners(pageNumber, pageSize);

            return View(airConditioners);
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
        public async Task<IActionResult> Add(AirConditioner data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _airConditioner.Add(data);

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

            var AirConditioner = await _airConditioner.GetAirConditioner(id);

            if (AirConditioner != null)
            {
                await CreateCategoriesSelectList();
                return View(AirConditioner);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(AirConditioner data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _airConditioner.Update(data);

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

            var AirConditioner = await _airConditioner.GetAirConditioner(id);

            if (AirConditioner != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(AirConditioner data)
        {
            await _airConditioner.Delete(data);

            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> AirConditioners()
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

                var result = await _airConditioner.GetBrandsAirConditioners(orderIndex, page, name, des);

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

                return View("AirConditioners", data);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Discounted(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _airConditioner.GetDiscountedAirConditioners(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("AirConditioners", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _airConditioner.GetTopRatedAirConditioners(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("AirConditioners", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _airConditioner.GetLatestAirConditioners(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("AirConditioners", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _airConditioner.GetAirConditionersWithPriceFilter(orderIndex, page, price1, price2, des);

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

            return View("AirConditioners", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _airConditioner.GetAirConditionerDetails(id);

                if (result != null)
                {
                    var airConditioners = new AirConditionerViewModel
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
                        Color = result.Color,
                        Capacity = result.Capacity,
                        CoolingPower = result.CoolingPower,
                        Voltage = result.Voltage,
                        ItemDimensions = result.ItemDimensions,
                        NoiseLevel = result.NoiseLevel,
                        SpecialFeatures = result.SpecialFeatures,
                        CategoryName = result.CategoryName,
                        RelatedAirConditioners = result.RelatedAirConditioners
                        .Select(a => new AirConditionerViewModel
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Rate = a.Rate,
                            Price = a.Price,
                            NewPrice = a.NewPrice,
                            imageSrc = a.imageSrc,
                            Color = a.Color,
                            Capacity = a.Capacity,
                            CoolingPower = a.CoolingPower,
                            Voltage = a.Voltage,
                            ItemDimensions = a.ItemDimensions,
                            NoiseLevel = a.NoiseLevel,
                            SpecialFeatures = a.SpecialFeatures,
                            isLiked = a.isLiked,
                            CategoryName = a.CategoryName,
                            RateCount = a.RateCount
                        }),
                        SimilarPriceAirConditioners = result.SimilarPriceAirConditioners
                         .Select(a => new AirConditionerViewModel
                         {
                             Id = a.Id,
                             Name = a.Name,
                             Rate = a.Rate,
                             Price = a.Price,
                             NewPrice = a.NewPrice,
                             imageSrc = a.imageSrc,
                             Color = a.Color,
                             Capacity = a.Capacity,
                             CoolingPower = a.CoolingPower,
                             Voltage = a.Voltage,
                             ItemDimensions = a.ItemDimensions,
                             NoiseLevel = a.NoiseLevel,
                             SpecialFeatures = a.SpecialFeatures,
                             isLiked = a.isLiked,
                             CategoryName = a.CategoryName,
                             RateCount = a.RateCount
                         }),
                        Comments = result.Comments,
                        Offers = result.Offers,
                        BOGOGet = result.BOGOGet,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount,
                        ControllerName = result.ControllerName,
                        TotalQuantity = result.TotalQuantity
                    };

                    return View(airConditioners);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllAirConditionerComments(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _airConditioner.GetAirConditionerAllComments(id);

                if (result is not null)
                {
                    var airConditioner = new AirConditionerViewModel
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Rate = result.Rate,
                        CategoryName = result.CategoryName,
                        Comments = result.Comments,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount
                    };

                    return View("AllComments", airConditioner);
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}