namespace DomainLayer.Models.Chat
{
    public class UsersChatsCountViewModel
    {
        public AppUser User { get; set; }
        public int ChatCount { get; set; } = 0;
    }
}
