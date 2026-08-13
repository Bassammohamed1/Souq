namespace PresentationLayer.ViewModels.Identity
{
    public class UsersViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
