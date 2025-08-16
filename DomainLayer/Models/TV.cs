namespace DomainLayer.Models
{
    public class TV : Item
    {
        public double? ScreenSize { get; set; }
        public int? RefreshRate { get; set; }
        public string? Resolution { get; set; }
        public string? DisplayTechnology { get; set; }
        public string? ConnectivityTechnology  { get; set; }
        public string? ItemDimensions { get; set; }
        public string? SpecialFeatures { get; set; }
    }
}
