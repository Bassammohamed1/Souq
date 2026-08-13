
namespace ApplicationLayer.DTOs
{
    public class OrdersDTO
    {
        public IEnumerable<OrderDTO> Orders { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
