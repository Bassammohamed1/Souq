using DomainLayer.Models;

namespace ApplicationLayer.DTOs
{
    public class FilterDTO
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string OrderIndex { get; set; }
        public string SearchPhrase { get; set; }
        public IEnumerable<Item>? MatchedItems { get; set; }
    }
}
