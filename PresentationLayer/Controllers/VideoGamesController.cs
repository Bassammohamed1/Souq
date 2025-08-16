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
    public class VideoGamesController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _unitOfWork.Categories.GetSpecificCategories("Video Games");

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public VideoGamesController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var videoGamesCategories = await _unitOfWork.VideoGames.GetItemCategories().Result.ToListAsync();

            var discountedVideoGames = _unitOfWork.VideoGames.GetDiscountedItems(1, 10, "ID", false).Result.ToList().
                Select(v => new VideoGameViewModel
                {
                    Id = v.ID,
                    Name = v.Name,
                    Rate = v.Rate,
                    Price = v.Price,
                    NewPrice = v.NewPrice ?? 0,
                    imageSrc = v.imageSrc,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, v.ID, "VideoGames"),
                    CategoryName = v.Category.Name,
                    RateCount = _unitOfWork.VideoGames.GetItemRates(v.ID, "VideoGames").Result.Count()
                }).OrderBy(v => Guid.NewGuid()).ToList();

            var topRatedVideoGames = _unitOfWork.VideoGames.GetTopRatedItems(1, 10, "ID", false).Result.ToList().
                Select(v => new VideoGameViewModel
                {
                    Id = v.ID,
                    Name = v.Name,
                    Rate = v.Rate,
                    Price = v.Price,
                    NewPrice = v.NewPrice ?? 0,
                    imageSrc = v.imageSrc,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, v.ID, "VideoGames"),
                    CategoryName = v.Category.Name,
                    RateCount = _unitOfWork.VideoGames.GetItemRates(v.ID, "VideoGames").Result.Count()
                }).OrderBy(v => Guid.NewGuid()).ToList();

            var latesVideoGameideoGames = _unitOfWork.VideoGames.GetLatestItems(1, 10, "ID", false).Result.ToList().
                Select(v => new VideoGameViewModel
                {
                    Id = v.ID,
                    Name = v.Name,
                    Rate = v.Rate,
                    Price = v.Price,
                    NewPrice = v.NewPrice ?? 0,
                    imageSrc = v.imageSrc,
                    isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, v.ID, "VideoGames"),
                    CategoryName = v.Category.Name,
                    RateCount = _unitOfWork.VideoGames.GetItemRates(v.ID, "VideoGames").Result.Count()
                }).OrderBy(v => Guid.NewGuid()).ToList();

            var videoGamesDepartment = _unitOfWork.Departments.GetAllWithoutPagination().Result
               .Where(d => d.Name == "Video Games").FirstOrDefault();

            var offers = await _unitOfWork.Offers.GetOffers(videoGamesDepartment.Name, null, null);

            var videoGamesVM = new ItemViewModel<VideoGameViewModel>()
            {
                ItemCategories = videoGamesCategories,
                DiscountedItems = discountedVideoGames,
                latestItems = latesVideoGameideoGames,
                TopRatedItems = topRatedVideoGames,
                Offers = offers
            };

            return View(videoGamesVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var videoGames = await _unitOfWork.VideoGames.GetAll(pageNumber, pageSize);

            return View(videoGames);
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
        public async Task<IActionResult> Add(VideoGame data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.VideoGames.Add(data);
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

            var VideoGame = await _unitOfWork.VideoGames.GetById(id);

            if (VideoGame != null)
            {
                await CreateCategoriesSelectList();
                return View(VideoGame);
            }

            throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(VideoGame data)
        {
            if (data is not null && data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);
                data.dbImage = stream.ToArray();

                await _unitOfWork.VideoGames.Update(data);
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

            var VideoGame = await _unitOfWork.VideoGames.GetById(id);

            if (VideoGame != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(VideoGame data)
        {
            await _unitOfWork.VideoGames.Delete(data);
            await _unitOfWork.Commit();
            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> VideoGames()
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
                var totalPages = (int)Math.Ceiling(await _unitOfWork.VideoGames.TotalItems("Brands", null, null, name) / (double)pageSize);

                var videoGames = _unitOfWork.VideoGames.GetCategoryItems(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                     Select(v => new VideoGameViewModel
                     {
                         Id = v.ID,
                         Name = v.Name,
                         Rate = v.Rate,
                         Price = v.Price,
                         NewPrice = v.NewPrice ?? 0,
                         imageSrc = v.imageSrc,
                         isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, v.ID, "VideoGames"),
                         CategoryName = v.Category.Name,
                         RateCount = _unitOfWork.VideoGames.GetItemRates(v.ID, "VideoGames").Result.Count()
                     }).ToList();

                var data = new ItemsViewModel
                {
                    Items = videoGames,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages,
                    OrderIndex = orderIndex,
                    Des = des,
                    ActionName = "Brands",
                    Brand = name
                };
                return View("VideoGames", data);
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
            var totalPages = (int)Math.Ceiling(await _unitOfWork.VideoGames.TotalItems("Discounted") / (double)pageSize);

            var discountedVideoGames = _unitOfWork.VideoGames.GetDiscountedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(v => new VideoGameViewModel
                 {
                     Id = v.ID,
                     Name = v.Name,
                     Rate = v.Rate,
                     Price = v.Price,
                     NewPrice = v.NewPrice ?? 0,
                     imageSrc = v.imageSrc,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, v.ID, "VideoGames"),
                     CategoryName = v.Category.Name,
                     RateCount = _unitOfWork.VideoGames.GetItemRates(v.ID, "VideoGames").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = discountedVideoGames,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
            return View("VideoGames", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.VideoGames.TotalItems("Rated") / (double)pageSize);

            var ratedVideoGames = _unitOfWork.VideoGames.GetTopRatedItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(v => new VideoGameViewModel
                 {
                     Id = v.ID,
                     Name = v.Name,
                     Rate = v.Rate,
                     Price = v.Price,
                     NewPrice = v.NewPrice ?? 0,
                     imageSrc = v.imageSrc,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, v.ID, "VideoGames"),
                     CategoryName = v.Category.Name,
                     RateCount = _unitOfWork.VideoGames.GetItemRates(v.ID, "VideoGames").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = ratedVideoGames,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
            return View("VideoGames", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.VideoGames.TotalItems("Latest") / (double)pageSize);

            var latesVideoGameideoGames = _unitOfWork.VideoGames.GetLatestItems(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(v => new VideoGameViewModel
                 {
                     Id = v.ID,
                     Name = v.Name,
                     Rate = v.Rate,
                     Price = v.Price,
                     NewPrice = v.NewPrice ?? 0,
                     imageSrc = v.imageSrc,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, v.ID, "VideoGames"),
                     CategoryName = v.Category.Name,
                     RateCount = _unitOfWork.VideoGames.GetItemRates(v.ID, "VideoGames").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = latesVideoGameideoGames,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
            return View("VideoGames", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _unitOfWork.VideoGames.TotalItems("Price", price1, price2, null) / (double)pageSize);

            var priceVideoGames = _unitOfWork.VideoGames.GetItemsFilteredByPrice(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).Result.ToList().
                 Select(v => new VideoGameViewModel
                 {
                     Id = v.ID,
                     Name = v.Name,
                     Rate = v.Rate,
                     Price = v.Price,
                     NewPrice = v.NewPrice ?? 0,
                     imageSrc = v.imageSrc,
                     isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, v.ID, "VideoGames"),
                     CategoryName = v.Category.Name,
                     RateCount = _unitOfWork.VideoGames.GetItemRates(v.ID, "VideoGames").Result.Count()
                 }).ToList();

            var data = new ItemsViewModel
            {
                Items = priceVideoGames,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
            return View("VideoGames", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var VideoGame = await _unitOfWork.VideoGames.GetById(id);

                if (VideoGame != null)
                {
                    var comments = await _unitOfWork.VideoGames.GetItemComments(id, "VideoGames", "Default");

                    var rateList = await _unitOfWork.VideoGames.GetItemRates(id, "VideoGames");
                    var rateCount = rateList.Count();

                    var starCounts = await _unitOfWork.VideoGames.GetItemRateDetails(id, "VideoGames");

                    var totalQuantity = await _unitOfWork.Carts.TotalItemQuantityInCart(id, "VideoGames");

                    var similarPriceVideoGames = _unitOfWork.VideoGames.GetAllWithoutPagination().Result.
                        Where(v => v.Price == VideoGame.Price || Math.Abs(v.Price - VideoGame.Price) <= 1000).ToList()
                        .Select(v => new VideoGameViewModel
                        {
                            Id = v.ID,
                            Name = v.Name,
                            Rate = v.Rate,
                            Price = v.Price,
                            NewPrice = v.NewPrice ?? 0,
                            IsDiscounted = v.IsDiscounted,
                            imageSrc = v.imageSrc,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, v.ID, "VideoGames"),
                            CategoryName = v.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var relatedVideoGames = _unitOfWork.VideoGames.GetAllWithoutPagination().Result
                        .Where(v => v.CategoryId == VideoGame.CategoryId).Take(10).ToList().
                        Select(v => new VideoGameViewModel
                        {
                            Id = v.ID,
                            Name = v.Name,
                            Rate = v.Rate,
                            Price = v.Price,
                            NewPrice = v.NewPrice ?? 0,
                            IsDiscounted = v.IsDiscounted,
                            imageSrc = v.imageSrc,
                            isLiked = _unitOfWork.WishLists.HasUserLiked(_userService.GetUserId().Result, v.ID, "VideoGames"),
                            CategoryName = v.Category.Name,
                            RateCount = rateCount
                        }).ToList();

                    var offers = await _unitOfWork.Offers.GetOffers("Video Games", VideoGame.Category?.Name, VideoGame.ID);

                    var discountValue = string.Empty;
                    if (offers.Any())
                    {
                        discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                           $"{offers.First().PercentDiscount}%" :
                           offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;
                    }

                    var BOGOGetItem = await _unitOfWork.Offers.GetBOGOGetItem(VideoGame);

                    var videoGame = new VideoGameViewModel
                    {
                        Id = id,
                        Name = VideoGame.Name,
                        Rate = VideoGame.Rate,
                        Price = VideoGame.Price,
                        NewPrice = VideoGame.NewPrice ?? 0,
                        IsDiscounted = VideoGame.IsDiscounted,
                        DiscountValue = discountValue,
                        IsBOGOBuy = VideoGame.IsBOGOBuy,
                        IsBOGOGet = VideoGame.IsBOGOGet,
                        imageSrc = VideoGame.imageSrc,
                        CategoryName = VideoGame.Category.Name,
                        RelatedVideoGames = relatedVideoGames,
                        SimilarPriceVideoGames = similarPriceVideoGames,
                        Comments = comments,
                        Offers = offers,
                        BOGOGet = BOGOGetItem,
                        StarCounts = starCounts,
                        RateCount = rateCount,
                        ControllerName = "VideoGames",
                        TotalQuantity = totalQuantity
                    };

                    return View(videoGame);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllVideoGameComments(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var VideoGame = await _unitOfWork.VideoGames.GetById(id);

                var rateList = await _unitOfWork.VideoGames.GetItemRates(id, "VideoGames");
                var rateCount = rateList.Count();

                var starCounts = await _unitOfWork.VideoGames.GetItemRateDetails(id, "VideoGames");

                if (VideoGame != null)
                {
                    var comments = await _unitOfWork.VideoGames.GetItemComments(id, "VideoGames", "All");

                    if (comments.Any())
                    {
                        var videoGame = new VideoGameViewModel
                        {
                            Id = VideoGame.ID,
                            Name = VideoGame.Name,
                            Rate = VideoGame.Rate,
                            CategoryName = VideoGame.Category.Name,
                            Comments = comments,
                            StarCounts = starCounts,
                            RateCount = rateCount
                        };

                        return View("AllComments", videoGame);
                    }
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}
