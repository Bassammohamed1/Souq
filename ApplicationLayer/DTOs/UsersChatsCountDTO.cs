using DomainLayer.Models;

namespace ApplicationLayer.DTOs
{
    public class UsersChatsCountDTO
    {
        public AppUser User { get; set; }
        public int ChatCount { get; set; } = 0;
    }
}
