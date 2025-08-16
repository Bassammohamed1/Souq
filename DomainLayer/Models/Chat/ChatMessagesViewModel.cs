namespace DomainLayer.Models.Chat
{
    public class ChatMessagesViewModel
    {
        public IEnumerable<ChatMessage> ReadMessages { get; set; }
        public IEnumerable<ChatMessage>? UnReadMessages { get; set; }
    }
}