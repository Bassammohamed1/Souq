using DomainLayer.Models;

namespace PresentationLayer.ViewModels.ItemVMs
{
    public class ItemViewModel <T>
    {
        public IEnumerable<Category> ItemCategories { get; set; }
        public IEnumerable<T> DiscountedItems { get; set; }
        public IEnumerable<T> TopRatedItems { get; set; }
        public IEnumerable<T> latestItems { get; set; }
        public IQueryable<Offer> Offers { get; set; }
    }
}
