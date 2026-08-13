using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repository
{
    public class OffersRepository : Repository<Offer>, IOffersRepository
    {
        private readonly AppDbContext _context;

        public OffersRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Offer> GetOffersWithDepartmentName(string departmentName)
        {
            return _context.Offers.AsNoTracking()
             .Where(o => o.DepartmentName == departmentName);
        }

        public IQueryable<Offer> GetOffersWithCategoryName(string categoryName)
        {
            return _context.Offers.AsNoTracking()
                .Where(o => o.CategoryName == categoryName);
        }

        public IQueryable<Offer> GetOffersWithItemID(int itemID)
        {
            return _context.Offers.AsNoTracking()
                .Where(o => o.ItemID == itemID || o.ItemOneID == itemID);
        }
    }
}