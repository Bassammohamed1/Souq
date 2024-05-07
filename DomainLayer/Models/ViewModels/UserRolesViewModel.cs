namespace DomainLayer.Models.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public List<RolesViewModel> Roles { get; set; }
    }
}