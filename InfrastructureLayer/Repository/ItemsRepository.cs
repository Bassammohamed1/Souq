using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace InfrastructureLayer.Repository
{
    public class ItemsRepository : Repository<Item>, IItemsRepository
    {
        private readonly AppDbContext _context;

        public ItemsRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Item> FindItemByID(int ID)
        {
            var items = await this.GetAllItems(1, int.MaxValue);

            var item = items.FirstOrDefault(i => i.ID == ID);

            return item;
        }

        public async Task<IEnumerable<Item>> GetAllItems(int pageNumber, int pageSize)
        {
            var items = new List<Item>();

            var airConditioners = await _context.AirConditioners.AsNoTracking().AsSplitQuery()
                .Include(a => a.Category).ToListAsync();

            items.AddRange(airConditioners);

            var cookers = await _context.Cookers.AsNoTracking().AsSplitQuery()
                .Include(c => c.Category).ToListAsync();

            items.AddRange(cookers);

            var fridges = await _context.Fridges.AsNoTracking().AsSplitQuery()
                .Include(f => f.Category).ToListAsync();

            items.AddRange(fridges);

            var washingMachines = await _context.WashingMachines.AsNoTracking().AsSplitQuery()
                .Include(w => w.Category).ToListAsync();

            items.AddRange(washingMachines);

            var headPhones = await _context.HeadPhones.AsNoTracking().AsSplitQuery()
                .Include(h => h.Category).ToListAsync();

            items.AddRange(headPhones);

            var laptops = await _context.Laptops.AsNoTracking().AsSplitQuery()
                .Include(l => l.Category).ToListAsync();

            items.AddRange(laptops);

            var tvs = await _context.TVs.AsNoTracking().AsSplitQuery()
                .Include(t => t.Category).ToListAsync();

            items.AddRange(tvs);

            var mobilePhones = await _context.MobilePhones.AsNoTracking().AsSplitQuery()
                .Include(p => p.Category).ToListAsync();

            items.AddRange(mobilePhones);

            var videoGames = await _context.VideoGames.AsNoTracking().AsSplitQuery()
                .Include(v => v.Category).ToListAsync();

            items.AddRange(videoGames);

            return items.Any() ? items.Skip((pageNumber - 1) * pageSize).Take(pageSize) : Enumerable.Empty<Item>();
        }

        public IQueryable<T> GetLatestItemsDesOrder<T>(int pageNumber, int pageSize, string orderKey) where T : Item
        {
            if (!string.IsNullOrEmpty(orderKey))
            {
                return _context.Set<T>().AsNoTracking().AsSplitQuery()
                     .OrderByDescending(i => EF.Property<object>(i, "AddedOn"))
                     .Include("Category")
                     .OrderByDescending(i => EF.Property<object>(i, orderKey))
                     .Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
            else
                throw new ArgumentException();
        }

        public IQueryable<T> GetLatestItemsAesOrder<T>(int pageNumber, int pageSize, string orderKey) where T : Item
        {
            if (!string.IsNullOrEmpty(orderKey))
            {
                return _context.Set<T>().AsNoTracking().AsSplitQuery()
                   .OrderByDescending(i => EF.Property<object>(i, "AddedOn"))
                   .Include("Category")
                   .OrderBy(i => EF.Property<object>(i, orderKey))
                   .Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
            else
                throw new ArgumentException();
        }

        public IEnumerable<Item> SortItems(IEnumerable<Item> items, string key, bool des)
        {
            Func<Item, object> keySelector = i => i.GetType().GetProperty(key)?.GetValue(i, null);

            IEnumerable<Item> sortedItems = des
                ? items.OrderByDescending(keySelector)
                : items.OrderBy(keySelector);

            return sortedItems;
        }

        public IQueryable<T> ItemDbSet<T>() where T : class
        {
            return _context.Set<T>().AsQueryable();
        }

        public INavigation FindNavigation<T>(string key) where T : Item
        {
            return _context.Model.FindEntityType(typeof(T))?.FindNavigation(key);
        }
    }
}