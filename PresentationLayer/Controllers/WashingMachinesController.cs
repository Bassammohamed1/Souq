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
    public class WashingMachinesController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _unitOfWork.Categories.GetSpecificCategories("Appliances");

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public WashingMachinesController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var washingMachinesCategories = await _unitOfWork.WashingMachines.GetItemCategories().Result.ToListAsync();

            var discountedWashingMachines = _unitOfWork.WashingMachines.GetDiscountedItems(1, 10, "ID", false).Result.ToList().
                Select(w => new WashingMachineViewModel
                {
                    Id = w.ID,
                    Name = w.Name,
                    Rate = w.Rate,
                    Price = w.Price,
                    NewPrice = w.NewPrice ?? 0,
                    imageSrc = w.imageSrc,
                    Capacity = w.Capacity,
                    Color = w.Color,
                    CycleOptions = w.CycleOptions,
                    ItemDimensions = w.ItemDimensions,
                    ItemWeight = w.ItemWeight,
                    SpecialFeatures = w.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, w.ID, "WashingMachines"),
                    CategoryName = w.Category.Name,
                    RateCount = _unitOfWork.WashingMachines.GetItemRates(w.ID, "WashingMachines").Result.Count()
                }).OrderBy(w => Guid.NewGuid()).ToList();

            var topRatedWashingMachines = _unitOfWork.WashingMachines.GetTopRatedItems(1, 10, "ID", false).Result.ToList().
                Select(w => new WashingMachineViewModel
                {
                    Id = w.ID,
                    Name = w.Name,
                    Rate = w.Rate,
                    Price = w.Price,
                    NewPrice = w.NewPrice ?? 0,
                    imageSrc = w.imageSrc,
                    Capacity = w.Capacity,
                    Color = w.Color,
                    CycleOptions = w.CycleOptions,
                    ItemDimensions = w.ItemDimensions,
                    ItemWeight = w.ItemWeight,
                    SpecialFeatures = w.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, w.ID, "WashingMachines"),
                    CategoryName = w.Category.Name,
                    RateCount = _unitOfWork.WashingMachines.GetItemRates(w.ID, "WashingMachines").Result.Count()
                }).OrderBy(w => Guid.NewGuid()).ToList();

            var latestWashingMachines = _unitOfWork.WashingMachines.GetLatestItems(1, 10, "ID", false).Result.ToList().
                Select(w => new WashingMachineViewModel
                {
                    Id = w.ID,
                    Name = w.Name,
                    Rate = w.Rate,
                    Price = w.Price,
                    NewPrice = w.NewPrice ?? 0,
                    imageSrc = w.imageSrc,
                    Capacity = w.Capacity,
                    Color = w.Color,
                    CycleOptions = w.CycleOptions,
                    ItemDimensions = w.ItemDimensions,
                    ItemWeight = w.ItemWeight,
                    SpecialFeatures = w.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, w.ID, "WashingMachines"),
                    CategoryName = w.Category.Name,
                    RateCount = _unitOfWork.WashingMachines.GetItemRates(w.ID, "WashingMachines").Result.Count()
                }).OrderBy(w => Guid.NewGuid()).ToList();

            var washingMachinesVM = new ItemViewModel<WashingMachineViewModel>()
            {
                ItemCategories = washingMachinesCategories,
                DiscountedItems = discountedWashingMachines,
                latestItems = latestWashingMachines,
                TopRatedItems = topRatedWashingMachines
            };

            return View(washingMachinesVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var washingMachines = await _unitOfWork.WashingMachines.GetAll(pageNumber, pageSize);

            return View(washingMachines);
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
        public async Task<IActionResult> Add(WashingMachine data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.WashingMachines.Add(data);
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

            var WashingMachine = await _unitOfWork.WashingMachines.GetById(id);

            if (WashingMachine != null)
            {
                await CreateCategoriesSelectList();
                return View(WashingMachine);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(WashingMachine data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.WashingMachines.Update(data);
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

            var WashingMachine = await _unitOfWork.WashingMachines.GetById(id);

            if (WashingMachine != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(WashingMachine data)
        {
            await _unitOfWork.WashingMachines.Delete(data);
            await _unitOfWork.Commit();
            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> WashingMachines()
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
                var totalPages = (int)Math.Ceiling(await _unitOfWork.WashingMachines.TotalItems("Brands", null, null, name) / (double)pageSize);

                var washingMachines = _unitOfWork.WashingMachines.GetCategoryItems(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                     Select(w => new WashingMachineViewModel
                     {
                         Id = w.ID,
                         Name = w.Name,
                         Rate = w.Rate,
                         Price = w.Price,
                         NewPrice = w.NewPrice ?? 0,
                         imageSrc = w.imageSrc,
                         Capacity = w.Capacity,
                         Color = w.Color,
                         CycleOptions = w.CycleOptions,
                         ItemDimensions = w.ItemDimensions,
                         ItemWeight = w.ItemWeight,
                         SpecialFeatures = w.SpecialFeatures,
                         isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, w.ID, "WashingMachines"),
                         CategoryName = w.Category.Name,
                         RateCount = _unitOfWork.WashingMachines.GetItemRates(w.ID, "WashingMachines").Result.Count()
                     }).ToList();

                var data = new ItemsViewModel
                {
                    Items = washingMachines,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    OrderIndex = orderIndex,
                    Des = des,
                    ActionName = "Brands",
                    Brand = name
                };
                return View("WashingMachines", data);
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
            var totalPages = (int)Math.Ceiling(await _unitOfWork.WashingMachines.TotalItems("Discounted") / (double)pageSize);

            var discountedWashingMachines = _unitOfWork.WashingMachines.GetDiscountedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(w => new WashingMachineViewModel
                 {
                     Id = w.ID,
                     Name = w.Name,
                     Rate = w.Rate,
                     Price = w.Price,
                     NewPrice = w.NewPrice ?? 0,
                     imageSrc = w.imageSrc,
                     Capacity = w.Capacity,
                     Color = w.Color,
                     CycleOptions = w.CycleOptions,
                     ItemDimensions = w.ItemDimensions,
                     ItemWeight = w.ItemWeight,
                     SpecialFeatures = w.SpecialFeatures,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, w.ID, "WashingMachines"),
                     CategoryName = w.Category.Name,
                     RateCount = _unitOfWork.WashingMachines.GetItemRates(w.ID, "WashingMachines").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = discountedWashingMachines,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
            return View("WashingMachines", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.WashingMachines.TotalItems("Rated") / (double)pageSize);

            var ratedWashingMachines = _unitOfWork.WashingMachines.GetTopRatedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(w => new WashingMachineViewModel
                 {
                     Id = w.ID,
                     Name = w.Name,
                     Rate = w.Rate,
                     Price = w.Price,
                     NewPrice = w.NewPrice ?? 0,
                     imageSrc = w.imageSrc,
                     Capacity = w.Capacity,
                     Color = w.Color,
                     CycleOptions = w.CycleOptions,
                     ItemDimensions = w.ItemDimensions,
                     ItemWeight = w.ItemWeight,
                     SpecialFeatures = w.SpecialFeatures,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, w.ID, "WashingMachines"),
                     CategoryName = w.Category.Name,
                     RateCount = _unitOfWork.WashingMachines.GetItemRates(w.ID, "WashingMachines").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = ratedWashingMachines,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
            return View("WashingMachines", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.WashingMachines.TotalItems("Latest") / (double)pageSize);

            var latestWashingMachines = _unitOfWork.WashingMachines.GetLatestItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(w => new WashingMachineViewModel
                 {
                     Id = w.ID,
                     Name = w.Name,
                     Rate = w.Rate,
                     Price = w.Price,
                     NewPrice = w.NewPrice ?? 0,
                     imageSrc = w.imageSrc,
                     Capacity = w.Capacity,
                     Color = w.Color,
                     CycleOptions = w.CycleOptions,
                     ItemDimensions = w.ItemDimensions,
                     ItemWeight = w.ItemWeight,
                     SpecialFeatures = w.SpecialFeatures,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, w.ID, "WashingMachines"),
                     CategoryName = w.Category.Name,
                     RateCount = _unitOfWork.WashingMachines.GetItemRates(w.ID, "WashingMachines").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = latestWashingMachines,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
            return View("WashingMachines", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.WashingMachines.TotalItems("Price", price1, price2, null) / (double)pageSize);

            var priceWashingMachines = _unitOfWork.WashingMachines.GetItemsFilteredByPrice(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(w => new WashingMachineViewModel
                 {
                     Id = w.ID,
                     Name = w.Name,
                     Rate = w.Rate,
                     Price = w.Price,
                     NewPrice = w.NewPrice ?? 0,
                     imageSrc = w.imageSrc,
                     Capacity = w.Capacity,
                     Color = w.Color,
                     CycleOptions = w.CycleOptions,
                     ItemDimensions = w.ItemDimensions,
                     ItemWeight = w.ItemWeight,
                     SpecialFeatures = w.SpecialFeatures,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, w.ID, "WashingMachines"),
                     CategoryName = w.Category.Name,
                     RateCount = _unitOfWork.WashingMachines.GetItemRates(w.ID, "WashingMachines").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = priceWashingMachines,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
            return View("WashingMachines", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var WashingMachine = await _unitOfWork.WashingMachines.GetById(id);

                if (WashingMachine != null)
                {
                    var comments = await _unitOfWork.WashingMachines.GetItemComments(id, "WashingMachines", "Default");

                    var rateList = await _unitOfWork.WashingMachines.GetItemRates(id, "WashingMachines");
                    var rateCount = rateList.Count();

                    var starCounts = await _unitOfWork.WashingMachines.GetItemRateDetails(id, "WashingMachines");

                    var totalQuantity = await _unitOfWork.Carts.TotalItemQuantityInCart(id, "WashingMachines");

                    var similarPriceWashingMachines = _unitOfWork.WashingMachines.GetAllWithoutPagination().Result.
                        Where(w => w.Price == WashingMachine.Price || Math.Abs(w.Price - WashingMachine.Price) <= 1000).ToList()
                        .Select(w => new WashingMachineViewModel
                        {
                            Id = w.ID,
                            Name = w.Name,
                            Rate = w.Rate,
                            Price = w.Price,
                            NewPrice = w.NewPrice ?? 0,
                            imageSrc = w.imageSrc,
                            Capacity = w.Capacity,
                            Color = w.Color,
                            CycleOptions = w.CycleOptions,
                            ItemDimensions = w.ItemDimensions,
                            ItemWeight = w.ItemWeight,
                            SpecialFeatures = w.SpecialFeatures,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, w.ID, "WashingMachines"),
                            CategoryName = w.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var relatedWashingMachines = _unitOfWork.WashingMachines.GetAllWithoutPagination().Result
                        .Where(w => w.CategoryId == WashingMachine.CategoryId).Take(10).ToList().
                        Select(w => new WashingMachineViewModel
                        {
                            Id = w.ID,
                            Name = w.Name,
                            Rate = w.Rate,
                            Price = w.Price,
                            NewPrice = w.NewPrice ?? 0,
                            imageSrc = w.imageSrc,
                            Capacity = w.Capacity,
                            Color = w.Color,
                            CycleOptions = w.CycleOptions,
                            ItemDimensions = w.ItemDimensions,
                            ItemWeight = w.ItemWeight,
                            SpecialFeatures = w.SpecialFeatures,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, w.ID, "WashingMachines"),
                            CategoryName = w.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var offers = await _unitOfWork.Offers.GetOffers("Appliances", WashingMachine.Category?.Name, WashingMachine.ID);

                    var discountValue = string.Empty;
                    if (offers.Any())
                    {
                        discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                           $"{offers.First().PercentDiscount}%" :
                           offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;
                    }

                    var BOGOGetItem = await _unitOfWork.Offers.GetBOGOGetItem(WashingMachine);

                    var washingMachine = new WashingMachineViewModel
                    {
                        Id = id,
                        Name = WashingMachine.Name,
                        Rate = WashingMachine.Rate,
                        Price = WashingMachine.Price,
                        NewPrice = WashingMachine.NewPrice ?? 0,
                        IsDiscounted = WashingMachine.IsDiscounted,
                        DiscountValue = discountValue,
                        IsBOGOBuy = WashingMachine.IsBOGOBuy,
                        IsBOGOGet = WashingMachine.IsBOGOGet,
                        imageSrc = WashingMachine.imageSrc,
                        Capacity = WashingMachine.Capacity,
                        Color = WashingMachine.Color,
                        CycleOptions = WashingMachine.CycleOptions,
                        ItemDimensions = WashingMachine.ItemDimensions,
                        ItemWeight = WashingMachine.ItemWeight,
                        SpecialFeatures = WashingMachine.SpecialFeatures,
                        CategoryName = WashingMachine.Category.Name,
                        RelatedWashingMachines = relatedWashingMachines,
                        SimilarPriceWashingMachines = similarPriceWashingMachines,
                        Comments = comments,
                        Offers = offers,
                        BOGOGet = BOGOGetItem,
                        StarCounts = starCounts,
                        RateCount = rateCount,
                        ControllerName = "WashingMachines",
                        TotalQuantity = totalQuantity
                    };

                    return View(washingMachine);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllWashingMachineComments(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var WashingMachine = await _unitOfWork.WashingMachines.GetById(id);

                var rateList = await _unitOfWork.WashingMachines.GetItemRates(id, "WashingMachines");
                var rateCount = rateList.Count();

                var starCounts = await _unitOfWork.WashingMachines.GetItemRateDetails(id, "WashingMachines");

                if (WashingMachine != null)
                {
                    var comments = await _unitOfWork.WashingMachines.GetItemComments(id, "WashingMachines", "All");

                    if (comments.Any())
                    {
                        var washingMachine = new WashingMachineViewModel
                        {
                            Id = WashingMachine.ID,
                            Name = WashingMachine.Name,
                            Rate = WashingMachine.Rate,
                            CategoryName = WashingMachine.Category.Name,
                            Comments = comments,
                            StarCounts = starCounts,
                            RateCount = rateCount
                        };

                        return View("AllComments", washingMachine);
                    }
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}
