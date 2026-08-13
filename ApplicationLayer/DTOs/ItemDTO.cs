using DomainLayer.Models;

namespace ApplicationLayer.DTOs
{
    public class ItemDTO<T>
    {
        public IEnumerable<Category> ItemCategories { get; set; }
        public IEnumerable<T> DiscountedItems { get; set; }
        public IEnumerable<T> TopRatedItems { get; set; }
        public IEnumerable<T> latestItems { get; set; }
        public IQueryable<Offer> Offers { get; set; }
    }
}
