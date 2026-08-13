using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresentationLayer.ViewModels;
using PresentationLayer.ViewModels.ItemVMs;

namespace PresentationLayer.Controllers
{
    [AllowAnonymous]
    public class HeadPhonesController : Controller
    {
        private async Task CreateCategoriesSelectList()
        {
            var allCategories = await _headPhone.GetSpecificCategoriesForSelectList();

            var categoriesList = new SelectList(allCategories.OrderBy(c => c.Name), "ID", "Name");

            ViewBag.categoriesViewBag = categoriesList;
        }

        private readonly IHeadPhonesService _headPhone;
        private readonly IDepartmentsService _departments;

        public HeadPhonesController(IHeadPhonesService headPhone, IDepartmentsService departments)
        {
            _headPhone = headPhone;
            _departments = departments;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = _headPhone.GetHeadPhonesWithRelatedOnes();

            var headPhonesVM = new ItemViewModel<HeadPhoneViewModel>()
            {
                ItemCategories = result.ItemCategories,
                DiscountedItems = result.DiscountedItems
                .Select(h => new HeadPhoneViewModel
                {
                    Id = h.Id,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = h.isLiked,
                    CategoryName = h.CategoryName,
                    RateCount = h.RateCount
                }),
                latestItems = result.latestItems
                .Select(h => new HeadPhoneViewModel
                {
                    Id = h.Id,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = h.isLiked,
                    CategoryName = h.CategoryName,
                    RateCount = h.RateCount
                }),
                TopRatedItems = result.TopRatedItems
                .Select(h => new HeadPhoneViewModel
                {
                    Id = h.Id,
                    Name = h.Name,
                    Rate = h.Rate,
                    Price = h.Price,
                    NewPrice = h.NewPrice,
                    imageSrc = h.imageSrc,
                    ConnectivityTechnology = h.ConnectivityTechnology,
                    Color = h.Color,
                    NoiseControl = h.NoiseControl,
                    HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                    HeadphonesFormFactor = h.HeadphonesFormFactor,
                    ModelName = h.ModelName,
                    isLiked = h.isLiked,
                    CategoryName = h.CategoryName,
                    RateCount = h.RateCount
                }),
                Offers = result.Offers ?? Enumerable.Empty<Offer>().AsQueryable()
            };

            return View(headPhonesVM);
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var headPhones = _headPhone.GetHeadPhones(pageNumber, pageSize);

            return View(headPhones);
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
        public async Task<IActionResult> Add(HeadPhone data)
        {
            if (data is not null && data.clientFile is not null)
            {
                await _headPhone.Add(data);

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

            var HeadPhone = await _headPhone.GetHeadPhone(id);

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
                await _headPhone.Update(data);

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

            var HeadPhone = await _headPhone.GetHeadPhone(id);

            if (HeadPhone != null)
                return View();
            else
                throw new ArgumentNullException("Invalid id!!");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(HeadPhone data)
        {
            await _headPhone.Delete(data);

            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> HeadPhones()
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

                var result = await _headPhone.GetBrandsHeadPhones(orderIndex, page, name, des);

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

                return View("HeadPhones", data);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Discounted(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _headPhone.GetDiscountedHeadPhones(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("HeadPhones", data);
        }

        public async Task<IActionResult> TopRated(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _headPhone.GetTopRatedHeadPhones(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("HeadPhones", data);
        }

        public async Task<IActionResult> Latest(string? orderIndex, int? page, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _headPhone.GetLatestHeadPhones(orderIndex, page, des);

            var data = new ItemsViewModel
            {
                Items = result.Items,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                OrderIndex = result.OrderIndex,
                Des = result.Des,
                ActionName = result.ActionName,
            };

            return View("HeadPhones", data);
        }

        public async Task<IActionResult> PriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _headPhone.GetHeadPhonesWithPriceFilter(orderIndex, page, price1, price2, des);

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

            return View("HeadPhones", data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _headPhone.GetHeadPhoneDetails(id);

                if (result != null)
                {
                    var headPhones = new HeadPhoneViewModel
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
                        ConnectivityTechnology = result.ConnectivityTechnology,
                        Color = result.Color,
                        NoiseControl = result.NoiseControl,
                        HeadphonesEarPlacement = result.HeadphonesEarPlacement,
                        HeadphonesFormFactor = result.HeadphonesFormFactor,
                        ModelName = result.ModelName,
                        CategoryName = result.CategoryName,
                        RelatedHeadPhones = result.RelatedHeadPhones
                        .Select(h => new HeadPhoneViewModel
                        {
                            Id = h.Id,
                            Name = h.Name,
                            Rate = h.Rate,
                            Price = h.Price,
                            NewPrice = h.NewPrice,
                            imageSrc = h.imageSrc,
                            ConnectivityTechnology = h.ConnectivityTechnology,
                            Color = h.Color,
                            NoiseControl = h.NoiseControl,
                            HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                            HeadphonesFormFactor = h.HeadphonesFormFactor,
                            ModelName = h.ModelName,
                            isLiked = h.isLiked,
                            CategoryName = h.CategoryName,
                            RateCount = h.RateCount
                        }),
                        SimilarPriceHeadPhones = result.SimilarPriceHeadPhones
                         .Select(h => new HeadPhoneViewModel
                         {
                             Id = h.Id,
                             Name = h.Name,
                             Rate = h.Rate,
                             Price = h.Price,
                             NewPrice = h.NewPrice,
                             imageSrc = h.imageSrc,
                             ConnectivityTechnology = h.ConnectivityTechnology,
                             Color = h.Color,
                             NoiseControl = h.NoiseControl,
                             HeadphonesEarPlacement = h.HeadphonesEarPlacement,
                             HeadphonesFormFactor = h.HeadphonesFormFactor,
                             ModelName = h.ModelName,
                             isLiked = h.isLiked,
                             CategoryName = h.CategoryName,
                             RateCount = h.RateCount
                         }),
                        Comments = result.Comments,
                        Offers = result.Offers,
                        BOGOGet = result.BOGOGet,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount,
                        ControllerName = result.ControllerName,
                        TotalQuantity = result.TotalQuantity
                    };

                    return View(headPhones);
                }
                else
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllHeadPhoneComments(int id)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (id != null && id != 0)
            {
                var result = await _headPhone.GetHeadPhoneAllComments(id);

                if (result is not null)
                {
                    var headPhone = new HeadPhoneViewModel
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Rate = result.Rate,
                        CategoryName = result.CategoryName,
                        Comments = result.Comments,
                        StarCounts = result.StarCounts,
                        RateCount = result.RateCount
                    };

                    return View("AllComments", headPhone);
                }
                else
                    return RedirectToAction("Details", id);
            }
            return RedirectToAction("Details", id);
        }
    }
}