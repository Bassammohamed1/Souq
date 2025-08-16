namespace PresentationLayer.ViewModels.ItemVMs
{
    public class ItemsViewModel
    {
        public IEnumerable<dynamic> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string ActionName { get; set; }
        public string? OrderIndex { get; set; }
        public bool? Des { get; set; }
        public int? Price1 { get; set; }
        public int? Price2 { get; set; }
        public int? Storage { get; set; }
        public string? Brand { get; set; }
    }
}
