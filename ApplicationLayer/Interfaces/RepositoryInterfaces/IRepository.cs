using X.PagedList;

namespace DomainLayer.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetById(int id);
        Task<T> GetByName(string name);
        Task<IEnumerable<T>> GetAll();
        IQueryable<T> GetAllByIDs(IEnumerable<int> ids);
        IQueryable<T> GetAllByCategoryIDs(IEnumerable<int> ids);
        IPagedList<T> GetAll(int pageNumber, int pageSize);
        Task<T> Add(T entity);
        T Update(T entity);
        T Delete(T entity);
    }
}