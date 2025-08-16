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
    public class TVsController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _unitOfWork.Categories.GetSpecificCategories("Electronics");

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public TVsController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var tvsCategories = await _unitOfWork.TVs.GetItemCategories().Result.ToListAsync();

            var discountedTVs = _unitOfWork.TVs.GetDiscountedItems(1, 10, "ID", false).Result.ToList().
                Select(t => new TVViewModel
                {
                    Id = t.ID,
                    Name = t.Name,
                    Rate = t.Rate,
                    Price = t.Price,
                    NewPrice = t.NewPrice ?? 0,
                    imageSrc = t.imageSrc,
                    ConnectivityTechnology = t.ConnectivityTechnology,
                    DisplayTechnology = t.DisplayTechnology,
                    ItemDimensions = t.ItemDimensions,
                    RefreshRate = t.RefreshRate,
                    SpecialFeatures = t.SpecialFeatures,
                    Resolution = t.Resolution,
                    ScreenSize = t.ScreenSize,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, t.ID, "TVs"),
                    CategoryName = t.Category.Name,
                    RateCount = _unitOfWork.TVs.GetItemRates(t.ID, "TVs").Result.Count()
                }).OrderBy(t => Guid.NewGuid()).ToList();

            var topRatedTVs = _unitOfWork.TVs.GetTopRatedItems(1, 10, "ID", false).Result.ToList().
                Select(t => new TVViewModel
                {
                    Id = t.ID,
                    Name = t.Name,
                    Rate = t.Rate,
                    Price = t.Price,
                    NewPrice = t.NewPrice ?? 0,
                    imageSrc = t.imageSrc,
                    ConnectivityTechnology = t.ConnectivityTechnology,
                    DisplayTechnology = t.DisplayTechnology,
                    ItemDimensions = t.ItemDimensions,
                    RefreshRate = t.RefreshRate,
                    SpecialFeatures = t.SpecialFeatures,
                    Resolution = t.Resolution,
                    ScreenSize = t.ScreenSize,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, t.ID, "TVs"),
                    CategoryName = t.Category.Name,
                    RateCount = _unitOfWork.TVs.GetItemRates(t.ID, "TVs").Result.Count()
                }).OrderBy(t => Guid.NewGuid()).ToList();

            var latestTVs = _unitOfWork.TVs.GetLatestItems(1, 10, "ID", false).Result.ToList().
                Select(t => new TVViewModel
                {
                    Id = t.ID,
                    Name = t.Name,
                    Rate = t.Rate,
                    Price = t.Price,
                    NewPrice = t.NewPrice ?? 0,
                    imageSrc = t.imageSrc,
                    ConnectivityTechnology = t.ConnectivityTechnology,
                    DisplayTechnology = t.DisplayTechnology,
                    ItemDimensions = t.ItemDimensions,
                    RefreshRate = t.RefreshRate,
                    SpecialFeatures = t.SpecialFeatures,
                    Resolution = t.Resolution,
                    ScreenSize = t.ScreenSize,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, t.ID, "TVs"),
                    CategoryName = t.Category.Name,
                    RateCount = _unitOfWork.TVs.GetItemRates(t.ID, "TVs").Result.Count()
                }).OrderBy(t => Guid.NewGuid()).ToList();

            var tvsVM = new ItemViewModel<TVViewModel>()
            {
                ItemCategories = tvsCategories,
                DiscountedItems = discountedTVs,
                latestItems = latestTVs,
                TopRatedItems = topRatedTVs
            };

            return View(tvsVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var tvs = await _unitOfWork.TVs.GetAll(pageNumber, pageSize);

            return View(tvs);
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
        public async Task<IActionResult> Add(TV data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.TVs.Add(data);
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

            var TV = await _unitOfWork.TVs.GetById(id);

            if (TV != null)
            {
                await CreateCategoriesSelectList();
                return View(TV);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(TV data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.TVs.Update(data);
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

            var TV = await _unitOfWork.TVs.GetById(id);

            if (TV != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(TV data)
        {
            await _unitOfWork.TVs.Delete(data);
            await _unitOfWork.Commit();
            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> TVs()
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
                var totalPages = (int)Math.Ceiling(await _unitOfWork.TVs.TotalItems("Brands", null, null, name) / (double)pageSize);

                var tvs = _unitOfWork.TVs.GetCategoryItems(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                     Select(t => new TVViewModel
                     {
                         Id = t.ID,
                         Name = t.Name,
                         Rate = t.Rate,
                         Price = t.Price,
                         NewPrice = t.NewPrice ?? 0,
                         imageSrc = t.imageSrc,
                         ConnectivityTechnology = t.ConnectivityTechnology,
                         DisplayTechnology = t.DisplayTechnology,
                         ItemDimensions = t.ItemDimensions,
                         RefreshRate = t.RefreshRate,
                         SpecialFeatures = t.SpecialFeatures,
                         Resolution = t.Resolution,
                         ScreenSize = t.ScreenSize,
                         isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, t.ID, "TVs"),
                         CategoryName = t.Category.Name,
                         RateCount = _unitOfWork.TVs.GetItemRates(t.ID, "TVs").Result.Count()
                     }).ToList();

                var data = new ItemsViewModel
                {
                    Items = tvs,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    OrderIndex = orderIndex,
                    Des = des,
                    ActionName = "Brands",
                    Brand = name
                };
                return View("TVs", data);
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
            var totalPages = (int)Math.Ceiling(await _unitOfWork.TVs.TotalItems("Discounted") / (double)pageSize);

            var discountedTVs = _unitOfWork.TVs.GetDiscountedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(t => new TVViewModel
                 {
                     Id = t.ID,
                     Name = t.Name,
                     Rate = t.Rate,
                     Price = t.Price,
                     NewPrice = t.NewPrice ?? 0,
                     imageSrc = t.imageSrc,
                     ConnectivityTechnology = t.ConnectivityTechnology,
                     DisplayTechnology = t.DisplayTechnology,
                     ItemDimensions = t.ItemDimensions,
                     RefreshRate = t.RefreshRate,
                     SpecialFeatures = t.SpecialFeatures,
                     Resolution = t.Resolution,
                     ScreenSize = t.ScreenSize,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, t.ID, "TVs"),
                     CategoryName = t.Category.Name,
                     RateCount = _unitOfWork.TVs.GetItemRates(t.ID, "TVs").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = discountedTVs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
            return View("TVs", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.TVs.TotalItems("Rated") / (double)pageSize);

            var ratedTVs = _unitOfWork.TVs.GetTopRatedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(t => new TVViewModel
                 {
                     Id = t.ID,
                     Name = t.Name,
                     Rate = t.Rate,
                     Price = t.Price,
                     NewPrice = t.NewPrice ?? 0,
                     imageSrc = t.imageSrc,
                     ConnectivityTechnology = t.ConnectivityTechnology,
                     DisplayTechnology = t.DisplayTechnology,
                     ItemDimensions = t.ItemDimensions,
                     RefreshRate = t.RefreshRate,
                     SpecialFeatures = t.SpecialFeatures,
                     Resolution = t.Resolution,
                     ScreenSize = t.ScreenSize,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, t.ID, "TVs"),
                     CategoryName = t.Category.Name,
                     RateCount = _unitOfWork.TVs.GetItemRates(t.ID, "TVs").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = ratedTVs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
            return View("TVs", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.TVs.TotalItems("Latest") / (double)pageSize);

            var latestTVs = _unitOfWork.TVs.GetLatestItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(t => new TVViewModel
                 {
                     Id = t.ID,
                     Name = t.Name,
                     Rate = t.Rate,
                     Price = t.Price,
                     NewPrice = t.NewPrice ?? 0,
                     imageSrc = t.imageSrc,
                     ConnectivityTechnology = t.ConnectivityTechnology,
                     DisplayTechnology = t.DisplayTechnology,
                     ItemDimensions = t.ItemDimensions,
                     RefreshRate = t.RefreshRate,
                     SpecialFeatures = t.SpecialFeatures,
                     Resolution = t.Resolution,
                     ScreenSize = t.ScreenSize,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, t.ID, "TVs"),
                     CategoryName = t.Category.Name,
                     RateCount = _unitOfWork.TVs.GetItemRates(t.ID, "TVs").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = latestTVs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
            return View("TVs", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.TVs.TotalItems("Price", price1, price2, null) / (double)pageSize);

            var priceTVs = _unitOfWork.TVs.GetItemsFilteredByPrice(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(t => new TVViewModel
                 {
                     Id = t.ID,
                     Name = t.Name,
                     Rate = t.Rate,
                     Price = t.Price,
                     NewPrice = t.NewPrice ?? 0,
                     imageSrc = t.imageSrc,
                     ConnectivityTechnology = t.ConnectivityTechnology,
                     DisplayTechnology = t.DisplayTechnology,
                     ItemDimensions = t.ItemDimensions,
                     RefreshRate = t.RefreshRate,
                     SpecialFeatures = t.SpecialFeatures,
                     Resolution = t.Resolution,
                     ScreenSize = t.ScreenSize,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, t.ID, "TVs"),
                     CategoryName = t.Category.Name,
                     RateCount = _unitOfWork.TVs.GetItemRates(t.ID, "TVs").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = priceTVs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
            return View("TVs", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var TV = await _unitOfWork.TVs.GetById(id);

                if (TV != null)
                {
                    var comments = await _unitOfWork.TVs.GetItemComments(id, "TVs", "Default");

                    var rateList = await _unitOfWork.TVs.GetItemRates(id, "TVs");
                    var rateCount = rateList.Count();

                    var starCounts = await _unitOfWork.TVs.GetItemRateDetails(id, "TVs");

                    var totalQuantity = await _unitOfWork.Carts.TotalItemQuantityInCart(id, "TVs");

                    var similarPriceTVs = _unitOfWork.TVs.GetAllWithoutPagination().Result.
                        Where(t => t.Price == TV.Price || Math.Abs(t.Price - TV.Price) <= 1000).ToList()
                        .Select(t => new TVViewModel
                        {
                            Id = t.ID,
                            Name = t.Name,
                            Rate = t.Rate,
                            Price = t.Price,
                            NewPrice = t.NewPrice ?? 0,
                            IsDiscounted = t.IsDiscounted,
                            imageSrc = t.imageSrc,
                            ConnectivityTechnology = t.ConnectivityTechnology,
                            DisplayTechnology = t.DisplayTechnology,
                            ItemDimensions = t.ItemDimensions,
                            RefreshRate = t.RefreshRate,
                            SpecialFeatures = t.SpecialFeatures,
                            Resolution = t.Resolution,
                            ScreenSize = t.ScreenSize,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, t.ID, "TVs"),
                            CategoryName = t.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var relatedTVs = _unitOfWork.TVs.GetAllWithoutPagination().Result
                        .Where(t => t.CategoryId == TV.CategoryId).Take(10).ToList().
                        Select(t => new TVViewModel
                        {
                            Id = t.ID,
                            Name = t.Name,
                            Rate = t.Rate,
                            Price = t.Price,
                            NewPrice = t.NewPrice ?? 0,
                            IsDiscounted = t.IsDiscounted,
                            imageSrc = t.imageSrc,
                            ConnectivityTechnology = t.ConnectivityTechnology,
                            DisplayTechnology = t.DisplayTechnology,
                            ItemDimensions = t.ItemDimensions,
                            RefreshRate = t.RefreshRate,
                            SpecialFeatures = t.SpecialFeatures,
                            Resolution = t.Resolution,
                            ScreenSize = t.ScreenSize,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, t.ID, "TVs"),
                            CategoryName = t.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var offers = await _unitOfWork.Offers.GetOffers("Electronics", TV.Category?.Name, TV.ID);

                    var discountValue = string.Empty;
                    if (offers.Any())
                    {
                        discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                           $"{offers.First().PercentDiscount}%" :
                           offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;
                    }

                    var BOGOGetItem = await _unitOfWork.Offers.GetBOGOGetItem(TV);

                    var tv = new TVViewModel
                    {
                        Id = id,
                        Name = TV.Name,
                        Rate = TV.Rate,
                        Price = TV.Price,
                        NewPrice = TV.NewPrice ?? 0,
                        IsDiscounted = TV.IsDiscounted,
                        DiscountValue = discountValue,
                        IsBOGOBuy = TV.IsBOGOBuy,
                        IsBOGOGet = TV.IsBOGOGet,
                        imageSrc = TV.imageSrc,
                        ConnectivityTechnology = TV.ConnectivityTechnology,
                        DisplayTechnology = TV.DisplayTechnology,
                        ItemDimensions = TV.ItemDimensions,
                        RefreshRate = TV.RefreshRate,
                        SpecialFeatures = TV.SpecialFeatures,
                        Resolution = TV.Resolution,
                        ScreenSize = TV.ScreenSize,
                        CategoryName = TV.Category.Name,
                        RelatedTVs = relatedTVs,
                        SimilarPriceTVs = similarPriceTVs,
                        Comments = comments,
                        Offers = offers,
                        BOGOGet = BOGOGetItem,
                        StarCounts = starCounts,
                        RateCount = rateCount,
                        ControllerName = "TVs",
                        TotalQuantity = totalQuantity
                    };

                    return View(tv);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllTVComments(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var TV = await _unitOfWork.TVs.GetById(id);

                var rateList = await _unitOfWork.TVs.GetItemRates(id, "TVs");
                var rateCount = rateList.Count();

                var starCounts = await _unitOfWork.TVs.GetItemRateDetails(id, "TVs");

                if (TV != null)
                {
                    var comments = await _unitOfWork.TVs.GetItemComments(id, "TVs", "All");

                    if (comments.Any())
                    {
                        var phone = new TVViewModel
                        {
                            Id = TV.ID,
                            Name = TV.Name,
                            Rate = TV.Rate,
                            CategoryName = TV.Category.Name,
                            Comments = comments,
                            StarCounts = starCounts,
                            RateCount = rateCount
                        };

                        return View("AllComments", phone);
                    }
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}
