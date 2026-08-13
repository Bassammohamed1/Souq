using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels.Wishing_List;
using X.PagedList.Extensions;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "User")]
    public class UserInteractionsController : Controller
    {
        private readonly IWishingListService _wishingList;
        private readonly IDepartmentsService _departments;
        private readonly IUserInteractionsService _userInteractions;
        private readonly IUsersService _userService;

        public UserInteractionsController(IUsersService userService, IWishingListService wishingList, IDepartmentsService departments, IUserInteractionsService userInteractions)
        {
            _userService = userService;
            _wishingList = wishingList;
            _departments = departments;
            _userInteractions = userInteractions;
        }

        public async Task<IActionResult> Comment(int itemId, string itemType)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (itemId != null && !string.IsNullOrEmpty(itemType))
            {
                var userId = _userService.GetUserId();

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
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (ModelState.IsValid)
            {
                var result = await _userInteractions.AddComment(comment);

                return result.Success ? RedirectToAction("Details", comment.ItemType, new { id = comment.ItemId }) :
                     View(comment);
            }
            return View(comment);
        }

        public async Task<IActionResult> Rate(int itemId, string itemType)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (itemId != null && !string.IsNullOrEmpty(itemType))
            {
                var userId = _userService.GetUserId();

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
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;


            if (ModelState.IsValid)
            {
                var result = await _userInteractions.AddRate(rate);

                return result.Success ? RedirectToAction("Details", rate.ItemType, new { id = rate.ItemId }) :
                    throw new InvalidOperationException("Failed to set rate.");
            }

            return View();
        }

        public async Task<IActionResult> AddItemToWishList(int itemId, string itemType)
        {
            var wishCount = await _wishingList.Add(itemId, itemType);

            return RedirectToAction("GetUserWishingList");
        }

        public async Task<IActionResult> RemoveItemFromWishList(int itemId, string itemType)
        {
            var wishCount = await _wishingList.Remove(itemId, itemType);

            return RedirectToAction("GetUserWishingList");
        }

        public async Task<IActionResult> GetUserWishingList(int? page)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _wishingList.UserWishingList(page);

            var wishingListVM = result.Select(wl => new WishingListViewModel
            {
                ItemId = wl.ItemId,
                Quantity = wl.Quantity,
                Name = wl.Name,
                Price = wl.Price,
                ItemType = wl.ItemType,
                imageSrc = wl.imageSrc
            }).ToPagedList();

            return View("WishingList", wishingListVM);
        }

        public async Task<IActionResult> GetTotalItemInWishingList()
        {
            int totalItems = await _wishingList.TotalItemsInWishingList();

            return Ok(totalItems);
        }
    }
}
