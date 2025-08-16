using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface IMobilePhonesRepository : IRepository<MobilePhone>
    {
        Task<IQueryable<MobilePhone>> GetPhonesFilteredByStorage(int storage, int pageNumber, int pageSize, string orderKey, bool desOrder);
        Task<int> TotalFilterStoragePhones(int Storage);
    }
}
