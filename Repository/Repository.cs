using Souq.Models;
using Souq.Repository.Interfaces;

namespace Souq.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        public Repository(AppDbContext context)
        {
            _context = context;
        }
        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }
        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }
        public void Add(T data)
        {
            _context.Set<T>().Add(data);
        }
        public void Update(T data)
        {
            _context.Set<T>().Update(data);
        }
        public void Delete(T data)
        {
            _context.Set<T>().Remove(data);
        }
    }
}
