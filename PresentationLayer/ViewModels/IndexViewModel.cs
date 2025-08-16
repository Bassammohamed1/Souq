using DomainLayer.Models;

namespace PresentationLayer.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IQueryable<Offer> Offers { get; set; }
    }
}
