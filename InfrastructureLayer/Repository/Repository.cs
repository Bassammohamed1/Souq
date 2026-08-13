using DomainLayer.Interfaces;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace InfrastructureLayer.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetById(int id)
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();

            var navCategory = _context.Model
                .FindEntityType(typeof(T))?
                .FindNavigation("Category");

            if (navCategory != null)
            {
                query = query.AsNoTracking().AsSplitQuery().Include("Category");
            }

            return await query.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, "ID") == id);
        }

        public async Task<T> GetByName(string name)
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();

            var navCategory = _context.Model
               .FindEntityType(typeof(T))?
               .FindNavigation("Category");

            if (navCategory != null)
            {
                query = query.AsNoTracking().AsSplitQuery().Include("Category");
            }

            return await query.AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<string>(e, "Name").ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();

            var navCategory = _context.Model
                .FindEntityType(typeof(T))?
                .FindNavigation("Category");

            if (navCategory != null)
            {
                query = query.AsNoTracking().AsSplitQuery().Include("Category");
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public IPagedList<T> GetAll(int pageNumber, int pageSize)
        {
            return _context.Set<T>().AsNoTracking()
               .OrderBy(i => EF.Property<string>(i, "Name"))
               .ToPagedList(pageNumber, pageSize);
        }

        public IQueryable<T> GetAllByIDs(IEnumerable<int> ids)
        {
            return _context.Set<T>().AsNoTracking()
                  .Where(c => ids.Contains(EF.Property<int>(c, "ID")));
        }

        public IQueryable<T> GetAllByCategoryIDs(IEnumerable<int> ids)
        {
            return _context.Set<T>().AsNoTracking()
                  .Where(c => ids.Contains(EF.Property<int>(c, "CategoryId")));
        }

        public async Task<T> Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);

            return entity;
        }

        public T Update(T entity)
        {
            _context.Set<T>().Update(entity);

            return entity;
        }

        public T Delete(T entity)
        {
            _context.Set<T>().Remove(entity);

            return entity;
        }
    }
}