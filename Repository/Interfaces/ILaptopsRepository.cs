using Souq.Models;

namespace Souq.Repository.Interfaces
{
    public interface ILaptopsRepository : IRepository<Laptop>
    {
        IEnumerable<Laptop> GetByPrice();
        IEnumerable<Laptop> GetByDate();
    }
}
