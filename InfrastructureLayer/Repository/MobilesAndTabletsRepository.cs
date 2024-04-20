using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repository
{
    public class MobilesAndTabletsRepository : Repository<MobileAndTablet>, IMobilesAndTabletsRepository
    {
        private readonly AppDbContext _context;

        public MobilesAndTabletsRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<MobileAndTablet> GetByDate()
        {
            return _context.MobilesAndTablets.OrderByDescending(x => x.AddedOn).ToList();
        }

        public IEnumerable<MobileAndTablet> GetByPrice()
        {
            return _context.MobilesAndTablets.OrderByDescending(x => x.Price).ToList();
        }
    }
}
