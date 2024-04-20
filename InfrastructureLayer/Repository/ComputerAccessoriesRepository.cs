using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repository
{
    public class ComputerAccessoriesRepository : Repository<ComputerAccessory>, IComputerAccessoriesRepository
    {
        private readonly AppDbContext _context;

        public ComputerAccessoriesRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<ComputerAccessory> GetByDate()
        {
            return _context.ComputerAccessories.OrderByDescending(x => x.AddedOn).ToList();
        }

        public IEnumerable<ComputerAccessory> GetByPrice()
        {
            return _context.ComputerAccessories.OrderByDescending(x => x.Price).ToList();
        }
    }
}
