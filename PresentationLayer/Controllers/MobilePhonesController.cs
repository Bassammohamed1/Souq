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
    public class MobilePhonesController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _unitOfWork.Categories.GetSpecificCategories("Mobile Phones");

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public MobilePhonesController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var phonesCategories = await _unitOfWork.MobilePhones.GetItemsCategories("Mobile Phones").Result.ToListAsync();

            var discountedPhones = _unitOfWork.MobilePhones.GetDiscountedItems(1, 10, "ID", false).Result.ToList().
                Select(p => new MobilePhoneViewModel
                {
                    Id = p.ID,
                    Name = p.Name,
                    Rate = p.Rate,
                    Price = p.Price,
                    NewPrice = p.NewPrice ?? 0,
                    imageSrc = p.imageSrc,
                    RAM = p.RAM,
                    OperatingSystem = p.OperatingSystem,
                    CPUModel = p.CPUModel,
                    MemoryStorageCapacity = p.MemoryStorageCapacity,
                    Color = p.Color,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, p.ID, "MobilePhones"),
                    CategoryName = p.Category.Name,
                    RateCount = _unitOfWork.MobilePhones.GetItemRates(p.ID, "MobilePhones").Result.Count()
                }).OrderBy(p => Guid.NewGuid()).ToList();

            var topRatedPhones = _unitOfWork.MobilePhones.GetTopRatedItems(1, 10, "ID", false).Result.ToList().
                Select(p => new MobilePhoneViewModel
                {
                    Id = p.ID,
                    Name = p.Name,
                    Rate = p.Rate,
                    Price = p.Price,
                    NewPrice = p.NewPrice ?? 0,
                    imageSrc = p.imageSrc,
                    RAM = p.RAM,
                    OperatingSystem = p.OperatingSystem,
                    CPUModel = p.CPUModel,
                    MemoryStorageCapacity = p.MemoryStorageCapacity,
                    Color = p.Color,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, p.ID, "MobilePhones"),
                    CategoryName = p.Category.Name,
                    RateCount = _unitOfWork.MobilePhones.GetItemRates(p.ID, "MobilePhones").Result.Count()
                }).OrderBy(p => Guid.NewGuid()).ToList();

            var latestPhones = _unitOfWork.MobilePhones.GetLatestItems(1, 10, "ID", false).Result.ToList().
                Select(p => new MobilePhoneViewModel
                {
                    Id = p.ID,
                    Name = p.Name,
                    Rate = p.Rate,
                    Price = p.Price,
                    NewPrice = p.NewPrice ?? 0,
                    imageSrc = p.imageSrc,
                    RAM = p.RAM,
                    OperatingSystem = p.OperatingSystem,
                    CPUModel = p.CPUModel,
                    MemoryStorageCapacity = p.MemoryStorageCapacity,
                    Color = p.Color,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, p.ID, "MobilePhones"),
                    CategoryName = p.Category.Name,
                    RateCount = _unitOfWork.MobilePhones.GetItemRates(p.ID, "MobilePhones").Result.Count()
                }).OrderBy(p => Guid.NewGuid()).ToList();

            var mobilePhonesDepartment = _unitOfWork.Departments.GetAllWithoutPagination().Result
                .Where(d => d.Name == "Mobile Phones").FirstOrDefault();

            var offers = await _unitOfWork.Offers.GetOffers(mobilePhonesDepartment.Name, null, null);

            var phonesVM = new ItemViewModel<MobilePhoneViewModel>()
            {
                ItemCategories = phonesCategories,
                DiscountedItems = discountedPhones,
                latestItems = latestPhones,
                TopRatedItems = topRatedPhones,
                Offers = offers
            };

            return View(phonesVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var mobilePhones = await _unitOfWork.MobilePhones.GetAll(pageNumber, pageSize);

            return View(mobilePhones);
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
        public async Task<IActionResult> Add(MobilePhone data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.MobilePhones.Add(data);
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

            var mobilePhone = await _unitOfWork.MobilePhones.GetById(id);

            if (mobilePhone != null)
            {
                await CreateCategoriesSelectList();
                return View(mobilePhone);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(MobilePhone data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.MobilePhones.Update(data);
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

            var mobilePhone = await _unitOfWork.MobilePhones.GetById(id);

            if (mobilePhone != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(MobilePhone data)
        {
            await _unitOfWork.MobilePhones.Delete(data);
            await _unitOfWork.Commit();
            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> MobilePhones()
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
                var totalPages = (int)Math.Ceiling(await _unitOfWork.MobilePhones.TotalItems("Brands", null, null, name) / (double)pageSize);

                var phones = _unitOfWork.MobilePhones.GetCategoryItems(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                     Select(p => new MobilePhoneViewModel
                     {
                         Id = p.ID,
                         Name = p.Name,
                         Rate = p.Rate,
                         Price = p.Price,
                         NewPrice = p.NewPrice ?? 0,
                         imageSrc = p.imageSrc,
                         RAM = p.RAM,
                         OperatingSystem = p.OperatingSystem,
                         CPUModel = p.CPUModel,
                         MemoryStorageCapacity = p.MemoryStorageCapacity,
                         Color = p.Color,
                         isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, p.ID, "MobilePhones"),
                         CategoryName = p.Category.Name,
                         RateCount = _unitOfWork.MobilePhones.GetItemRates(p.ID, "MobilePhones").Result.Count()
                     }).ToList();

                var data = new ItemsViewModel
                {
                    Items = phones,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    OrderIndex = orderIndex,
                    Des = des,
                    ActionName = "Brands",
                    Brand = name
                };
                return View("MobilePhones", data);
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
            var totalPages = (int)Math.Ceiling(await _unitOfWork.MobilePhones.TotalItems("Discounted") / (double)pageSize);


            var discountedPhones = _unitOfWork.MobilePhones.GetDiscountedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(p => new MobilePhoneViewModel
                 {
                     Id = p.ID,
                     Name = p.Name,
                     Rate = p.Rate,
                     Price = p.Price,
                     NewPrice = p.NewPrice ?? 0,
                     imageSrc = p.imageSrc,
                     RAM = p.RAM,
                     OperatingSystem = p.OperatingSystem,
                     CPUModel = p.CPUModel,
                     MemoryStorageCapacity = p.MemoryStorageCapacity,
                     Color = p.Color,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, p.ID, "MobilePhones"),
                     CategoryName = p.Category.Name,
                     RateCount = _unitOfWork.MobilePhones.GetItemRates(p.ID, "MobilePhones").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = discountedPhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                ActionName = "Discounted",
                OrderIndex = orderIndex,
                Des = des
            };
            return View("MobilePhones", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.MobilePhones.TotalItems("Rated") / (double)pageSize);


            var ratedPhones = _unitOfWork.MobilePhones.GetTopRatedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(p => new MobilePhoneViewModel
                 {
                     Id = p.ID,
                     Name = p.Name,
                     Rate = p.Rate,
                     Price = p.Price,
                     NewPrice = p.NewPrice ?? 0,
                     imageSrc = p.imageSrc,
                     RAM = p.RAM,
                     OperatingSystem = p.OperatingSystem,
                     CPUModel = p.CPUModel,
                     MemoryStorageCapacity = p.MemoryStorageCapacity,
                     Color = p.Color,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, p.ID, "MobilePhones"),
                     CategoryName = p.Category.Name,
                     RateCount = _unitOfWork.MobilePhones.GetItemRates(p.ID, "MobilePhones").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = ratedPhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                ActionName = "TopRated",
                OrderIndex = orderIndex,
                Des = des
            };
            return View("MobilePhones", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.MobilePhones.TotalItems("Latest") / (double)pageSize);

            var latestPhones = _unitOfWork.MobilePhones.GetLatestItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(p => new MobilePhoneViewModel
                 {
                     Id = p.ID,
                     Name = p.Name,
                     Rate = p.Rate,
                     Price = p.Price,
                     NewPrice = p.NewPrice ?? 0,
                     imageSrc = p.imageSrc,
                     RAM = p.RAM,
                     OperatingSystem = p.OperatingSystem,
                     CPUModel = p.CPUModel,
                     MemoryStorageCapacity = p.MemoryStorageCapacity,
                     Color = p.Color,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, p.ID, "MobilePhones"),
                     CategoryName = p.Category.Name,
                     RateCount = _unitOfWork.MobilePhones.GetItemRates(p.ID, "MobilePhones").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = latestPhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                ActionName = "Latest",
                OrderIndex = orderIndex,
                Des = des
            };
            return View("MobilePhones", data);
        }

        public async Task<IActionResult> StorageFilter(string? orderIndex, int? page, int storage, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.MobilePhones.TotalFilterStoragePhones(storage) / (double)pageSize);

            var storagePhones = _unitOfWork.MobilePhones.GetPhonesFilteredByStorage(storage, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(p => new MobilePhoneViewModel
                 {
                     Id = p.ID,
                     Name = p.Name,
                     Rate = p.Rate,
                     Price = p.Price,
                     NewPrice = p.NewPrice ?? 0,
                     imageSrc = p.imageSrc,
                     RAM = p.RAM,
                     OperatingSystem = p.OperatingSystem,
                     CPUModel = p.CPUModel,
                     MemoryStorageCapacity = p.MemoryStorageCapacity,
                     Color = p.Color,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, p.ID, "MobilePhones"),
                     CategoryName = p.Category.Name,
                     RateCount = _unitOfWork.MobilePhones.GetItemRates(p.ID, "MobilePhones").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = storagePhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "StorageFilter",
                Storage = storage
            };
            return View("MobilePhones", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.MobilePhones.TotalItems("Price", price1, price2, null) / (double)pageSize);

            var pricePhones = _unitOfWork.MobilePhones.GetItemsFilteredByPrice(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(p => new MobilePhoneViewModel
                 {
                     Id = p.ID,
                     Name = p.Name,
                     Rate = p.Rate,
                     Price = p.Price,
                     NewPrice = p.NewPrice ?? 0,
                     imageSrc = p.imageSrc,
                     RAM = p.RAM,
                     OperatingSystem = p.OperatingSystem,
                     CPUModel = p.CPUModel,
                     MemoryStorageCapacity = p.MemoryStorageCapacity,
                     Color = p.Color,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, p.ID, "MobilePhones"),
                     CategoryName = p.Category.Name,
                     RateCount = _unitOfWork.MobilePhones.GetItemRates(p.ID, "MobilePhones").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = pricePhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
            return View("MobilePhones", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var Phone = await _unitOfWork.MobilePhones.GetById(id);

                if (Phone != null)
                {
                    var comments = await _unitOfWork.MobilePhones.GetItemComments(id, "MobilePhones", "Default");

                    var rateList = await _unitOfWork.MobilePhones.GetItemRates(id, "MobilePhones");
                    var rateCount = rateList.Count();

                    var starCounts = await _unitOfWork.MobilePhones.GetItemRateDetails(id, "MobilePhones");

                    var totalQuantity = await _unitOfWork.Carts.TotalItemQuantityInCart(id, "MobilePhones");

                    var similarPricePhones = _unitOfWork.MobilePhones.GetAllWithoutPagination().Result.
                        Where(p => p.Price == Phone.Price || Math.Abs(p.Price - Phone.Price) <= 1000).ToList()
                        .Select(p => new MobilePhoneViewModel
                        {
                            Id = p.ID,
                            Name = p.Name,
                            Rate = p.Rate,
                            Price = p.Price,
                            NewPrice = p.NewPrice ?? 0,
                            IsDiscounted = p.IsDiscounted,
                            imageSrc = p.imageSrc,
                            RAM = p.RAM,
                            OperatingSystem = p.OperatingSystem,
                            CPUModel = p.CPUModel,
                            MemoryStorageCapacity = p.MemoryStorageCapacity,
                            Color = p.Color,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, p.ID, "MobilePhones"),
                            CategoryName = p.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var relatedPhones = _unitOfWork.MobilePhones.GetAllWithoutPagination().Result
                        .Where(m => m.CategoryId == Phone.CategoryId).Take(10).ToList().
                        Select(p => new MobilePhoneViewModel
                        {
                            Id = p.ID,
                            Name = p.Name,
                            Rate = p.Rate,
                            Price = p.Price,
                            NewPrice = p.NewPrice ?? 0,
                            IsDiscounted = p.IsDiscounted,
                            imageSrc = p.imageSrc,
                            RAM = p.RAM,
                            OperatingSystem = p.OperatingSystem,
                            CPUModel = p.CPUModel,
                            MemoryStorageCapacity = p.MemoryStorageCapacity,
                            Color = p.Color,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, p.ID, "MobilePhones"),
                            CategoryName = p.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var offers = await _unitOfWork.Offers.GetOffers("Mobile Phones", Phone.Category?.Name, Phone.ID);

                    var discountValue = string.Empty;
                    if (offers.Any())
                    {
                        discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                           $"{offers.First().PercentDiscount}%" :
                           offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;
                    }

                    var BOGOGetItem = await _unitOfWork.Offers.GetBOGOGetItem(Phone);

                    var phone = new MobilePhoneViewModel
                    {
                        Id = id,
                        Name = Phone.Name,
                        Rate = Phone.Rate,
                        Price = Phone.Price,
                        NewPrice = Phone.NewPrice ?? 0,
                        IsDiscounted = Phone.IsDiscounted,
                        DiscountValue = discountValue,
                        IsBOGOBuy = Phone.IsBOGOBuy,
                        IsBOGOGet = Phone.IsBOGOGet,
                        imageSrc = Phone.imageSrc,
                        RAM = Phone.RAM,
                        OperatingSystem = Phone.OperatingSystem,
                        CPUModel = Phone.CPUModel,
                        MemoryStorageCapacity = Phone.MemoryStorageCapacity,
                        Color = Phone.Color,
                        CategoryName = Phone.Category.Name,
                        RelatedPhones = relatedPhones,
                        SimilarPricePhones = similarPricePhones,
                        Comments = comments,
                        Offers = offers,
                        BOGOGet = BOGOGetItem,
                        StarCounts = starCounts,
                        RateCount = rateCount,
                        ControllerName = "MobilePhones",
                        TotalQuantity = totalQuantity,
                    };

                    return View(phone);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllPhoneComments(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var Phone = await _unitOfWork.MobilePhones.GetById(id);

                var rateList = await _unitOfWork.MobilePhones.GetItemRates(id, "MobilePhones");
                var rateCount = rateList.Count();

                var starCounts = await _unitOfWork.MobilePhones.GetItemRateDetails(id, "MobilePhones");

                if (Phone != null)
                {
                    var comments = await _unitOfWork.MobilePhones.GetItemComments(id, "MobilePhones", "All");

                    if (comments.Any())
                    {
                        var phone = new MobilePhoneViewModel
                        {
                            Id = Phone.ID,
                            Name = Phone.Name,
                            Rate = Phone.Rate,
                            CategoryName = Phone.Category.Name,
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