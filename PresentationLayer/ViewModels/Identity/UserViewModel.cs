namespace PresentationLayer.ViewModels.Identity
{
    public class UserViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
