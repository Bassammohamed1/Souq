using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repository
{
    public class ItemsRepository : IItemsRepository
    {
        private readonly AppDbContext _context;

        public ItemsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Item> FindItemByID(int ID)
        {
            var items = await GetAll(1, int.MaxValue);

            var item = items.Where(i => i.ID == ID).FirstOrDefault();

            return item;
        }

        public async Task<IEnumerable<Item>> GetAll(int pageNumber, int pageSize)
        {
            var items = new List<Item>();

            var airConditioners = await _context.AirConditioners.Include(a => a.Category).ToListAsync();
            items.AddRange(airConditioners);

            var cookers = await _context.Cookers.Include(c => c.Category).ToListAsync();
            items.AddRange(cookers);

            var fridges = await _context.Fridges.Include(f => f.Category).ToListAsync();
            items.AddRange(fridges);

            var washingMachines = await _context.WashingMachines.Include(w => w.Category).ToListAsync();
            items.AddRange(washingMachines);

            var headPhones = await _context.HeadPhones.Include(h => h.Category).ToListAsync();
            items.AddRange(headPhones);

            var laptops = await _context.Laptops.Include(l => l.Category).ToListAsync();
            items.AddRange(laptops);

            var tvs = await _context.TVs.Include(t => t.Category).ToListAsync();
            items.AddRange(tvs);

            var mobilePhones = await _context.MobilePhones.Include(p => p.Category).ToListAsync();
            items.AddRange(mobilePhones);

            var videoGames = await _context.VideoGames.Include(v => v.Category).ToListAsync();
            items.AddRange(videoGames);

            return items.Any() ? items.Skip((pageNumber - 1) * pageSize).Take(pageSize) : Enumerable.Empty<Item>();
        }

        public async Task<IEnumerable<Item>> GetFilteredItems(List<string> selectedFilters, int pageNumber, int pageSize)
        {
            var items = new List<Item>();

            foreach (var filter in selectedFilters)
            {
                switch (filter)
                {
                    case "Appliances":
                        var appliances = await GetDepartmentItems(filter);
                        items.AddRange(appliances);
                        break;

                    case "Electronics":
                        var electronics = await GetDepartmentItems(filter);
                        items.AddRange(electronics);
                        break;

                    case "Mobile Phones":
                        var mobilePhones = await GetDepartmentItems(filter);
                        items.AddRange(mobilePhones);
                        break;

                    case "Video Games":
                        var videoGames = await GetDepartmentItems(filter);
                        items.AddRange(videoGames);
                        break;

                    case "Air Conditioners":
                        var airConditioners = await _context.AirConditioners.AsNoTracking().ToListAsync();
                        items.AddRange(airConditioners);
                        break;

                    case "Cookers":
                        var cookers = await _context.Cookers.AsNoTracking().ToListAsync();
                        items.AddRange(cookers);
                        break;

                    case "Fridges":
                        var fridges = await _context.Fridges.AsNoTracking().ToListAsync();
                        items.AddRange(fridges);
                        break;

                    case "Washing Machines":
                        var washingMachines = await _context.WashingMachines.AsNoTracking().ToListAsync();
                        items.AddRange(washingMachines);
                        break;

                    case "Laptops":
                        var laptops = await _context.Laptops.AsNoTracking().ToListAsync();
                        items.AddRange(laptops);
                        break;

                    case "TVs":
                        var tvs = await _context.TVs.AsNoTracking().ToListAsync();
                        items.AddRange(tvs);
                        break;

                    case "Headphones":
                        var headphones = await _context.HeadPhones.AsNoTracking().ToListAsync();
                        items.AddRange(headphones);
                        break;

                    default:
                        break;
                }
            }

            return items.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public Task<IEnumerable<Item>> SortItems(IEnumerable<Item> items, string key, bool des)
        {
            Func<Item, object> keySelector = i => i.GetType().GetProperty(key)?.GetValue(i, null);

            IEnumerable<Item> sortedItems = des
                ? items.OrderByDescending(keySelector)
                : items.OrderBy(keySelector);

            return Task.FromResult(sortedItems);
        }

        private async Task<IEnumerable<Item>> GetDepartmentItems(string name)
        {
            var items = new List<Item>();

            switch (name)
            {
                case "Appliances":
                    var airConditioners = await _context.AirConditioners.AsNoTracking().ToListAsync();
                    items.AddRange(airConditioners);

                    var cookers = await _context.Cookers.AsNoTracking().ToListAsync();
                    items.AddRange(cookers);

                    var fridges = await _context.Fridges.AsNoTracking().ToListAsync();
                    items.AddRange(fridges);

                    var washingMachines = await _context.WashingMachines.AsNoTracking().ToListAsync();
                    items.AddRange(washingMachines);

                    break;

                case "Electronics":
                    var laptops = await _context.Laptops.AsNoTracking().ToListAsync();
                    items.AddRange(laptops);

                    var headphones = await _context.HeadPhones.AsNoTracking().ToListAsync();
                    items.AddRange(headphones);

                    var tvs = await _context.TVs.AsNoTracking().ToListAsync();
                    items.AddRange(tvs);

                    break;

                case "Mobile Phones":
                    var mobilePhones = await _context.MobilePhones.AsNoTracking().ToListAsync();
                    items.AddRange(mobilePhones);
                    break;

                case "Video Games":
                    var videoGames = await _context.VideoGames.AsNoTracking().ToListAsync();
                    items.AddRange(videoGames);
                    break;

                default:
                    break;
            }

            return items;
        }
    }
}
