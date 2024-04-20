using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface IElectricalDevicesRepository : IRepository<ElectricalDevice>
    {
        IEnumerable<ElectricalDevice> GetByPrice();
        IEnumerable<ElectricalDevice> GetByDate();
    }
}
