using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IItemsRepository Items { get; }
        IRepository<Department> Departments { get; }
        ILaptopsRepository Laptops { get; }
        IElectricalDevicesRepository ElectricalDevices { get; }
        IMobilesAndTabletsRepository MobilesAndTablets { get; }
        IComputerAccessoriesRepository ComputerAccessories { get; }
        ICartRepository Carts { get; }
        IOrdersRepository Orders { get; }
        void SaveChanges();
    }
}
