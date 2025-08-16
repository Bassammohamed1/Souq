using DomainLayer.Interfaces;
using DomainLayer.Models;
using DomainLayer.Models.Chat;
using InfrastructureLayer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "User")]
    public class SupportController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private string _currentUserID;

        private async Task GenerateCurrentUserAndAdminIDsViewBags()
        {
            var currentUserID = await _userService.GetUserId();

            var adminID = await _userManager.Users.Where(u => u.UserName == "Bassam").Select(u => u.Id).FirstOrDefaultAsync();

            ViewBag.AdminID = adminID;
            ViewBag.CurrentUserID = currentUserID;
        }

        public SupportController(AppDbContext context, IUserService userService, UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _context = context;
            _userService = userService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _currentUserID = _userService.GetUserId().Result;
        }

        public async Task<IActionResult> UserChat()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            await GenerateCurrentUserAndAdminIDsViewBags();

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminChat()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            await GenerateCurrentUserAndAdminIDsViewBags();

            var usersIDs = _context.ChatMessages.ToList()
                .DistinctBy(cm => cm.SenderId)
                .Where(cm => cm.ReceiverId == _currentUserID)
                .Select(cm => cm.SenderId);

            List<UsersChatsCountViewModel> usersChatCounts = new List<UsersChatsCountViewModel>();

            foreach (var id in usersIDs)
            {
                var userChatVM = new UsersChatsCountViewModel()
                {
                    User = await _userManager.FindByIdAsync(id),
                    ChatCount = _context.ChatMessages
                    .Where(cm => cm.SenderId == id && cm.ReceiverId == _currentUserID && !cm.IsRead).Count()
                };

                usersChatCounts.Add(userChatVM);
            }

            return View(new AdminChatViewModel() { Users = usersChatCounts });
        }

        public async Task<IActionResult> LoadChatMessages(string senderID, string receiverID)
        {
            await GenerateCurrentUserAndAdminIDsViewBags();

            var chat = await _unitOfWork.Chats.GetChatMessages(senderID, receiverID);

            var chatVM = new ChatMessagesViewModel()
            {
                ReadMessages = chat.Where(c => c.IsRead || c.SenderId == _currentUserID),
                UnReadMessages = chat.Where(c => c.ReceiverId == _currentUserID && !c.IsRead)
            };

            return PartialView("_ChatMessagesPartial", chatVM);
        }
    }
}