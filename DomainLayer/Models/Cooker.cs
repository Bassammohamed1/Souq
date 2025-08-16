namespace DomainLayer.Models
{
    public class Cooker : Item
    {
        public string? ItemDimensions { get; set; }
        public string? DrawerType { get; set; }
        public string? Material { get; set; }
        public string? FinishType { get; set; }
        public string? ModelName { get; set; }
        public string? FormFactor { get; set; }
        public string? ControlsType { get; set; }
        public string? SpecialFeatures { get; set; }
        public string? Color { get; set; }
        public string? ItemWeight { get; set; }
        public int? NumberOfHeatingElements { get; set; }
    }
}
