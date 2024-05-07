using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace InfrastructureLayer.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UnitOfWork(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            Items = new ItemsRepository(_context);
            Departments = new Repository<Department>(_context);
            Laptops = new LaptopsRepository(_context);
            ElectricalDevices = new ElectricalDevicesRepository(_context);
            MobilesAndTablets = new MobilesAndTabletsRepository(_context);
            ComputerAccessories = new ComputerAccessoriesRepository(_context);
            Carts = new CartRepository(_context, _httpContextAccessor, _userManager);
            Orders = new OrdersRepository(_context);
        }
        public ILaptopsRepository Laptops { get; private set; }
        public IElectricalDevicesRepository ElectricalDevices { get; private set; }
        public IMobilesAndTabletsRepository MobilesAndTablets { get; private set; }
        public IComputerAccessoriesRepository ComputerAccessories { get; private set; }
        public IItemsRepository Items { get; private set; }
        public IRepository<Department> Departments { get; private set; }
        public ICartRepository Carts { get; private set; }
        public IOrdersRepository Orders { get; private set; }
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
