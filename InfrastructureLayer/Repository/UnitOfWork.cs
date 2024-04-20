using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Laptops = new LaptopsRepository(_context);
            ElectricalDevices = new ElectricalDevicesRepository(_context);
            MobilesAndTablets = new MobilesAndTabletsRepository(_context);
            ComputerAccessories = new ComputerAccessoriesRepository(_context);
            Orders = new OrdersRepository(_context);
            Items = new Repository<BaseModel>(_context);
        }

        public ILaptopsRepository Laptops { get; private set; }

        public IElectricalDevicesRepository ElectricalDevices { get; private set; }

        public IMobilesAndTabletsRepository MobilesAndTablets { get; private set; }

        public IComputerAccessoriesRepository ComputerAccessories { get; private set; }

        public IOrdersRepository Orders { get; private set; }

        public IRepository<BaseModel> Items { get; private set; }
        public void Dispose()
        {
            _context.Dispose();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
