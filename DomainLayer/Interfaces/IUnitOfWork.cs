using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ILaptopsRepository Laptops { get; }
        IElectricalDevicesRepository ElectricalDevices { get; }
        IMobilesAndTabletsRepository MobilesAndTablets { get; }
        IComputerAccessoriesRepository ComputerAccessories { get; }
        IOrdersRepository Orders { get; }
        IRepository<BaseModel> Items { get; }
        void SaveChanges();
    }
}
