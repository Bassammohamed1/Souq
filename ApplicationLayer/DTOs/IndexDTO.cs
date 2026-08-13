using DomainLayer.Models;

namespace ApplicationLayer.DTOs
{
    public class IndexDTO
    {
        public IEnumerable<Category> Categories { get; set; }
        public IQueryable<Offer> Offers { get; set; }
    }
}
