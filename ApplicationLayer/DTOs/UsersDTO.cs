
namespace ApplicationLayer.DTOs
{
    public class UsersDTO
    {
        public IEnumerable<UserDTO> Users { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
