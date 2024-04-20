using DomainLayer.Enums;
using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Models
{
    public class MobileAndTablet : BaseModel
    {
        [Required]
        public MobilesAndTabletsCategory Category { get; set; }
    }
}
