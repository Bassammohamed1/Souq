namespace DomainLayer.Models
{
    public class Fridge : Item
    {
        public string? ItemDimensions { get; set; }
        public string? Capacity { get; set; }
        public int? EnergyStar  { get; set; }
        public int? NumberOfDoors { get; set; }
        public string? Color { get; set; }
        public string? SpecialFeatures  { get; set; }
        public string? InstallationType   { get; set; }
        public string? DefrostSystem   { get; set; }
    }
}
