using DomainLayer.Models;

namespace ApplicationLayer.DTOs
{
    public class ProductsDTO
    {
        public IEnumerable<Item> Items { get; set; }
        public List<string> SelectedFilters { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? OrderIndex { get; set; }
        public bool? Des { get; set; }
    }
}
