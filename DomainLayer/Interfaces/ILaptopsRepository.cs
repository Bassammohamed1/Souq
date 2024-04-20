using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface ILaptopsRepository : IRepository<Laptop>
    {
        IEnumerable<Laptop> GetByPrice();
        IEnumerable<Laptop> GetByDate();
    }
}
