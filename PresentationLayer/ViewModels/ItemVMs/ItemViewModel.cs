using DomainLayer.Models;

namespace PresentationLayer.ViewModels.ItemVMs
{
    public class ItemViewModel <T>
    {
        public List<Category> ItemCategories { get; set; }
        public List<T> DiscountedItems { get; set; }
        public List<T> TopRatedItems { get; set; }
        public List<T> latestItems { get; set; }
        public IQueryable<Offer> Offers { get; set; }
    }
}
