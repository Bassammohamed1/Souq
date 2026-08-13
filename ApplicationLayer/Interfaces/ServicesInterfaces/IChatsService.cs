using ApplicationLayer.DTOs;
using DomainLayer.Models.Chat;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IChatsService
    {
        Task<AdminChatDTO> GetAdminChats(string currentUserID);
        Task<IQueryable<ChatMessage>> GetChatMessages(string senderID, string receiverID);
    }
}