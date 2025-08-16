using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Models
{
    public class MobilePhone : Item
    {
        [Required]
        public string Color { get; set; }
        [Required]
        public int RAM { get; set; }
        [Required]
        public int MemoryStorageCapacity { get; set; }
        [Required]
        public string OperatingSystem { get; set; }
        [Required]
        public string CPUModel { get; set; } 
    }
}
