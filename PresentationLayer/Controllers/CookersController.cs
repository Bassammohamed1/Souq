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
    public class CookersController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _unitOfWork.Categories.GetSpecificCategories("Appliances");

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public CookersController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var cookersCategories = await _unitOfWork.Cookers.GetItemCategories().Result.ToListAsync();

            var discountedCookers = _unitOfWork.Cookers.GetDiscountedItems(1, 10, "ID", false).Result.ToList().
                Select(c => new CookerViewModel
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    ModelName = c.ModelName,
                    Material = c.Material,
                    ItemWeight = c.ItemWeight,
                    Color = c.Color,
                    ItemDimensions = c.ItemDimensions,
                    DrawerType = c.DrawerType,
                    ControlsType = c.ControlsType,
                    FinishType = c.FinishType,
                    FormFactor = c.FormFactor,
                    NumberOfHeatingElements = c.NumberOfHeatingElements,
                    SpecialFeatures = c.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, c.ID, "Cookers"),
                    CategoryName = c.Category.Name,
                    RateCount = _unitOfWork.Cookers.GetItemRates(c.ID, "Cookers").Result.Count()
                }).OrderBy(c => Guid.NewGuid()).ToList();

            var topRatedCookers = _unitOfWork.Cookers.GetTopRatedItems(1, 10, "ID", false).Result.ToList().
                Select(c => new CookerViewModel
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    ModelName = c.ModelName,
                    Material = c.Material,
                    ItemWeight = c.ItemWeight,
                    Color = c.Color,
                    ItemDimensions = c.ItemDimensions,
                    DrawerType = c.DrawerType,
                    ControlsType = c.ControlsType,
                    FinishType = c.FinishType,
                    FormFactor = c.FormFactor,
                    NumberOfHeatingElements = c.NumberOfHeatingElements,
                    SpecialFeatures = c.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, c.ID, "Cookers"),
                    CategoryName = c.Category.Name,
                    RateCount = _unitOfWork.Cookers.GetItemRates(c.ID, "Cookers").Result.Count()
                }).OrderBy(c => Guid.NewGuid()).ToList();

            var latestCookers = _unitOfWork.Cookers.GetLatestItems(1, 10, "ID", false).Result.ToList().
                Select(c => new CookerViewModel
                {
                    Id = c.ID,
                    Name = c.Name,
                    Rate = c.Rate,
                    Price = c.Price,
                    NewPrice = c.NewPrice ?? 0,
                    imageSrc = c.imageSrc,
                    ModelName = c.ModelName,
                    Material = c.Material,
                    ItemWeight = c.ItemWeight,
                    Color = c.Color,
                    ItemDimensions = c.ItemDimensions,
                    DrawerType = c.DrawerType,
                    ControlsType = c.ControlsType,
                    FinishType = c.FinishType,
                    FormFactor = c.FormFactor,
                    NumberOfHeatingElements = c.NumberOfHeatingElements,
                    SpecialFeatures = c.SpecialFeatures,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, c.ID, "Cookers"),
                    CategoryName = c.Category.Name,
                    RateCount = _unitOfWork.Cookers.GetItemRates(c.ID, "Cookers").Result.Count()
                }).OrderBy(c => Guid.NewGuid()).ToList();

            var cookersVM = new ItemViewModel<CookerViewModel>()
            {
                ItemCategories = cookersCategories,
                DiscountedItems = discountedCookers,
                latestItems = latestCookers,
                TopRatedItems = topRatedCookers
            };

            return View(cookersVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var cookers = await _unitOfWork.Cookers.GetAll(pageNumber, pageSize);

            return View(cookers);
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
        public async Task<IActionResult> Add(Cooker data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.Cookers.Add(data);
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

            var Cooker = await _unitOfWork.Cookers.GetById(id);

            if (Cooker != null)
            {
                await CreateCategoriesSelectList();
                return View(Cooker);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(Cooker data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.Cookers.Update(data);
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

            var Cooker = await _unitOfWork.Cookers.GetById(id);

            if (Cooker != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(Cooker data)
        {
            await _unitOfWork.Cookers.Delete(data);
            await _unitOfWork.Commit();
            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> Cookers()
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
                var totalPages = (int)Math.Ceiling(await _unitOfWork.Cookers.TotalItems("Brands", null, null, name) / (double)pageSize);

                var cookers = _unitOfWork.Cookers.GetCategoryItems(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                     Select(c => new CookerViewModel
                     {
                         Id = c.ID,
                         Name = c.Name,
                         Rate = c.Rate,
                         Price = c.Price,
                         NewPrice = c.NewPrice ?? 0,
                         imageSrc = c.imageSrc,
                         ModelName = c.ModelName,
                         Material = c.Material,
                         ItemWeight = c.ItemWeight,
                         Color = c.Color,
                         ItemDimensions = c.ItemDimensions,
                         DrawerType = c.DrawerType,
                         ControlsType = c.ControlsType,
                         FinishType = c.FinishType,
                         FormFactor = c.FormFactor,
                         NumberOfHeatingElements = c.NumberOfHeatingElements,
                         SpecialFeatures = c.SpecialFeatures,
                         isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, c.ID, "Cookers"),
                         CategoryName = c.Category.Name,
                         RateCount = _unitOfWork.Cookers.GetItemRates(c.ID, "Cookers").Result.Count()
                     }).ToList();

                var data = new ItemsViewModel
                {
                    Items = cookers,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    OrderIndex = orderIndex,
                    Des = des,
                    ActionName = "Brands",
                    Brand = name
                };
                return View("Cookers", data);
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
            var totalPages = (int)Math.Ceiling(await _unitOfWork.Cookers.TotalItems("Discounted") / (double)pageSize);

            var discountedCookers = _unitOfWork.Cookers.GetDiscountedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(c => new CookerViewModel
                 {
                     Id = c.ID,
                     Name = c.Name,
                     Rate = c.Rate,
                     Price = c.Price,
                     NewPrice = c.NewPrice ?? 0,
                     imageSrc = c.imageSrc,
                     ModelName = c.ModelName,
                     Material = c.Material,
                     ItemWeight = c.ItemWeight,
                     Color = c.Color,
                     ItemDimensions = c.ItemDimensions,
                     DrawerType = c.DrawerType,
                     ControlsType = c.ControlsType,
                     FinishType = c.FinishType,
                     FormFactor = c.FormFactor,
                     NumberOfHeatingElements = c.NumberOfHeatingElements,
                     SpecialFeatures = c.SpecialFeatures,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, c.ID, "Cookers"),
                     CategoryName = c.Category.Name,
                     RateCount = _unitOfWork.Cookers.GetItemRates(c.ID, "Cookers").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = discountedCookers,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
            return View("Cookers", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.Cookers.TotalItems("Rated") / (double)pageSize);

            var ratedCookers = _unitOfWork.Cookers.GetTopRatedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(c => new CookerViewModel
                 {
                     Id = c.ID,
                     Name = c.Name,
                     Rate = c.Rate,
                     Price = c.Price,
                     NewPrice = c.NewPrice ?? 0,
                     imageSrc = c.imageSrc,
                     ModelName = c.ModelName,
                     Material = c.Material,
                     ItemWeight = c.ItemWeight,
                     Color = c.Color,
                     ItemDimensions = c.ItemDimensions,
                     DrawerType = c.DrawerType,
                     ControlsType = c.ControlsType,
                     FinishType = c.FinishType,
                     FormFactor = c.FormFactor,
                     NumberOfHeatingElements = c.NumberOfHeatingElements,
                     SpecialFeatures = c.SpecialFeatures,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, c.ID, "Cookers"),
                     CategoryName = c.Category.Name,
                     RateCount = _unitOfWork.Cookers.GetItemRates(c.ID, "Cookers").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = ratedCookers,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
            return View("Cookers", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.Cookers.TotalItems("Latest") / (double)pageSize);

            var latestCookers = _unitOfWork.Cookers.GetLatestItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(c => new CookerViewModel
                 {
                     Id = c.ID,
                     Name = c.Name,
                     Rate = c.Rate,
                     Price = c.Price,
                     NewPrice = c.NewPrice ?? 0,
                     imageSrc = c.imageSrc,
                     ModelName = c.ModelName,
                     Material = c.Material,
                     ItemWeight = c.ItemWeight,
                     Color = c.Color,
                     ItemDimensions = c.ItemDimensions,
                     DrawerType = c.DrawerType,
                     ControlsType = c.ControlsType,
                     FinishType = c.FinishType,
                     FormFactor = c.FormFactor,
                     NumberOfHeatingElements = c.NumberOfHeatingElements,
                     SpecialFeatures = c.SpecialFeatures,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, c.ID, "Cookers"),
                     CategoryName = c.Category.Name,
                     RateCount = _unitOfWork.Cookers.GetItemRates(c.ID, "Cookers").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = latestCookers,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
            return View("Cookers", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.Cookers.TotalItems("Price", price1, price2, null) / (double)pageSize);

            var priceCookers = _unitOfWork.Cookers.GetItemsFilteredByPrice(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(c => new CookerViewModel
                 {
                     Id = c.ID,
                     Name = c.Name,
                     Rate = c.Rate,
                     Price = c.Price,
                     NewPrice = c.NewPrice ?? 0,
                     imageSrc = c.imageSrc,
                     ModelName = c.ModelName,
                     Material = c.Material,
                     ItemWeight = c.ItemWeight,
                     Color = c.Color,
                     ItemDimensions = c.ItemDimensions,
                     DrawerType = c.DrawerType,
                     ControlsType = c.ControlsType,
                     FinishType = c.FinishType,
                     FormFactor = c.FormFactor,
                     NumberOfHeatingElements = c.NumberOfHeatingElements,
                     SpecialFeatures = c.SpecialFeatures,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, c.ID, "Cookers"),
                     CategoryName = c.Category.Name,
                     RateCount = _unitOfWork.Cookers.GetItemRates(c.ID, "Cookers").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = priceCookers,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
            return View("Cookers", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var Cooker = await _unitOfWork.Cookers.GetById(id);

                if (Cooker != null)
                {
                    var comments = await _unitOfWork.Cookers.GetItemComments(id, "Cookers", "Default");

                    var rateList = await _unitOfWork.Cookers.GetItemRates(id, "Cookers");
                    var rateCount = rateList.Count();

                    var starCounts = await _unitOfWork.Cookers.GetItemRateDetails(id, "Cookers");

                    var totalQuantity = await _unitOfWork.Carts.TotalItemQuantityInCart(id, "Cookers");

                    var similarPriceCookers = _unitOfWork.Cookers.GetAllWithoutPagination().Result.
                        Where(c => c.Price == Cooker.Price || Math.Abs(c.Price - Cooker.Price) <= 1000).ToList().
                        Select(c => new CookerViewModel
                        {
                            Id = c.ID,
                            Name = c.Name,
                            Rate = c.Rate,
                            Price = c.Price,
                            NewPrice = c.NewPrice ?? 0,
                            imageSrc = c.imageSrc,
                            ModelName = c.ModelName,
                            Material = c.Material,
                            ItemWeight = c.ItemWeight,
                            Color = c.Color,
                            ItemDimensions = c.ItemDimensions,
                            DrawerType = c.DrawerType,
                            ControlsType = c.ControlsType,
                            FinishType = c.FinishType,
                            FormFactor = c.FormFactor,
                            NumberOfHeatingElements = c.NumberOfHeatingElements,
                            SpecialFeatures = c.SpecialFeatures,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, c.ID, "Cookers"),
                            CategoryName = c.Category.Name,
                            RateCount = _unitOfWork.Cookers.GetItemRates(c.ID, "Cookers").Result.Count()
                        }).ToList();

                    var relatedCookers = _unitOfWork.Cookers.GetAllWithoutPagination().Result
                        .Where(c => c.CategoryId == Cooker.CategoryId).Take(10).ToList().
                        Select(c => new CookerViewModel
                        {
                            Id = c.ID,
                            Name = c.Name,
                            Rate = c.Rate,
                            Price = c.Price,
                            NewPrice = c.NewPrice ?? 0,
                            imageSrc = c.imageSrc,
                            ModelName = c.ModelName,
                            Material = c.Material,
                            ItemWeight = c.ItemWeight,
                            Color = c.Color,
                            ItemDimensions = c.ItemDimensions,
                            DrawerType = c.DrawerType,
                            ControlsType = c.ControlsType,
                            FinishType = c.FinishType,
                            FormFactor = c.FormFactor,
                            NumberOfHeatingElements = c.NumberOfHeatingElements,
                            SpecialFeatures = c.SpecialFeatures,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, c.ID, "Cookers"),
                            CategoryName = c.Category.Name,
                            RateCount = _unitOfWork.Cookers.GetItemRates(c.ID, "Cookers").Result.Count()
                        }).ToList();

                    var offers = await _unitOfWork.Offers.GetOffers("Appliances", Cooker.Category?.Name, Cooker.ID);

                    var discountValue = string.Empty;
                    if (offers.Any())
                    {
                        discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                           $"{offers.First().PercentDiscount}%" :
                           offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;
                    }

                    var BOGOGetItem = await _unitOfWork.Offers.GetBOGOGetItem(Cooker);

                    var cooker = new CookerViewModel
                    {
                        Id = Cooker.ID,
                        Name = Cooker.Name,
                        Rate = Cooker.Rate,
                        Price = Cooker.Price,
                        NewPrice = Cooker.NewPrice ?? 0,
                        IsDiscounted = Cooker.IsDiscounted,
                        DiscountValue = discountValue,
                        IsBOGOBuy = Cooker.IsBOGOBuy,
                        IsBOGOGet = Cooker.IsBOGOGet,
                        imageSrc = Cooker.imageSrc,
                        ModelName = Cooker.ModelName,
                        Material = Cooker.Material,
                        ItemWeight = Cooker.ItemWeight,
                        Color = Cooker.Color,
                        ItemDimensions = Cooker.ItemDimensions,
                        DrawerType = Cooker.DrawerType,
                        ControlsType = Cooker.ControlsType,
                        FinishType = Cooker.FinishType,
                        FormFactor = Cooker.FormFactor,
                        NumberOfHeatingElements = Cooker.NumberOfHeatingElements,
                        SpecialFeatures = Cooker.SpecialFeatures,
                        CategoryName = Cooker.Category.Name,
                        RelatedCookers = relatedCookers,
                        SimilarPriceCookers = similarPriceCookers,
                        Comments = comments,
                        Offers = offers,
                        BOGOGet = BOGOGetItem,
                        StarCounts = starCounts,
                        RateCount = rateCount,
                        ControllerName = "Cookers",
                        TotalQuantity = totalQuantity
                    };

                    return View(cooker);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllCookerComments(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var Cooker = await _unitOfWork.Cookers.GetById(id);

                var rateList = await _unitOfWork.Cookers.GetItemRates(id, "Cookers");
                var rateCount = rateList.Count();

                var starCounts = await _unitOfWork.Cookers.GetItemRateDetails(id, "Cookers");

                if (Cooker != null)
                {
                    var comments = await _unitOfWork.Cookers.GetItemComments(id, "Cookers", "All");

                    if (comments.Any())
                    {
                        var cooker = new CookerViewModel
                        {
                            Id = Cooker.ID,
                            Name = Cooker.Name,
                            Rate = Cooker.Rate,
                            CategoryName = Cooker.Category.Name,
                            Comments = comments,
                            StarCounts = starCounts,
                            RateCount = rateCount
                        };

                        return View("AllComments", cooker);
                    }
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}