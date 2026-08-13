using DomainLayer.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DomainLayer.Interfaces
{
    public interface IItemsRepository : IRepository<Item>
    {
        Task<Item> FindItemByID(int ID);
        Task<IEnumerable<Item>> GetAllItems(int pageNumber, int pageSize);
        IQueryable<T> GetLatestItemsDesOrder<T>(int pageNumber, int pageSize, string orderKey) where T : Item;
        IQueryable<T> GetLatestItemsAesOrder<T>(int pageNumber, int pageSize, string orderKey) where T : Item;
        IEnumerable<Item> SortItems(IEnumerable<Item> items, string key, bool des);
        IQueryable<T> ItemDbSet<T>() where T : class;
        INavigation FindNavigation<T>(string key) where T : Item;
    }
}
