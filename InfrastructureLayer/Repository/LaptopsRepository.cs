using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repository
{
    public class LaptopsRepository : Repository<Laptop>, ILaptopsRepository
    {
        private readonly AppDbContext _context;

        public LaptopsRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Laptop> GetByDate()
        {
            return _context.Laptops.OrderByDescending(x => x.AddedOn).ToList();
        }

        public IEnumerable<Laptop> GetByPrice()
        {
            return _context.Laptops.OrderByDescending(x => x.Price).ToList();
        }
    }
}
