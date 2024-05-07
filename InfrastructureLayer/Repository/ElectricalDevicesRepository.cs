using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repository
{
    public class ElectricalDevicesRepository : Repository<Item>, IElectricalDevicesRepository
    {
        private readonly AppDbContext _context;

        public ElectricalDevicesRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Item> GetAllItems()
        {
            return _context.Items.Where(x => x.DepartmentID == 2).ToList();
        }

        public IEnumerable<Item> GetByDate()
        {
            return _context.Items.Where(x => x.DepartmentID == 2).OrderByDescending(x => x.AddedOn).ToList();
        }

        public IEnumerable<Item> GetByPrice()
        {
            return _context.Items.Where(x => x.DepartmentID == 2).OrderByDescending(x => x.Price).ToList();
        }
    }
}
