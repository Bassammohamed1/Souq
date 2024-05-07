using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface ILaptopsRepository : IRepository<Item>
    {
        IEnumerable<Item> GetByPrice();
        IEnumerable<Item> GetByDate();
        IEnumerable<Item> GetAllItems();
    }
}
