using DomainLayer.Interfaces;
using DomainLayer.Models.Chat;
using InfrastructureLayer.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Helpers
{
    public class ChatHub : Hub
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _context;
        private static string _userID = string.Empty;
        private string _currentUserID;

        public ChatHub(IUserService userService, AppDbContext context)
        {
            _userService = userService;
            _context = context;
            _currentUserID = _userService.GetUserId().Result;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            var connectionId = Context.ConnectionId;

            await _context.UserConnections.AddAsync(new UserConnection { UserId = userId, ConnectionId = connectionId });
            await _context.SaveChangesAsync();

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connection = await _context.UserConnections.FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);
            if (connection != null)
            {
                _context.UserConnections.Remove(connection);
                await _context.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageFromAdmin(string receiverID, string message)
        {
            var receiver = await _context.UserConnections
                .Where(uc => uc.UserId == receiverID).FirstOrDefaultAsync();

            bool isOnline = receiver is not null;

            var msg = new ChatMessage()
            {
                SenderId = _currentUserID,
                ReceiverId = receiverID,
                MessageContent = message,
                MessageDate = DateTime.Now,
                IsRead = isOnline,

            };
            await _context.ChatMessages.AddAsync(msg);
            await _context.SaveChangesAsync();

            await Clients.Caller.SendAsync("YourMessage", message, isOnline);
            await Clients.User(receiverID).SendAsync("NewMessage", message);
        }

        public async Task SendMessageFromUser(string receiverID, string message)
        {
            var isInChat = _userID == _currentUserID;

            var msg = new ChatMessage()
            {
                SenderId = _currentUserID,
                ReceiverId = receiverID,
                MessageContent = message,
                MessageDate = DateTime.Now,
                IsRead = isInChat,

            };
            await _context.ChatMessages.AddAsync(msg);
            await _context.SaveChangesAsync();

            await Clients.Caller.SendAsync("YourMessage", message, isInChat);
            await Clients.User(receiverID).SendAsync("NewMessage", message);
        }

        public Task CurrentChatUserID(string userID)
        {
            _userID = userID;

            return Task.CompletedTask;
        }

        public async Task MarkMessagesAsRead(string senderID)
        {
            var rowsUpdated = await _context.ChatMessages
                  .Where(cm => cm.SenderId == senderID && cm.ReceiverId == _currentUserID)
                  .ExecuteUpdateAsync(x => x.SetProperty(m => m.IsRead, true));

            if (rowsUpdated > 0)
            {
                await Clients.User(senderID).SendAsync("MessagesRead");
            }
        }
    }
}