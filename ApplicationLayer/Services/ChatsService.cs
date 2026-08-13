using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using DomainLayer.Models.Chat;
using Microsoft.AspNetCore.Identity;

namespace ApplicationLayer.Services
{
    public class ChatsService : IChatsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public ChatsService(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminChatDTO> GetAdminChats(string currentUserID)
        {
            var usersIDs = (await _unitOfWork.Chats.GetAll())
                .DistinctBy(cm => cm.SenderId)
                .Where(cm => cm.ReceiverId == currentUserID)
                .Select(cm => cm.SenderId);

            List<UsersChatsCountDTO> usersChatCounts = new List<UsersChatsCountDTO>();

            foreach (var id in usersIDs)
            {
                var userChatVM = new UsersChatsCountDTO()
                {
                    User = await _userManager.FindByIdAsync(id),
                    ChatCount = (await _unitOfWork.Chats.GetAll())
                    .Where(cm => cm.SenderId == id && cm.ReceiverId == currentUserID && !cm.IsRead).Count()
                };

                usersChatCounts.Add(userChatVM);
            }

            return new AdminChatDTO() { Users = usersChatCounts };
        }

        public async Task<IQueryable<ChatMessage>> GetChatMessages(string senderID, string receiverID)
        {
            var chatMessages = (await _unitOfWork.Chats.GetAll())
                .Where(cm => cm.SenderId == senderID && cm.ReceiverId == receiverID
                || cm.SenderId == receiverID && cm.ReceiverId == senderID)
                .OrderBy(cm => cm.MessageDate).AsQueryable();

            return chatMessages.Any() ? chatMessages : Enumerable.Empty<ChatMessage>().AsQueryable();
        }
    }
}