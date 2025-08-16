using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface IItemsRepository
    {
        Task<IEnumerable<Item>> GetAll(int pageNumber, int pageSize);
        Task<IEnumerable<Item>> GetFilteredItems(List<string> selectedFilters, int pageNumber, int pageSize);
        Task<IEnumerable<Item>> SortItems(IEnumerable<Item> items, string key, bool des);
        Task<Item> FindItemByID(int ID);
    }
}
