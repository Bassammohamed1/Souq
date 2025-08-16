using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ViewModels.Identity
{
    public class SettingsViewModel
    {
        public string UserID { get; set; }
        [MaxLength(50)]
        public string? UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}