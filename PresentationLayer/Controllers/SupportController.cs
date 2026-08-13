using ApplicationLayer.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels.Chat;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "User")]
    public class SupportController : Controller
    {
        private readonly IUsersService _users;
        private readonly IChatsService _chats;
        private readonly IDepartmentsService _departments;
        private string _currentUserID;

        public SupportController(IDepartmentsService departments, IChatsService chats, IUsersService users)
        {
            _users = users;
            _chats = chats;
            _departments = departments;
            _currentUserID = _users.GetUserId();

        }

        private async Task GenerateCurrentUserAndAdminIDsViewBags()
        {
            var currentUserID = _users.GetUserId();

            var adminID = _users.GetUsers()
                .FirstOrDefault(u => u.Name == "Bassam").ID;

            ViewBag.AdminID = adminID;
            ViewBag.CurrentUserID = currentUserID;
        }

        public async Task<IActionResult> UserChat()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            await GenerateCurrentUserAndAdminIDsViewBags();

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminChat()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            await GenerateCurrentUserAndAdminIDsViewBags();

            var result = await _chats.GetAdminChats(_currentUserID);

            return View(new AdminChatViewModel()
            {
                Users = result.Users
                .Select(u => new UsersChatsCountViewModel
                {
                    User = u.User,
                    ChatCount = u.ChatCount
                })
            });
        }

        public async Task<IActionResult> LoadChatMessages(string senderID, string receiverID)
        {
            await GenerateCurrentUserAndAdminIDsViewBags();

            var chat = await _chats.GetChatMessages(senderID, receiverID);

            var chatVM = new ChatMessagesViewModel()
            {
                ReadMessages = chat.Where(c => c.IsRead || c.SenderId == _currentUserID),
                UnReadMessages = chat.Where(c => c.ReceiverId == _currentUserID && !c.IsRead)
            };

            return PartialView("_ChatMessagesPartial", chatVM);
        }
    }
}