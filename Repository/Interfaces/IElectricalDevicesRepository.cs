using Souq.Models;

namespace Souq.Repository.Interfaces
{
    public interface IElectricalDevicesRepository : IRepository<ElectricalDevice>
    {
        IEnumerable<ElectricalDevice> GetByPrice();
        IEnumerable<ElectricalDevice> GetByDate();
    }
}
