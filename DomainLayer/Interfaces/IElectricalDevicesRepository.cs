using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface IElectricalDevicesRepository : IRepository<Item>
    {
        IEnumerable<Item> GetByPrice();
        IEnumerable<Item> GetByDate();
        IEnumerable<Item> GetAllItems();
    }
}
