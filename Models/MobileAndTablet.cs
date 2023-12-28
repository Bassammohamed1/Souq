using Souq.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Souq.Models
{
    public class MobileAndTablet : BaseModel
    {
        [Required]
        public MobilesAndTabletsCategory Category { get; set; }
    }
}
