namespace DomainLayer.Models
{
    public class WashingMachine : Item
    {
        public string? ItemDimensions { get; set; }
        public string? CycleOptions { get; set; }
        public string? ItemWeight { get; set; }
        public string? Capacity { get; set; }
        public string? SpecialFeatures { get; set; }
        public string? Color { get; set; }
    }
}
