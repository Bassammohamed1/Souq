using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "User")]
    public class UserInteractionsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService userService;

        public UserInteractionsController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            this.userService = userService;
        }

        public async Task<IActionResult> Comment(int itemId, string itemType)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (itemId != null && !string.IsNullOrEmpty(itemType))
            {
                var userId = await userService.GetUserId();

                var comment = new Comment()
                {
                    ItemId = itemId,
                    ItemType = itemType,
                    CommentTime = DateTime.Now.AddMinutes(1),
                    UserId = userId
                };

                return View(comment);
            }
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Comment(Comment comment)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (ModelState.IsValid)
            {
                await _unitOfWork.Comments.Add(comment);
                await _unitOfWork.Commit();

                return RedirectToAction("Details", comment.ItemType, new { id = comment.ItemId });
            }
            return View(comment);
        }

        public async Task<IActionResult> Rate(int itemId, string itemType)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (itemId != null && !string.IsNullOrEmpty(itemType))
            {
                var userId = await userService.GetUserId();

                var rate = new Rate()
                {
                    ItemId = itemId,
                    ItemType = itemType,
                    UserId = userId,
                };

                return View(rate);
            }
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Rate(Rate rate)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;


            if (ModelState.IsValid)
            {
                var rates = _unitOfWork.Rates.GetAllWithoutPagination().Result.Where(r => r.UserId == rate.UserId && r.ItemId == rate.ItemId && r.ItemType == rate.ItemType);

                if (rates.Any())
                {
                    await _unitOfWork.Rates.Delete(rates.First());
                    await _unitOfWork.Commit();
                }

                await _unitOfWork.Rates.Add(rate);
                await _unitOfWork.Commit();

                var propInfo = _unitOfWork.GetType()
                  .GetProperty(rate.ItemType, BindingFlags.Public | BindingFlags.Instance);

                if (propInfo == null)
                    throw new ArgumentException($"No repository named '{rate.ItemType}'", nameof(rate.ItemType));

                dynamic model = propInfo.GetValue(_unitOfWork);

                var result = await model.SetRate(rate);

                return result ? RedirectToAction("Details", rate.ItemType, new { id = rate.ItemId }) : throw new InvalidOperationException("Failed to set rate.");
            }
            return View();
        }

        public async Task<IActionResult> AddItemToWishList(int itemId, string itemType)
        {
            var wishCount = await _unitOfWork.WishLists.Add(itemId, itemType);
            return RedirectToAction("GetUserWishingList");
        }

        public async Task<IActionResult> RemoveItemFromWishList(int itemId, string itemType)
        {
            var wishCount = await _unitOfWork.WishLists.Remove(itemId, itemType);
            return RedirectToAction("GetUserWishingList");
        }

        public async Task<IActionResult> GetUserWishingList(int? page)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            int pageNumber = page ?? 1;
            int pageSize = 10;

            var wishList = await _unitOfWork.WishLists.UserWishingList(pageNumber, pageSize);

            foreach (var wish in wishList)
            {
                wish.Quantity = await _unitOfWork.Carts.TotalItemQuantityInCart(wish.ItemId, wish.ItemType);
                await _unitOfWork.Commit();
            }

            return View("WishingList", wishList);
        }

        public async Task<IActionResult> GetTotalItemInWishingList()
        {
            int totalItems = await _unitOfWork.WishLists.TotalItemsInWishingList();
            return Ok(totalItems);
        }
    }
}
