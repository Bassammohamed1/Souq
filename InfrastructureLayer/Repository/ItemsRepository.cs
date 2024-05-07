using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repository
{
    public class ItemsRepository : Repository<Item>, IItemsRepository
    {
        private readonly AppDbContext _context;

        public ItemsRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public List<Item> GetAllItems()
        {
            return _context.Items.Include(x => x.Department).ToList();
        }
    }
}
