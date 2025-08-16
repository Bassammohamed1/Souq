using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repository
{
    public class MobilePhonesRepository : Repository<MobilePhone>, IMobilePhonesRepository
    {
        private readonly AppDbContext _context;
        public MobilePhonesRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<IQueryable<MobilePhone>> GetPhonesFilteredByStorage(int storage, int pageNumber, int pageSize, string orderKey, bool desOrder)
        {
            if (desOrder)
            {
                if (storage == 256)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var phones = _context.MobilePhones.AsNoTracking().AsSplitQuery()
                            .Where(mp => mp.MemoryStorageCapacity >= storage)
                            .Include(p => p.Category)
                            .OrderByDescending(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return Task.FromResult(phones.Any() ? phones : Enumerable.Empty<MobilePhone>().AsQueryable());
                    }
                    else
                        throw new ArgumentException();
                }
                else
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var phones = _context.MobilePhones.AsNoTracking().AsSplitQuery()
                            .Where(mp => mp.MemoryStorageCapacity == storage)
                            .Include(p => p.Category)
                            .OrderByDescending(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return Task.FromResult(phones.Any() ? phones : Enumerable.Empty<MobilePhone>().AsQueryable());
                    }
                    else
                        throw new ArgumentException();
                }
            }
            else
            {
                if (storage == 256)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var phones = _context.MobilePhones.AsNoTracking().AsSplitQuery()
                            .Where(mp => mp.MemoryStorageCapacity >= storage)
                            .Include(p => p.Category)
                            .OrderBy(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return Task.FromResult(phones.Any() ? phones : Enumerable.Empty<MobilePhone>().AsQueryable());
                    }
                    else
                        throw new ArgumentException();
                }
                else
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var phones = _context.MobilePhones.AsNoTracking().AsSplitQuery()
                            .Where(mp => mp.MemoryStorageCapacity == storage)
                            .Include(p => p.Category)
                            .OrderBy(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return Task.FromResult(phones.Any() ? phones : Enumerable.Empty<MobilePhone>().AsQueryable());
                    }
                    else
                        throw new ArgumentException();
                }
            }
        }

        public async Task<int> TotalFilterStoragePhones(int Storage)
        {
            var max = await _context.MobilePhones.AsNoTracking().CountAsync();

            var phonesCount = await GetPhonesFilteredByStorage(Storage, 1, max, "ID", false);
            var result = phonesCount.Count();

            return result;
        }
    }
}