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
    public class HeadPhonesController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _unitOfWork.Categories.GetSpecificCategories("Electronics");

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public HeadPhonesController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var headphonesCategories = await _unitOfWork.HeadPhones.GetItemCategories().Result.ToListAsync();

            var discountedHeadPhones = _unitOfWork.HeadPhones.GetDiscountedItems(1, 10, "ID", false).Result.ToList().
                Select(h => new HeadPhoneViewModel
                {
                    Id = h.ID,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice ?? 0,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, h.ID, "HeadPhones"),
                    CategoryName = h.Category.Name,
                    RateCount = _unitOfWork.HeadPhones.GetItemRates(h.ID, "HeadPhones").Result.Count()
                }).OrderBy(h => Guid.NewGuid()).ToList();

            var topRatedHeadPhones = _unitOfWork.HeadPhones.GetTopRatedItems(1, 10, "ID", false).Result.ToList().
                Select(h => new HeadPhoneViewModel
                {
                    Id = h.ID,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice ?? 0,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, h.ID, "HeadPhones"),
                    CategoryName = h.Category.Name,
                    RateCount = _unitOfWork.HeadPhones.GetItemRates(h.ID, "HeadPhones").Result.Count()
                }).OrderBy(h => Guid.NewGuid()).ToList();

            var latestHeadPhones = _unitOfWork.HeadPhones.GetLatestItems(1, 10, "ID", false).Result.ToList().
                Select(h => new HeadPhoneViewModel
                {
                    Id = h.ID,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice ?? 0,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, h.ID, "HeadPhones"),
                    CategoryName = h.Category.Name,
                    RateCount = _unitOfWork.HeadPhones.GetItemRates(h.ID, "HeadPhones").Result.Count()
                }).OrderBy(h => Guid.NewGuid()).ToList();

            var HeadPhonesVM = new ItemViewModel<HeadPhoneViewModel>()
            {
                ItemCategories = headphonesCategories,
                DiscountedItems = discountedHeadPhones,
                latestItems = latestHeadPhones,
                TopRatedItems = topRatedHeadPhones
            };

            return View(HeadPhonesVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var headPhones = await _unitOfWork.HeadPhones.GetAll(pageNumber, pageSize);

            return View(headPhones);
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
        public async Task<IActionResult> Add(HeadPhone data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.HeadPhones.Add(data);
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

            var HeadPhone = await _unitOfWork.HeadPhones.GetById(id);

            if (HeadPhone != null)
            {
                await CreateCategoriesSelectList();
                return View(HeadPhone);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(HeadPhone data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.HeadPhones.Update(data);
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

            var HeadPhone = await _unitOfWork.HeadPhones.GetById(id);

            if (HeadPhone != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(HeadPhone data)
        {
            await _unitOfWork.HeadPhones.Delete(data);
            await _unitOfWork.Commit();
            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> HeadPhones()
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
                var totalPages = (int)Math.Ceiling(await _unitOfWork.HeadPhones.TotalItems("Brands", null, null, name) / (double)pageSize);

                var headPhones = _unitOfWork.HeadPhones.GetCategoryItems(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                    Select(h => new HeadPhoneViewModel
                    {
                        Id = h.ID,
                        Name = h.Name,
                        Rate = h.Rate,
                        Price = h.Price,
                        NewPrice = h.NewPrice ?? 0,
                        imageSrc = h.imageSrc,
                        ConnectivityTechnology = h.ConnectivityTechnology,
                        Color = h.Color,
                        NoiseControl = h.NoiseControl,
                        HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                        HeadphonesFormFactor = h.HeadphonesFormFactor,
                        ModelName = h.ModelName,
                        isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, h.ID, "HeadPhones"),
                        CategoryName = h.Category.Name,
                        RateCount = _unitOfWork.HeadPhones.GetItemRates(h.ID, "HeadPhones").Result.Count()
                    }).ToList();

                var data = new ItemsViewModel
                {
                    Items = headPhones,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    OrderIndex = orderIndex,
                    Des = des,
                    ActionName = "Brands",
                    Brand = name
                };
                return View("HeadPhones", data);
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
            var totalPages = (int)Math.Ceiling(await _unitOfWork.HeadPhones.TotalItems("Discounted") / (double)pageSize);

            var discountedHeadPhones = _unitOfWork.HeadPhones.GetDiscountedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(h => new HeadPhoneViewModel
                {
                    Id = h.ID,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice ?? 0,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, h.ID, "HeadPhones"),
                    CategoryName = h.Category.Name,
                    RateCount = _unitOfWork.HeadPhones.GetItemRates(h.ID, "HeadPhones").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = discountedHeadPhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
            return View("HeadPhones", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.HeadPhones.TotalItems("Rated") / (double)pageSize);

            var ratedHeadPhones = _unitOfWork.HeadPhones.GetTopRatedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(h => new HeadPhoneViewModel
                {
                    Id = h.ID,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice ?? 0,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, h.ID, "HeadPhones"),
                    CategoryName = h.Category.Name,
                    RateCount = _unitOfWork.HeadPhones.GetItemRates(h.ID, "HeadPhones").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = ratedHeadPhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
            return View("HeadPhones", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.HeadPhones.TotalItems("Latest") / (double)pageSize);

            var latestHeadPhones = _unitOfWork.HeadPhones.GetLatestItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(h => new HeadPhoneViewModel
                {
                    Id = h.ID,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice ?? 0,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, h.ID, "HeadPhones"),
                    CategoryName = h.Category.Name,
                    RateCount = _unitOfWork.HeadPhones.GetItemRates(h.ID, "HeadPhones").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = latestHeadPhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
            return View("HeadPhones", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.HeadPhones.TotalItems("Price", price1, price2, null) / (double)pageSize);

            var priceHeadPhones = _unitOfWork.HeadPhones.GetItemsFilteredByPrice(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(h => new HeadPhoneViewModel
                {
                    Id = h.ID,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice ?? 0,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, h.ID, "HeadPhones"),
                    CategoryName = h.Category.Name,
                    RateCount = _unitOfWork.HeadPhones.GetItemRates(h.ID, "HeadPhones").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = priceHeadPhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
            return View("HeadPhones", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var HeadPhone = await _unitOfWork.HeadPhones.GetById(id);

                if (HeadPhone != null)
                {
                    var comments = await _unitOfWork.HeadPhones.GetItemComments(id, "HeadPhones", "Default");

                    var rateList = await _unitOfWork.HeadPhones.GetItemRates(id, "HeadPhones");
                    var rateCount = rateList.Count();

                    var starCounts = await _unitOfWork.HeadPhones.GetItemRateDetails(id, "HeadPhones");

                    var totalQuantity = await _unitOfWork.Carts.TotalItemQuantityInCart(id, "HeadPhones");

                    var similarPriceHeadPhones = _unitOfWork.HeadPhones.GetAllWithoutPagination().Result.
                        Where(h => h.Price == HeadPhone.Price || Math.Abs(h.Price - HeadPhone.Price) <= 1000).ToList()
                        .Select(h => new HeadPhoneViewModel
                        {
                            Id = h.ID,
                            Name = h.Name,
                            Rate = h.Rate,
                            Price = h.Price,
                            NewPrice = h.NewPrice ?? 0,
                            imageSrc = h.imageSrc,
                            ConnectivityTechnology = h.ConnectivityTechnology,
                            Color = h.Color,
                            NoiseControl = h.NoiseControl,
                            HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                            HeadphonesFormFactor = h.HeadphonesFormFactor,
                            ModelName = h.ModelName,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, h.ID, "HeadPhones"),
                            CategoryName = h.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var relatedHeadPhones = _unitOfWork.HeadPhones.GetAllWithoutPagination().Result
                        .Where(m => m.CategoryId == HeadPhone.CategoryId).Take(10).ToList().
                        Select(h => new HeadPhoneViewModel
                        {
                            Id = h.ID,
                            Name = h.Name,
                            Rate = h.Rate,
                            Price = h.Price,
                            NewPrice = h.NewPrice ?? 0,
                            imageSrc = h.imageSrc,
                            ConnectivityTechnology = h.ConnectivityTechnology,
                            Color = h.Color,
                            NoiseControl = h.NoiseControl,
                            HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                            HeadphonesFormFactor = h.HeadphonesFormFactor,
                            ModelName = h.ModelName,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, h.ID, "HeadPhones"),
                            CategoryName = h.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var offers = await _unitOfWork.Offers.GetOffers("Electronics", HeadPhone.Category?.Name, HeadPhone.ID);

                    var discountValue = string.Empty;
                    if (offers.Any())
                    {
                        discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                           $"{offers.First().PercentDiscount}%" :
                           offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;
                    }

                    var BOGOGetItem = await _unitOfWork.Offers.GetBOGOGetItem(HeadPhone);

                    var headphone = new HeadPhoneViewModel
                    {
                        Id = HeadPhone.ID,
                        Name = HeadPhone.Name,
                        Rate = HeadPhone.Rate,
                        Price = HeadPhone.Price,
                        NewPrice = HeadPhone.NewPrice ?? 0,
                        IsDiscounted = HeadPhone.IsDiscounted,
                        DiscountValue = discountValue,
                        IsBOGOBuy = HeadPhone.IsBOGOBuy,
                        IsBOGOGet = HeadPhone.IsBOGOGet,
                        imageSrc = HeadPhone.imageSrc,
                        ConnectivityTechnology = HeadPhone.ConnectivityTechnology,
                        Color = HeadPhone.Color,
                        NoiseControl = HeadPhone.NoiseControl,
                        HeadphonesEarPlacement = HeadPhone.HeadphonesEarPlacement,
                        HeadphonesFormFactor = HeadPhone.HeadphonesFormFactor,
                        ModelName = HeadPhone.ModelName,
                        CategoryName = HeadPhone.Category.Name,
                        RelatedHeadPhones = relatedHeadPhones,
                        SimilarPriceHeadPhones = similarPriceHeadPhones,
                        Comments = comments,
                        Offers = offers,
                        BOGOGet = BOGOGetItem,
                        StarCounts = starCounts,
                        RateCount = rateCount,
                        ControllerName = "HeadPhones",
                        TotalQuantity = totalQuantity
                    };

                    return View(headphone);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllHeadPhoneComments(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var HeadPhone = await _unitOfWork.HeadPhones.GetById(id);

                var rateList = await _unitOfWork.HeadPhones.GetItemRates(id, "HeadPhones");
                var rateCount = rateList.Count();

                var starCounts = await _unitOfWork.HeadPhones.GetItemRateDetails(id, "HeadPhones");

                if (HeadPhone != null)
                {
                    var comments = await _unitOfWork.HeadPhones.GetItemComments(id, "HeadPhones", "All");

                    if (comments.Any())
                    {
                        var headPhone = new HeadPhoneViewModel
                        {
                            Id = HeadPhone.ID,
                            Name = HeadPhone.Name,
                            Rate = HeadPhone.Rate,
                            CategoryName = HeadPhone.Category.Name,
                            Comments = comments,
                            StarCounts = starCounts,
                            RateCount = rateCount
                        };

                        return View("AllComments", headPhone);
                    }
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}
