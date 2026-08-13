using DomainLayer.Models;

namespace ApplicationLayer.DTOs
{
    public class HomePageDTO
    {
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<Item> Latest { get; set; }
        public IEnumerable<Item> Featured { get; set; }
        public IEnumerable<OfferDTO> Offers { get; set; }
    }
}
