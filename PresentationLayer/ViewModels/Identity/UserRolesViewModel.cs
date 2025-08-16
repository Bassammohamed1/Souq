namespace PresentationLayer.ViewModels.Identity
{
    public class UserRolesViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }
}
