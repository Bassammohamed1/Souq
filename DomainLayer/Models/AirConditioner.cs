namespace DomainLayer.Models
{
    public class AirConditioner : Item
    {
        public string? Capacity { get; set; }
        public string? CoolingPower { get; set; }
        public double? NoiseLevel { get; set; }
        public int? Voltage { get; set; }
        public string? SpecialFeatures { get; set; }
        public string? Color { get; set; }
        public string? ItemDimensions { get; set; }
    }
}
