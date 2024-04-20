namespace DomainLayer.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T data);
        void Update(T data);
        void Delete(T data);
    }
}