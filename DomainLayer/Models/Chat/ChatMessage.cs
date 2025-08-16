namespace DomainLayer.Models.Chat
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string MessageContent { get; set; }
        public DateTime MessageDate { get; set; }
        public bool IsRead { get; set; }
    }
}
