using ApplicationLayer.DTOs;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IItemsService
    {
        Task<Item> GetItem(int ID);
        Task<IEnumerable<Item>> GetItems(int pageNumber, int pageSize);
        Task<IEnumerable<Item>> GetFilteredItems(List<string> selectedFilters, int pageNumber, int pageSize);
        Task<ProductsDTO> GetAllItemsWithSort(int? page, string? orderIndex, bool? des);
        Task<ProductsDTO> GetAllItemsWithSortAndFilter(ProductsDTO? data, int? page, string? filters, string? orderIndex, bool? des);
        Task<ProductsDTO> GetAllItemsWithFilter(ProductsDTO data, string? orderIndex, bool? des);
        IEnumerable<Category> GetItemCategories<T>() where T : Item;
        Task<IEnumerable<Category>> GetItemsCategories(string key);
        IQueryable<T> GetLatestItems<T>(int pageNumber, int pageSize, string orderKey, bool desOrder) where T : Item;
        IQueryable<T> GetDiscountedItems<T>(int pageNumber, int pageSize, string orderKey, bool desOrder) where T : Item;
        IQueryable<T> GetTopRatedItems<T>(int pageNumber, int pageSize, string orderKey, bool desOrder) where T : Item;
        IQueryable<T> GetItemsFilteredByPrice<T>(int price1, int price2, int pageNumber, int pageSize, string orderKey, bool desOrder) where T : Item;
        IQueryable<T> GetCategoryItems<T>(string name, int pageNumber, int pageSize, string orderKey, bool desOrder) where T : Item;
        Task<int> TotalItems<T>(string Key, int? price1 = null, int? price2 = null, string? categoryName = null) where T : Item;
        Task<IEnumerable<Comment>> GetItemComments(int id, string type, string key);
        Task<IEnumerable<Rate>> GetItemRates(int id, string type);
        Task<bool> SetRate(Rate data);
        Task<int[]> GetItemRateDetails<T>(int id, string type) where T : Item;
        IEnumerable<Item> SortItems(IEnumerable<Item> items, string key, bool des);
    }
}
