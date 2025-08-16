using DomainLayer.Models.Chat;

namespace DomainLayer.Interfaces
{
    public interface IChatsRepository
    {
        Task<IQueryable<ChatMessage>> GetChatMessages(string senderID, string receiverID); 
    }
}
