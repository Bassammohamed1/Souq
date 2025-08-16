using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Models
{
    public class Laptop : Item
    {
        public double? ScreenSize { get; set; }
        public string? ModelName { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public int RAM { get; set; }
        [Required]
        public int HardDiskSize { get; set; }
        [Required]
        public string CPU { get; set; }
        [Required]
        public string GPU { get; set; }
        [Required]
        public string OperatingSystem { get; set; }
        public string? HardDiskDescription { get; set; }
    }
}
