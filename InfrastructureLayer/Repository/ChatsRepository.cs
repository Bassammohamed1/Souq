using DomainLayer.Interfaces;
using DomainLayer.Models.Chat;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repository
{
    public class ChatsRepository : IChatsRepository
    {
        private readonly AppDbContext _context;

        public ChatsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<ChatMessage>> GetChatMessages(string senderID, string receiverID)
        {
            var chatMessages = _context.ChatMessages
                .Where(cm => cm.SenderId == senderID && cm.ReceiverId == receiverID
                || cm.SenderId == receiverID && cm.ReceiverId == senderID)
                .OrderBy(cm => cm.MessageDate);

            return await chatMessages.AnyAsync() ? chatMessages : Enumerable.Empty<ChatMessage>().AsQueryable();
        }
    }
}
