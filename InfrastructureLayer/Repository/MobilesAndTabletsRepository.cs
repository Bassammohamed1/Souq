using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repository
{
    public class MobilesAndTabletsRepository : Repository<Item>, IMobilesAndTabletsRepository
    {
        private readonly AppDbContext _context;

        public MobilesAndTabletsRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Item> GetAllItems()
        {
            return _context.Items.Where(x => x.DepartmentID == 3).ToList();
        }
        public IEnumerable<Item> GetByDate()
        {
            return _context.Items.Where(x => x.DepartmentID == 3).OrderByDescending(x => x.AddedOn).ToList();
        }
        public IEnumerable<Item> GetByPrice()
        {
            return _context.Items.Where(x => x.DepartmentID == 3).OrderByDescending(x => x.Price).ToList();
        }
    }
}
