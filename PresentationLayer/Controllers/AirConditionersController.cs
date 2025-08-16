using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PresentationLayer.ViewModels;
using PresentationLayer.ViewModels.ItemVMs;

namespace PresentationLayer.Controllers
{
    [AllowAnonymous]
    public class AirConditionersController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _unitOfWork.Categories.GetSpecificCategories("Appliances");

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public AirConditionersController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var airConditionersCategories = await _unitOfWork.AirConditioners.GetItemCategories().Result.ToListAsync();

            var discountedAirConditioners = _unitOfWork.AirConditioners.GetDiscountedItems(1, 10, "ID", false).Result.ToList().
                Select(a => new AirConditionerViewModel
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, a.ID, "AirConditioners"),
                    CategoryName = a.Category.Name,
                    RateCount = _unitOfWork.AirConditioners.GetItemRates(a.ID, "AirConditioners").Result.Count()
                }).OrderBy(a => Guid.NewGuid()).ToList();

            var topRatedAirConditioners = _unitOfWork.AirConditioners.GetTopRatedItems(1, 10, "ID", false).Result.ToList().
                Select(a => new AirConditionerViewModel
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, a.ID, "AirConditioners"),
                    CategoryName = a.Category.Name,
                    RateCount = _unitOfWork.AirConditioners.GetItemRates(a.ID, "AirConditioners").Result.Count()
                }).OrderBy(a => Guid.NewGuid()).ToList();

            var latestAirConditioners = _unitOfWork.AirConditioners.GetLatestItems(1, 10, "ID", false).Result.ToList().
                Select(a => new AirConditionerViewModel
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, a.ID, "AirConditioners"),
                    CategoryName = a.Category.Name,
                    RateCount = _unitOfWork.AirConditioners.GetItemRates(a.ID, "AirConditioners").Result.Count()
                }).OrderBy(a => Guid.NewGuid()).ToList();

            var airConditionersVM = new ItemViewModel<AirConditionerViewModel>()
            {
                ItemCategories = airConditionersCategories,
                DiscountedItems = discountedAirConditioners,
                latestItems = latestAirConditioners,
                TopRatedItems = topRatedAirConditioners,
            };

            return View(airConditionersVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var airConditioners = await _unitOfWork.AirConditioners.GetAll(pageNumber, pageSize);

            return View(airConditioners);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
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
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.AirConditioners.Add(data);
                await _unitOfWork.Commit();
                return RedirectToAction(nameof(IndexAdmin));
            }

            return View(data);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id == null && id != 0)
                throw new ArgumentNullException("Invalid id!!");

            var AirConditioner = await _unitOfWork.AirConditioners.GetById(id);

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
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.AirConditioners.Update(data);
                await _unitOfWork.Commit();
                return RedirectToAction(nameof(IndexAdmin));
            }

            return View(data);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id == null && id != 0)
                throw new ArgumentNullException("Invalid id!!");

            var AirConditioner = await _unitOfWork.AirConditioners.GetById(id);

            if (AirConditioner != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(AirConditioner data)
        {
            await _unitOfWork.AirConditioners.Delete(data);
            await _unitOfWork.Commit();
            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> AirConditioners()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            return View();
        }

        public async Task<IActionResult> Brands(string? orderIndex, int? page, string name, bool? des)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
                ViewData["Departments"] = departments;

                bool desOrder = des ?? false;
                int pageSize = 9;
                int pageNumber = page ?? 1;
                var totalPages = (int)Math.Ceiling(await _unitOfWork.AirConditioners.TotalItems("Brands", null, null, name) / (double)pageSize);

                var airConditioners = _unitOfWork.AirConditioners.GetCategoryItems(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                    Select(a => new AirConditionerViewModel
                    {
                        Id = a.ID,
                        Name = a.Name,
                        Rate = a.Rate,
                        Price = a.Price,
                        NewPrice = a.NewPrice ?? 0,
                        imageSrc = a.imageSrc,
                        Color = a.Color,
                        Capacity = a.Capacity,
                        CoolingPower = a.CoolingPower,
                        Voltage = a.Voltage,
                        ItemDimensions = a.ItemDimensions,
                        NoiseLevel = a.NoiseLevel,
                        SpecialFeatures = a.SpecialFeatures,
                        isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, a.ID, "AirConditioners"),
                        CategoryName = a.Category.Name,
                        RateCount = _unitOfWork.AirConditioners.GetItemRates(a.ID, "AirConditioners").Result.Count()
                    }).ToList();

                var data = new ItemsViewModel
                {
                    Items = airConditioners,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    OrderIndex = orderIndex,
                    Des = des,
                    ActionName = "Brands",
                    Brand = name
                };
                return View("AirConditioners", data);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Discounted(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.AirConditioners.TotalItems("Discounted") / (double)pageSize);

            var discountedAirConditioners = _unitOfWork.AirConditioners.GetDiscountedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(a => new AirConditionerViewModel
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, a.ID, "AirConditioners"),
                    CategoryName = a.Category.Name,
                    RateCount = _unitOfWork.AirConditioners.GetItemRates(a.ID, "AirConditioners").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = discountedAirConditioners,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
            return View("AirConditioners", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.AirConditioners.TotalItems("Rated") / (double)pageSize);


            var ratedAirConditioners = _unitOfWork.AirConditioners.GetTopRatedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(a => new AirConditionerViewModel
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, a.ID, "AirConditioners"),
                    CategoryName = a.Category.Name,
                    RateCount = _unitOfWork.AirConditioners.GetItemRates(a.ID, "AirConditioners").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = ratedAirConditioners,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
            return View("AirConditioners", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.AirConditioners.TotalItems("Latest") / (double)pageSize);

            var latestAirConditioners = _unitOfWork.AirConditioners.GetLatestItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(a => new AirConditionerViewModel
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, a.ID, "AirConditioners"),
                    CategoryName = a.Category.Name,
                    RateCount = _unitOfWork.AirConditioners.GetItemRates(a.ID, "AirConditioners").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = latestAirConditioners,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
            return View("AirConditioners", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.AirConditioners.TotalItems("Price", price1, price2, null) / (double)pageSize);

            var priceAirConditioners = _unitOfWork.AirConditioners.GetItemsFilteredByPrice(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(a => new AirConditionerViewModel
                {
                    Id = a.ID,
                    Name = a.Name,
                    Rate = a.Rate,
                    Price = a.Price,
                    NewPrice = a.NewPrice ?? 0,
                    imageSrc = a.imageSrc,
                    Color = a.Color,
                    Capacity = a.Capacity,
                    CoolingPower = a.CoolingPower,
                    Voltage = a.Voltage,
                    ItemDimensions = a.ItemDimensions,
                    NoiseLevel = a.NoiseLevel,
                    SpecialFeatures = a.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, a.ID, "AirConditioners"),
                    CategoryName = a.Category.Name,
                    RateCount = _unitOfWork.AirConditioners.GetItemRates(a.ID, "AirConditioners").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = priceAirConditioners,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
            return View("AirConditioners", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var AirConditioner = await _unitOfWork.AirConditioners.GetById(id);

                if (AirConditioner != null)
                {
                    var comments = await _unitOfWork.AirConditioners.GetItemComments(id, "AirConditioners", "Default");

                    var rateList = await _unitOfWork.AirConditioners.GetItemRates(id, "AirConditioners");
                    var rateCount = rateList.Count();

                    var starCounts = await _unitOfWork.AirConditioners.GetItemRateDetails(id, "AirConditioners");

                    var totalQuantity = await _unitOfWork.Carts.TotalItemQuantityInCart(id, "AirConditioners");

                    var similarPriceAirConditioners = _unitOfWork.AirConditioners.GetAllWithoutPagination().Result
                        .Where(a => a.Price == AirConditioner.Price || Math.Abs(a.Price - AirConditioner.Price) <= 1000).ToList().
                        Select(a => new AirConditionerViewModel
                        {
                            Id = a.ID,
                            Name = a.Name,
                            Rate = a.Rate,
                            Price = a.Price,
                            NewPrice = a.NewPrice ?? 0,
                            imageSrc = a.imageSrc,
                            Color = a.Color,
                            Capacity = a.Capacity,
                            CoolingPower = a.CoolingPower,
                            Voltage = a.Voltage,
                            ItemDimensions = a.ItemDimensions,
                            NoiseLevel = a.NoiseLevel,
                            SpecialFeatures = a.SpecialFeatures,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, a.ID, "AirConditioners"),
                            CategoryName = a.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var relatedAirConditioners = _unitOfWork.AirConditioners.GetAllWithoutPagination().Result
                        .Where(a => a.CategoryId == AirConditioner.CategoryId).Take(10).ToList().
                        Select(a => new AirConditionerViewModel
                        {
                            Id = a.ID,
                            Name = a.Name,
                            Rate = a.Rate,
                            Price = a.Price,
                            NewPrice = a.NewPrice ?? 0,
                            imageSrc = a.imageSrc,
                            Color = a.Color,
                            Capacity = a.Capacity,
                            CoolingPower = a.CoolingPower,
                            Voltage = a.Voltage,
                            ItemDimensions = a.ItemDimensions,
                            NoiseLevel = a.NoiseLevel,
                            SpecialFeatures = a.SpecialFeatures,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, a.ID, "AirConditioners"),
                            CategoryName = a.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var offers = await _unitOfWork.Offers.GetOffers("Appliances", AirConditioner.Category?.Name, AirConditioner.ID);

                    var discountValue = string.Empty;
                    if (offers.Any())
                    {
                        discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                           $"{offers.First().PercentDiscount}%" :
                           offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;
                    }

                    var BOGOGetItem = await _unitOfWork.Offers.GetBOGOGetItem(AirConditioner);

                    var airConditioner = new AirConditionerViewModel
                    {
                        Id = AirConditioner.ID,
                        Name = AirConditioner.Name,
                        Rate = AirConditioner.Rate,
                        Price = AirConditioner.Price,
                        NewPrice = AirConditioner.NewPrice ?? 0,
                        IsDiscounted = AirConditioner.IsDiscounted,
                        DiscountValue = discountValue,
                        IsBOGOBuy = AirConditioner.IsBOGOBuy,
                        IsBOGOGet = AirConditioner.IsBOGOGet,
                        imageSrc = AirConditioner.imageSrc,
                        Color = AirConditioner.Color,
                        Capacity = AirConditioner.Capacity,
                        CoolingPower = AirConditioner.CoolingPower,
                        Voltage = AirConditioner.Voltage,
                        ItemDimensions = AirConditioner.ItemDimensions,
                        NoiseLevel = AirConditioner.NoiseLevel,
                        SpecialFeatures = AirConditioner.SpecialFeatures,
                        CategoryName = AirConditioner.Category.Name,
                        RelatedAirConditioners = relatedAirConditioners,
                        SimilarPriceAirConditioners = similarPriceAirConditioners,
                        Comments = comments,
                        Offers = offers,
                        BOGOGet = BOGOGetItem,
                        StarCounts = starCounts,
                        RateCount = rateCount,
                        ControllerName = "AirConditioners",
                        TotalQuantity = totalQuantity
                    };

                    return View(airConditioner);

                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllAirConditionerComments(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var AirConditioner = await _unitOfWork.AirConditioners.GetById(id);

                var rateList = await _unitOfWork.AirConditioners.GetItemRates(id, "AirConditioners");
                var rateCount = rateList.Count();

                var starCounts = await _unitOfWork.AirConditioners.GetItemRateDetails(id, "AirConditioners");

                if (AirConditioner != null)
                {
                    var comments = await _unitOfWork.AirConditioners.GetItemComments(id, "AirConditioners", "All");

                    if (comments.Any())
                    {
                        var airConditioner = new AirConditionerViewModel
                        {
                            Id = AirConditioner.ID,
                            Name = AirConditioner.Name,
                            Rate = AirConditioner.Rate,
                            CategoryName = AirConditioner.Category.Name,
                            Comments = comments,
                            StarCounts = starCounts,
                            RateCount = rateCount
                        };

                        return View("AllComments", airConditioner);
                    }
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}
