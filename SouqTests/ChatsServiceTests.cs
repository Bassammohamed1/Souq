using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using DomainLayer.Models.Chat;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace SouqTests
{
    public class ChatsServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly ChatsService _service;

        public ChatsServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userManager = A.Fake<UserManager<AppUser>>();

            _service = new ChatsService(_userManager, _unitOfWork);
        }

        [Fact]
        public async Task GetChatMessages_ShouldReturnMessages()
        {
            // Arrange
            var messages = new List<ChatMessage>
    {
        new ChatMessage
        {
            SenderId = "user1",
            ReceiverId = "admin",
            MessageDate = DateTime.Now.AddMinutes(-2)
        },
        new ChatMessage
        {
            SenderId = "admin",
            ReceiverId = "user1",
            MessageDate = DateTime.Now
        }
    };

            A.CallTo(() => _unitOfWork.Chats.GetAll())
                .Returns(messages);

            // Act
            var result = await _service.GetChatMessages("user1", "admin");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetChatMessages_ShouldReturnEmpty_WhenNoMessages()
        {
            // Arrange
            A.CallTo(() => _unitOfWork.Chats.GetAll())
                .Returns(new List<ChatMessage>());

            // Act
            var result = await _service.GetChatMessages("user1", "admin");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetChatMessages_ShouldReturnMessagesInBothDirections()
        {
            // Arrange
            var messages = new List<ChatMessage>
    {
        new ChatMessage
        {
            SenderId = "user1",
            ReceiverId = "admin"
        },
        new ChatMessage
        {
            SenderId = "admin",
            ReceiverId = "user1"
        },
        new ChatMessage
        {
            SenderId = "otherUser",
            ReceiverId = "admin"
        }
    };

            A.CallTo(() => _unitOfWork.Chats.GetAll())
                .Returns(messages);

            // Act
            var result = await _service.GetChatMessages("user1", "admin");

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAdminChats_ShouldReturnUsersWithUnreadCounts()
        {
            // Arrange
            var currentUserId = "admin";

            var messages = new List<ChatMessage>
    {
        new ChatMessage
        {
            SenderId = "user1",
            ReceiverId = "admin",
            IsRead = false
        },
        new ChatMessage
        {
            SenderId = "user1",
            ReceiverId = "admin",
            IsRead = false
        },
        new ChatMessage
        {
            SenderId = "user1",
            ReceiverId = "admin",
            IsRead = true
        },
        new ChatMessage
        {
            SenderId = "user2",
            ReceiverId = "admin",
            IsRead = false
        }
    };

            A.CallTo(() => _unitOfWork.Chats.GetAll())
                .Returns(messages);

            var user1 = new AppUser { Id = "user1" };
            var user2 = new AppUser { Id = "user2" };

            A.CallTo(() => _userManager.FindByIdAsync("user1"))
                .Returns(user1);

            A.CallTo(() => _userManager.FindByIdAsync("user2"))
                .Returns(user2);

            // Act
            var result = await _service.GetAdminChats(currentUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Users.Count());

            var user1Chat = result.Users.First(x => x.User.Id == "user1");
            var user2Chat = result.Users.First(x => x.User.Id == "user2");

            Assert.Equal(2, user1Chat.ChatCount);
            Assert.Equal(1, user2Chat.ChatCount);
        }

        [Fact]
        public async Task GetAdminChats_ShouldReturnEmpty_WhenNoChatsExist()
        {
            // Arrange
            A.CallTo(() => _unitOfWork.Chats.GetAll())
                .Returns(new List<ChatMessage>());

            // Act
            var result = await _service.GetAdminChats("admin");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Users);
        }
    }
}
