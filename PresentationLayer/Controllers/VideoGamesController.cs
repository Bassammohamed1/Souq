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
    public class VideoGamesController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _videoGame.GetSpecificCategoriesForSelectList();

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IVideoGamesService _videoGame;
        private readonly IDepartmentsService _departments;

        public VideoGamesController(IVideoGamesService videoGame, IDepartmentsService departments)
        {
            _videoGame = videoGame;
            _departments = departments;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = _videoGame.GetVideoGamesWithRelatedOnes();

            var videoGamesVM = new ItemViewModel<VideoGameViewModel>()
            {
                ItemCategories = result.ItemCategories,
                DiscountedItems = result.DiscountedItems
                .Select(i => new VideoGameViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Rate = i.Rate,
                    Price = i.Price,
                    NewPrice = i.NewPrice,
                    imageSrc = i.imageSrc,
                    isLiked = i.isLiked,
                    CategoryName = i.CategoryName,
                    RateCount = i.RateCount
                }),
                latestItems = result.latestItems
                .Select(i => new VideoGameViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Rate = i.Rate,
                    Price = i.Price,
                    NewPrice = i.NewPrice,
                    imageSrc = i.imageSrc,
                    isLiked = i.isLiked,
                    CategoryName = i.CategoryName,
                    RateCount = i.RateCount
                }),
                TopRatedItems = result.TopRatedItems
                .Select(i => new VideoGameViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Rate = i.Rate,
                    Price = i.Price,
                    NewPrice = i.NewPrice,
                    imageSrc = i.imageSrc,
                    isLiked = i.isLiked,
                    CategoryName = i.CategoryName,
                    RateCount = i.RateCount
                }),
                Offers = result.Offers ?? Enumerable.Empty<Offer>().AsQueryable()
            };

            return View(videoGamesVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var videoGames = _videoGame.GetVideoGames(pageNumber, pageSize);

            return View(videoGames);
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
        public async Task<IActionResult> Add(VideoGame data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _videoGame.Add(data);

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

            var VideoGame = await _videoGame.GetVideoGame(id);

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
                await _videoGame.Update(data);

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

            var VideoGame = await _videoGame.GetVideoGame(id);

            if (VideoGame != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(VideoGame data)
        {
            await _videoGame.Delete(data);

            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> VideoGames()
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

                var result = await _videoGame.GetBrandsVideoGames(orderIndex, page, name, des);

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

                return View("VideoGames", data);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Discounted(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _videoGame.GetDiscountedVideoGames(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("VideoGames", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _videoGame.GetTopRatedVideoGames(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("VideoGames", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _videoGame.GetLatestVideoGames(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("VideoGames", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _videoGame.GetVideoGamesWithPriceFilter(orderIndex, page, price1, price2, des);

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

            return View("VideoGames", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _videoGame.GetVideoGameDetails(id);

                if (result != null)
                {
                    var videoGames = new VideoGameViewModel
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
                        CategoryName = result.CategoryName,
                        RelatedVideoGames = result.RelatedVideoGames
                        .Select(c => new VideoGameViewModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Rate = c.Rate,
                            Price = c.Price,
                            NewPrice = c.NewPrice,
                            imageSrc = c.imageSrc,
                            isLiked = c.isLiked,
                            CategoryName = c.CategoryName,
                            RateCount = c.RateCount
                        }),
                        SimilarPriceVideoGames = result.SimilarPriceVideoGames
                        .Select(c => new VideoGameViewModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Rate = c.Rate,
                            Price = c.Price,
                            NewPrice = c.NewPrice,
                            imageSrc = c.imageSrc,
                            isLiked = c.isLiked,
                            CategoryName = c.CategoryName,
                            RateCount = c.RateCount
                        }),
                        Comments = result.Comments,
                        Offers = result.Offers,
                        BOGOGet = result.BOGOGet,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount,
                        ControllerName = result.ControllerName,
                        TotalQuantity = result.TotalQuantity
                    };

                    return View(videoGames);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllVideoGameComments(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _videoGame.GetVideoGameAllComments(id);

                if (result is not null)
                {
                    var videoGame = new VideoGameViewModel
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Rate = result.Rate,
                        CategoryName = result.CategoryName,
                        Comments = result.Comments,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount
                    };

                    return View("AllComments", videoGame);
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}