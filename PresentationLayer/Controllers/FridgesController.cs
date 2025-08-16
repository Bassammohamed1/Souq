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
    public class FridgesController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _unitOfWork.Categories.GetSpecificCategories("Appliances");

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public FridgesController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var fridgesCategories = await _unitOfWork.Fridges.GetItemCategories().Result.ToListAsync();

            var discountedFridges = _unitOfWork.Fridges.GetDiscountedItems(1, 10, "ID", false).Result.ToList().
                Select(f => new FridgeViewModel
                {
                    Id = f.ID,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice ?? 0,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, f.ID, "Fridges"),
                    CategoryName = f.Category.Name,
                    RateCount = _unitOfWork.Fridges.GetItemRates(f.ID, "Fridges").Result.Count()
                }).OrderBy(f => Guid.NewGuid()).ToList();

            var topRatedFridges = _unitOfWork.Fridges.GetTopRatedItems(1, 10, "ID", false).Result.ToList().
                Select(f => new FridgeViewModel
                {
                    Id = f.ID,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice ?? 0,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, f.ID, "Fridges"),
                    CategoryName = f.Category.Name,
                    RateCount = _unitOfWork.Fridges.GetItemRates(f.ID, "Fridges").Result.Count()
                }).OrderBy(f => Guid.NewGuid()).ToList();

            var latestFridges = _unitOfWork.Fridges.GetLatestItems(1, 10, "ID", false).Result.ToList().
                Select(f => new FridgeViewModel
                {
                    Id = f.ID,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice ?? 0,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, f.ID, "Fridges"),
                    CategoryName = f.Category.Name,
                    RateCount = _unitOfWork.Fridges.GetItemRates(f.ID, "Fridges").Result.Count()
                }).OrderBy(p => Guid.NewGuid()).ToList();

            var fridgesVM = new ItemViewModel<FridgeViewModel>()
            {
                ItemCategories = fridgesCategories,
                DiscountedItems = discountedFridges,
                latestItems = latestFridges,
                TopRatedItems = topRatedFridges
            };

            return View(fridgesVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var fridges = await _unitOfWork.Fridges.GetAll(pageNumber, pageSize);

            return View(fridges);
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
        public async Task<IActionResult> Add(Fridge data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.Fridges.Add(data);
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

            var Fridge = await _unitOfWork.Fridges.GetById(id);

            if (Fridge != null)
            {
                await CreateCategoriesSelectList();
                return View(Fridge);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(Fridge data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.Fridges.Update(data);
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

            var Fridge = await _unitOfWork.Fridges.GetById(id);

            if (Fridge != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(Fridge data)
        {
            await _unitOfWork.Fridges.Delete(data);
            await _unitOfWork.Commit();
            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> Fridges()
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
                var totalPages = (int)Math.Ceiling(await _unitOfWork.Fridges.TotalItems("Brands", null, null, name) / (double)pageSize);

                var fridges = _unitOfWork.Fridges.GetCategoryItems(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                    Select(f => new FridgeViewModel
                    {
                        Id = f.ID,
                        Name = f.Name,
                        Rate = f.Rate,
                        Price = f.Price,
                        NewPrice = f.NewPrice ?? 0,
                        imageSrc = f.imageSrc,
                        Capacity = f.Capacity,
                        Color = f.Color,
                        DefrostSystem = f.DefrostSystem,
                        EnergyStar = f.EnergyStar,
                        InstallationType = f.InstallationType,
                        ItemDimensions = f.ItemDimensions,
                        NumberOfDoors = f.NumberOfDoors,
                        SpecialFeatures = f.SpecialFeatures,
                        isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, f.ID, "Fridges"),
                        CategoryName = f.Category.Name,
                        RateCount = _unitOfWork.Fridges.GetItemRates(f.ID, "Fridges").Result.Count()
                    }).ToList();

                var data = new ItemsViewModel
                {
                    Items = fridges,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    OrderIndex = orderIndex,
                    Des = des,
                    ActionName = "Brands",
                    Brand = name
                };
                return View("Fridges", data);
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
            var totalPages = (int)Math.Ceiling(await _unitOfWork.Fridges.TotalItems("Discounted") / (double)pageSize);

            var discountedFridges = _unitOfWork.Fridges.GetDiscountedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(f => new FridgeViewModel
                {
                    Id = f.ID,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice ?? 0,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, f.ID, "Fridges"),
                    CategoryName = f.Category.Name,
                    RateCount = _unitOfWork.Fridges.GetItemRates(f.ID, "Fridges").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = discountedFridges,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
            return View("Fridges", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.Fridges.TotalItems("Rated") / (double)pageSize);

            var ratedFridges = _unitOfWork.Fridges.GetTopRatedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(f => new FridgeViewModel
                {
                    Id = f.ID,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice ?? 0,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, f.ID, "Fridges"),
                    CategoryName = f.Category.Name,
                    RateCount = _unitOfWork.Fridges.GetItemRates(f.ID, "Fridges").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = ratedFridges,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
            return View("Fridges", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.Fridges.TotalItems("Latest") / (double)pageSize);

            var latestFridges = _unitOfWork.Fridges.GetLatestItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(f => new FridgeViewModel
                {
                    Id = f.ID,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice ?? 0,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, f.ID, "Fridges"),
                    CategoryName = f.Category.Name,
                    RateCount = _unitOfWork.Fridges.GetItemRates(f.ID, "Fridges").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = latestFridges,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
            return View("Fridges", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.Fridges.TotalItems("Price", price1, price2, null) / (double)pageSize);

            var priceFridges = _unitOfWork.Fridges.GetItemsFilteredByPrice(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(f => new FridgeViewModel
                {
                    Id = f.ID,
                    Name = f.Name,
                    Rate = f.Rate,
                    Price = f.Price,
                    NewPrice = f.NewPrice ?? 0,
                    imageSrc = f.imageSrc,
                    Capacity = f.Capacity,
                    Color = f.Color,
                    DefrostSystem = f.DefrostSystem,
                    EnergyStar = f.EnergyStar,
                    InstallationType = f.InstallationType,
                    ItemDimensions = f.ItemDimensions,
                    NumberOfDoors = f.NumberOfDoors,
                    SpecialFeatures = f.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, f.ID, "Fridges"),
                    CategoryName = f.Category.Name,
                    RateCount = _unitOfWork.Fridges.GetItemRates(f.ID, "Fridges").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = priceFridges,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
            return View("Fridges", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var Fridge = await _unitOfWork.Fridges.GetById(id);

                if (Fridge != null)
                {
                    var comments = await _unitOfWork.Fridges.GetItemComments(id, "Fridges", "Default");

                    var rateList = await _unitOfWork.Fridges.GetItemRates(id, "Fridges");
                    var rateCount = rateList.Count();

                    var starCounts = await _unitOfWork.Fridges.GetItemRateDetails(id, "Fridges");

                    var totalQuantity = await _unitOfWork.Carts.TotalItemQuantityInCart(id, "Fridges");

                    var similarPriceFridges = _unitOfWork.Fridges.GetAllWithoutPagination().Result.
                        Where(f => f.Price == Fridge.Price || Math.Abs(f.Price - Fridge.Price) <= 1000).ToList().
                             Select(f => new FridgeViewModel
                             {
                                 Id = f.ID,
                                 Name = f.Name,
                                 Rate = f.Rate,
                                 Price = f.Price,
                                 NewPrice = f.NewPrice ?? 0,
                                 imageSrc = f.imageSrc,
                                 Capacity = f.Capacity,
                                 Color = f.Color,
                                 DefrostSystem = f.DefrostSystem,
                                 EnergyStar = f.EnergyStar,
                                 InstallationType = f.InstallationType,
                                 ItemDimensions = f.ItemDimensions,
                                 NumberOfDoors = f.NumberOfDoors,
                                 SpecialFeatures = f.SpecialFeatures,
                                 isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, f.ID, "Fridges"),
                                 CategoryName = f.Category.Name,
                                 RateCount = rateCount
                             }).ToList();

                    var relatedFridges = _unitOfWork.Fridges.GetAllWithoutPagination().Result
                        .Where(m => m.CategoryId == Fridge.CategoryId).Take(10).ToList().
                             Select(f => new FridgeViewModel
                             {
                                 Id = f.ID,
                                 Name = f.Name,
                                 Rate = f.Rate,
                                 Price = f.Price,
                                 NewPrice = f.NewPrice ?? 0,
                                 imageSrc = f.imageSrc,
                                 Capacity = f.Capacity,
                                 Color = f.Color,
                                 DefrostSystem = f.DefrostSystem,
                                 EnergyStar = f.EnergyStar,
                                 InstallationType = f.InstallationType,
                                 ItemDimensions = f.ItemDimensions,
                                 NumberOfDoors = f.NumberOfDoors,
                                 SpecialFeatures = f.SpecialFeatures,
                                 isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, f.ID, "Fridges"),
                                 CategoryName = f.Category.Name,
                                 RateCount = rateCount
                             }).ToList();

                    var offers = await _unitOfWork.Offers.GetOffers("Appliances", Fridge.Category?.Name, Fridge.ID);

                    var discountValue = string.Empty;
                    if (offers.Any())
                    {
                        discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                           $"{offers.First().PercentDiscount}%" :
                           offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;
                    }

                    var BOGOGetItem = await _unitOfWork.Offers.GetBOGOGetItem(Fridge);
                    var fridge = new FridgeViewModel
                    {
                        Id = Fridge.ID,
                        Name = Fridge.Name,
                        Rate = Fridge.Rate,
                        Price = Fridge.Price,
                        NewPrice = Fridge.NewPrice ?? 0,
                        IsDiscounted = Fridge.IsDiscounted,
                        DiscountValue = discountValue,
                        IsBOGOBuy = Fridge.IsBOGOBuy,
                        IsBOGOGet = Fridge.IsBOGOGet,
                        imageSrc = Fridge.imageSrc,
                        Capacity = Fridge.Capacity,
                        Color = Fridge.Color,
                        DefrostSystem = Fridge.DefrostSystem,
                        EnergyStar = Fridge.EnergyStar,
                        InstallationType = Fridge.InstallationType,
                        ItemDimensions = Fridge.ItemDimensions,
                        NumberOfDoors = Fridge.NumberOfDoors,
                        SpecialFeatures = Fridge.SpecialFeatures,
                        CategoryName = Fridge.Category.Name,
                        RelatedFridges = relatedFridges,
                        SimilarPriceFridges = similarPriceFridges,
                        Comments = comments,
                        Offers = offers,
                        BOGOGet = BOGOGetItem,
                        StarCounts = starCounts,
                        RateCount = rateCount,
                        ControllerName = "Fridges",
                        TotalQuantity = totalQuantity
                    };

                    return View(fridge);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllFridgeComments(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var Fridge = await _unitOfWork.Fridges.GetById(id);

                var rateList = await _unitOfWork.Fridges.GetItemRates(id, "Fridges");
                var rateCount = rateList.Count();

                var starCounts = await _unitOfWork.Fridges.GetItemRateDetails(id, "Fridges");

                if (Fridge != null)
                {
                    var comments = await _unitOfWork.Fridges.GetItemComments(id, "Fridges", "All");

                    if (comments.Any())
                    {
                        var fridge = new FridgeViewModel
                        {
                            Id = Fridge.ID,
                            Name = Fridge.Name,
                            Rate = Fridge.Rate,
                            CategoryName = Fridge.Category.Name,
                            Comments = comments,
                            StarCounts = starCounts,
                            RateCount = rateCount
                        };

                        return View("AllComments", fridge);
                    }
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}
