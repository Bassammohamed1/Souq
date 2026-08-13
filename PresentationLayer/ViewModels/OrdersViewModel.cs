namespace PresentationLayer.ViewModels
{
    public class OrdersViewModel
    {
        public IEnumerable<OrderViewModel> Orders { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
