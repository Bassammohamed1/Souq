using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repository
{
    public class ComputerAccessoriesRepository : Repository<Item>, IComputerAccessoriesRepository
    {
        private readonly AppDbContext _context;

        public ComputerAccessoriesRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Item> GetAllItems()
        {
            return _context.Items.Where(x => x.DepartmentID == 1).ToList();
        }

        public IEnumerable<Item> GetByDate()
        {
            return _context.Items.Where(x => x.DepartmentID == 1).OrderByDescending(x => x.AddedOn).ToList();
        }

        public IEnumerable<Item> GetByPrice()
        {
            return _context.Items.Where(x => x.DepartmentID == 1).OrderByDescending(x => x.Price).ToList();
        }
    }
}
