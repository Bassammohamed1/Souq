using DomainLayer.Models;
using X.PagedList;


namespace DomainLayer.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetById(int id);
        Task<IPagedList<T>> GetAll(int pageNumber, int pageSize);
        Task<IEnumerable<T>> GetAllWithoutPagination();
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task<IQueryable<Category>> GetItemCategories();
        Task<IQueryable<Category>> GetItemsCategories(string key);
        Task<IQueryable<T>> GetLatestItems(int pageNumber, int pageSize, string orderKey, bool desOrder);
        Task<IQueryable<T>> GetDiscountedItems(int pageNumber, int pageSize, string orderKey, bool desOrder);
        Task<IQueryable<T>> GetTopRatedItems(int pageNumber, int pageSize, string orderKey, bool desOrder);
        Task<IQueryable<T>> GetItemsFilteredByPrice(int price1, int price2, int pageNumber, int pageSize, string orderKey, bool desOrder);
        Task<IQueryable<T>> GetCategoryItems(string name, int pageNumber, int pageSize, string orderKey, bool desOrder);
        Task<int> TotalItems(string Key, int? price1 = null, int? price2 = null, string? categoryName = null);
        Task<IQueryable<Comment>> GetItemComments(int id, string type, string key);
        Task<IQueryable<Rate>> GetItemRates(int id, string type);
        Task<bool> SetRate(Rate data);
        Task<int[]> GetItemRateDetails(int id, string type);
    }
}