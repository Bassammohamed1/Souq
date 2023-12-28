using Souq.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Souq.Models
{
    public class ElectricalDevice : BaseModel
    {
        [Required]
        public ElectricalDevicesCategory Category { get; set; }
    }
}
