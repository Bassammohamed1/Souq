using DomainLayer.DTOs;
using DomainLayer.Models;

namespace PresentationLayer.ViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<Item> Latest { get; set; }
        public IEnumerable<Item> Featured { get; set; }
        public IEnumerable<OfferDTO> Offers { get; set; }
    }
}
