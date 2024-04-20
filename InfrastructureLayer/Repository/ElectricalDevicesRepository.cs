using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repository
{
    public class ElectricalDevicesRepository : Repository<ElectricalDevice>, IElectricalDevicesRepository
    {
        private readonly AppDbContext _context;

        public ElectricalDevicesRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<ElectricalDevice> GetByDate()
        {
            return _context.ElectricalDevices.OrderByDescending(x => x.AddedOn).ToList();
        }

        public IEnumerable<ElectricalDevice> GetByPrice()
        {
            return _context.ElectricalDevices.OrderByDescending(x => x.Price).ToList();
        }
    }
}
