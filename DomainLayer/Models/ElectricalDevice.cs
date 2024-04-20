using DomainLayer.Enums;
using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Models
{
    public class ElectricalDevice : BaseModel
    {
        [Required]
        public ElectricalDevicesCategory Category { get; set; }
    }
}
