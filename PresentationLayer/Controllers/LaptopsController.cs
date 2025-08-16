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
    public class LaptopsController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _unitOfWork.Categories.GetSpecificCategories("Electronics");

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public LaptopsController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var laptopsCategories = await _unitOfWork.Laptops.GetItemCategories().Result.ToListAsync();

            var discountedLaptops = _unitOfWork.Laptops.GetDiscountedItems(1, 10, "ID", false).Result.ToList().
                Select(l => new LaptopViewModel
                {
                    Id = l.ID,
                    Name = l.Name,
                    Rate = l.Rate,
                    Price = l.Price,
                    NewPrice = l.NewPrice ?? 0,
                    imageSrc = l.imageSrc,
                    Color = l.Color,
                    CPU = l.CPU,
                    GPU = l.GPU,
                    HardDiskDescription = l.HardDiskDescription,
                    HardDiskSize = l.HardDiskSize,
                    ModelName = l.ModelName,
                    RAM = l.RAM,
                    OperatingSystem = l.OperatingSystem,
                    ScreenSize = l.ScreenSize,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, l.ID, "Laptops"),
                    CategoryName = l.Category.Name,
                    RateCount = _unitOfWork.Laptops.GetItemRates(l.ID, "Laptops").Result.Count()
                }).OrderBy(l => Guid.NewGuid()).ToList();

            var topRatedLaptops = _unitOfWork.Laptops.GetTopRatedItems(1, 10, "ID", false).Result.ToList().
                Select(l => new LaptopViewModel
                {
                    Id = l.ID,
                    Name = l.Name,
                    Rate = l.Rate,
                    Price = l.Price,
                    NewPrice = l.NewPrice ?? 0,
                    imageSrc = l.imageSrc,
                    Color = l.Color,
                    CPU = l.CPU,
                    GPU = l.GPU,
                    HardDiskDescription = l.HardDiskDescription,
                    HardDiskSize = l.HardDiskSize,
                    ModelName = l.ModelName,
                    RAM = l.RAM,
                    OperatingSystem = l.OperatingSystem,
                    ScreenSize = l.ScreenSize,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, l.ID, "Laptops"),
                    CategoryName = l.Category.Name,
                    RateCount = _unitOfWork.Laptops.GetItemRates(l.ID, "Laptops").Result.Count()
                }).OrderBy(l => Guid.NewGuid()).ToList();

            var latestLaptops = _unitOfWork.Laptops.GetLatestItems(1, 10, "ID", false).Result.ToList().
                Select(l => new LaptopViewModel
                {
                    Id = l.ID,
                    Name = l.Name,
                    Rate = l.Rate,
                    Price = l.Price,
                    NewPrice = l.NewPrice ?? 0,
                    imageSrc = l.imageSrc,
                    Color = l.Color,
                    CPU = l.CPU,
                    GPU = l.GPU,
                    HardDiskDescription = l.HardDiskDescription,
                    HardDiskSize = l.HardDiskSize,
                    ModelName = l.ModelName,
                    RAM = l.RAM,
                    OperatingSystem = l.OperatingSystem,
                    ScreenSize = l.ScreenSize,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, l.ID, "Laptops"),
                    CategoryName = l.Category.Name,
                    RateCount = _unitOfWork.Laptops.GetItemRates(l.ID, "Laptops").Result.Count()
                }).OrderBy(l => Guid.NewGuid()).ToList();

            var laptopsVM = new ItemViewModel<LaptopViewModel>()
            {
                ItemCategories = laptopsCategories,
                DiscountedItems = discountedLaptops,
                latestItems = latestLaptops,
                TopRatedItems = topRatedLaptops
            };

            return View(laptopsVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var laptops = await _unitOfWork.Laptops.GetAll(pageNumber, pageSize);

            return View(laptops);
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
        public async Task<IActionResult> Add(Laptop data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.Laptops.Add(data);
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

            var Laptop = await _unitOfWork.Laptops.GetById(id);

            if (Laptop != null)
            {
                await CreateCategoriesSelectList();
                return View(Laptop);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(Laptop data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.Laptops.Update(data);
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

            var Laptop = await _unitOfWork.Laptops.GetById(id);

            if (Laptop != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(Laptop data)
        {
            await _unitOfWork.Laptops.Delete(data);
            await _unitOfWork.Commit();
            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> Laptops()
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
                var totalPages = (int)Math.Ceiling(await _unitOfWork.Laptops.TotalItems("Brands", null, null, name) / (double)pageSize);

                var laptops = _unitOfWork.Laptops.GetCategoryItems(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                    Select(l => new LaptopViewModel
                    {
                        Id = l.ID,
                        Name = l.Name,
                        Rate = l.Rate,
                        Price = l.Price,
                        NewPrice = l.NewPrice ?? 0,
                        imageSrc = l.imageSrc,
                        Color = l.Color,
                        CPU = l.CPU,
                        GPU = l.GPU,
                        HardDiskDescription = l.HardDiskDescription,
                        HardDiskSize = l.HardDiskSize,
                        ModelName = l.ModelName,
                        RAM = l.RAM,
                        OperatingSystem = l.OperatingSystem,
                        ScreenSize = l.ScreenSize,
                        isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, l.ID, "Laptops"),
                        CategoryName = l.Category.Name,
                        RateCount = _unitOfWork.Laptops.GetItemRates(l.ID, "Laptops").Result.Count()
                    }).ToList();

                var data = new ItemsViewModel
                {
                    Items = laptops,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    OrderIndex = orderIndex,
                    Des = des,
                    ActionName = "Brands",
                    Brand = name
                };
                return View("Laptops", data);
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
            var totalPages = (int)Math.Ceiling(await _unitOfWork.Laptops.TotalItems("Discounted") / (double)pageSize);

            var discountedLaptops = _unitOfWork.Laptops.GetDiscountedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(l => new LaptopViewModel
                {
                    Id = l.ID,
                    Name = l.Name,
                    Rate = l.Rate,
                    Price = l.Price,
                    NewPrice = l.NewPrice ?? 0,
                    imageSrc = l.imageSrc,
                    Color = l.Color,
                    CPU = l.CPU,
                    GPU = l.GPU,
                    HardDiskDescription = l.HardDiskDescription,
                    HardDiskSize = l.HardDiskSize,
                    ModelName = l.ModelName,
                    RAM = l.RAM,
                    OperatingSystem = l.OperatingSystem,
                    ScreenSize = l.ScreenSize,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, l.ID, "Laptops"),
                    CategoryName = l.Category.Name,
                    RateCount = _unitOfWork.Laptops.GetItemRates(l.ID, "Laptops").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = discountedLaptops,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
            return View("Laptops", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.Laptops.TotalItems("Rated") / (double)pageSize);

            var ratedLaptops = _unitOfWork.Laptops.GetTopRatedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(l => new LaptopViewModel
                {
                    Id = l.ID,
                    Name = l.Name,
                    Rate = l.Rate,
                    Price = l.Price,
                    NewPrice = l.NewPrice ?? 0,
                    imageSrc = l.imageSrc,
                    Color = l.Color,
                    CPU = l.CPU,
                    GPU = l.GPU,
                    HardDiskDescription = l.HardDiskDescription,
                    HardDiskSize = l.HardDiskSize,
                    ModelName = l.ModelName,
                    RAM = l.RAM,
                    OperatingSystem = l.OperatingSystem,
                    ScreenSize = l.ScreenSize,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, l.ID, "Laptops"),
                    CategoryName = l.Category.Name,
                    RateCount = _unitOfWork.Laptops.GetItemRates(l.ID, "Laptops").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = ratedLaptops,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
            return View("Laptops", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.Laptops.TotalItems("Latest") / (double)pageSize);

            var latestLaptops = _unitOfWork.Laptops.GetLatestItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(l => new LaptopViewModel
                {
                    Id = l.ID,
                    Name = l.Name,
                    Rate = l.Rate,
                    Price = l.Price,
                    NewPrice = l.NewPrice ?? 0,
                    imageSrc = l.imageSrc,
                    Color = l.Color,
                    CPU = l.CPU,
                    GPU = l.GPU,
                    HardDiskDescription = l.HardDiskDescription,
                    HardDiskSize = l.HardDiskSize,
                    ModelName = l.ModelName,
                    RAM = l.RAM,
                    OperatingSystem = l.OperatingSystem,
                    ScreenSize = l.ScreenSize,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, l.ID, "Laptops"),
                    CategoryName = l.Category.Name,
                    RateCount = _unitOfWork.Laptops.GetItemRates(l.ID, "Laptops").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = latestLaptops,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
            return View("Laptops", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.Laptops.TotalItems("Price", price1, price2, null) / (double)pageSize);

            var priceLaptops = _unitOfWork.Laptops.GetItemsFilteredByPrice(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                Select(l => new LaptopViewModel
                {
                    Id = l.ID,
                    Name = l.Name,
                    Rate = l.Rate,
                    Price = l.Price,
                    NewPrice = l.NewPrice ?? 0,
                    imageSrc = l.imageSrc,
                    Color = l.Color,
                    CPU = l.CPU,
                    GPU = l.GPU,
                    HardDiskDescription = l.HardDiskDescription,
                    HardDiskSize = l.HardDiskSize,
                    ModelName = l.ModelName,
                    RAM = l.RAM,
                    OperatingSystem = l.OperatingSystem,
                    ScreenSize = l.ScreenSize,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, l.ID, "Laptops"),
                    CategoryName = l.Category.Name,
                    RateCount = _unitOfWork.Laptops.GetItemRates(l.ID, "Laptops").Result.Count()
                }).ToList();

            var data = new ItemsViewModel
            {
                Items = priceLaptops,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
            return View("Laptops", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var Laptop = await _unitOfWork.Laptops.GetById(id);

                if (Laptop != null)
                {
                    var comments = await _unitOfWork.Laptops.GetItemComments(id, "Laptops", "Default");

                    var rateList = await _unitOfWork.Laptops.GetItemRates(id, "Laptops");
                    var rateCount = rateList.Count();

                    var starCounts = await _unitOfWork.Laptops.GetItemRateDetails(id, "Laptops");

                    var totalQuantity = await _unitOfWork.Carts.TotalItemQuantityInCart(id, "Laptops");

                    var similarPriceLaptops = _unitOfWork.Laptops.GetAllWithoutPagination().Result.
                        Where(l => l.Price == Laptop.Price || Math.Abs(l.Price - Laptop.Price) <= 1000).ToList()
                        .Select(l => new LaptopViewModel
                        {
                            Id = l.ID,
                            Name = l.Name,
                            Rate = l.Rate,
                            Price = l.Price,
                            NewPrice = l.NewPrice ?? 0,
                            IsDiscounted = l.IsDiscounted,
                            imageSrc = l.imageSrc,
                            Color = l.Color,
                            CPU = l.CPU,
                            GPU = l.GPU,
                            HardDiskDescription = l.HardDiskDescription,
                            HardDiskSize = l.HardDiskSize,
                            ModelName = l.ModelName,
                            RAM = l.RAM,
                            OperatingSystem = l.OperatingSystem,
                            ScreenSize = l.ScreenSize,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, l.ID, "Laptops"),
                            CategoryName = l.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var relatedLaptops = _unitOfWork.Laptops.GetAllWithoutPagination().Result
                        .Where(m => m.CategoryId == Laptop.CategoryId).Take(10).ToList().
                        Select(l => new LaptopViewModel
                        {
                            Id = l.ID,
                            Name = l.Name,
                            Rate = l.Rate,
                            Price = l.Price,
                            NewPrice = l.NewPrice ?? 0,
                            IsDiscounted = l.IsDiscounted,
                            imageSrc = l.imageSrc,
                            Color = l.Color,
                            CPU = l.CPU,
                            GPU = l.GPU,
                            HardDiskDescription = l.HardDiskDescription,
                            HardDiskSize = l.HardDiskSize,
                            ModelName = l.ModelName,
                            RAM = l.RAM,
                            OperatingSystem = l.OperatingSystem,
                            ScreenSize = l.ScreenSize,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, l.ID, "Laptops"),
                            CategoryName = l.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var offers = await _unitOfWork.Offers.GetOffers("Electronics", Laptop.Category?.Name, Laptop.ID);

                    var discountValue = string.Empty;
                    if (offers.Any())
                    {
                        discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                           $"{offers.First().PercentDiscount}%" :
                           offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;
                    }

                    var BOGOGetItem = await _unitOfWork.Offers.GetBOGOGetItem(Laptop);

                    var laptop = new LaptopViewModel
                    {
                        Id = id,
                        Name = Laptop.Name,
                        Rate = Laptop.Rate,
                        Price = Laptop.Price,
                        NewPrice = Laptop.NewPrice ?? 0,
                        IsDiscounted = Laptop.IsDiscounted,
                        DiscountValue = discountValue,
                        IsBOGOBuy = Laptop.IsBOGOBuy,
                        IsBOGOGet = Laptop.IsBOGOGet,
                        imageSrc = Laptop.imageSrc,
                        Color = Laptop.Color,
                        CPU = Laptop.CPU,
                        GPU = Laptop.GPU,
                        HardDiskDescription = Laptop.HardDiskDescription,
                        HardDiskSize = Laptop.HardDiskSize,
                        ModelName = Laptop.ModelName,
                        RAM = Laptop.RAM,
                        OperatingSystem = Laptop.OperatingSystem,
                        ScreenSize = Laptop.ScreenSize,
                        CategoryName = Laptop.Category.Name,
                        RelatedLaptops = relatedLaptops,
                        SimilarPriceLaptops = similarPriceLaptops,
                        Comments = comments,
                        Offers = offers,
                        BOGOGet = BOGOGetItem,
                        StarCounts = starCounts,
                        RateCount = rateCount,
                        ControllerName = "Laptops",
                        TotalQuantity = totalQuantity
                    };

                    return View(laptop);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllLaptopComments(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var Laptop = await _unitOfWork.Laptops.GetById(id);

                var rateList = await _unitOfWork.Laptops.GetItemRates(id, "Laptops");
                var rateCount = rateList.Count();

                var starCounts = await _unitOfWork.Laptops.GetItemRateDetails(id, "Laptops");

                if (Laptop != null)
                {
                    var comments = await _unitOfWork.Laptops.GetItemComments(id, "Laptops", "All");

                    if (comments.Any())
                    {
                        var laptop = new LaptopViewModel
                        {
                            Id = Laptop.ID,
                            Name = Laptop.Name,
                            Rate = Laptop.Rate,
                            CategoryName = Laptop.Category.Name,
                            Comments = comments,
                            StarCounts = starCounts,
                            RateCount = rateCount
                        };

                        return View("AllComments", laptop);
                    }
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}
