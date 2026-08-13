using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using X.PagedList.Extensions;

namespace InfrastructureLayer.Repository
{
    public class CategoriesRepository : Repository<Category>, ICategoriesRepository
    {
        private readonly AppDbContext _context;

        public CategoriesRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Category> GetCategoryByName(string name)
        {
            return await _context.Categories.AsNoTracking()
                .FirstOrDefaultAsync(d => d.Name.ToLower() == name.ToLower());
        }

        public IEnumerable<Category> AllCategoriesWithDepartment(int pageNumber, int pageSize)
        {
            return _context.Categories.AsNoTracking().AsSplitQuery()
                .OrderBy(c => c.Name).Include(c => c.CategoryDepartments)
                .ThenInclude(d => d.Department).ToPagedList(pageNumber, pageSize);
        }

        public async Task<List<int>> GetCategoryDepartments(int id)
        {
            var departmentsIds = await _context.CategoryDepartments.AsNoTracking()
                .Where(c => c.CategoryId == id)
                .Select(d => d.DepartmentId).ToListAsync();

            return departmentsIds.Any() ? departmentsIds : Enumerable.Empty<int>().ToList();
        }

        public async Task<IQueryable<Item>> GetCategoryItems(Category category)
        {
            var items = new List<Item>();

            var airConditioners = await _context.AirConditioners.AsNoTracking()
                .Where(a => a.CategoryId == category.ID).ToListAsync();

            items.AddRange(airConditioners);

            var cookers = await _context.Cookers.AsNoTracking()
                .Where(c => c.CategoryId == category.ID).ToListAsync();

            items.AddRange(cookers);

            var fridges = await _context.Fridges.AsNoTracking()
                .Where(f => f.CategoryId == category.ID).ToListAsync();

            items.AddRange(fridges);

            var washingMachines = await _context.WashingMachines.AsNoTracking()
                .Where(w => w.CategoryId == category.ID).ToListAsync();

            items.AddRange(washingMachines);

            var headPhones = await _context.HeadPhones.AsNoTracking()
                .Where(h => h.CategoryId == category.ID).ToListAsync();

            items.AddRange(headPhones);

            var laptops = await _context.Laptops.AsNoTracking()
                .Where(l => l.CategoryId == category.ID).ToListAsync();

            items.AddRange(laptops);

            var tvs = await _context.TVs.AsNoTracking()
                .Where(t => t.CategoryId == category.ID).ToListAsync();

            items.AddRange(tvs);

            var mobilePhones = await _context.MobilePhones.AsNoTracking()
                .Where(m => m.CategoryId == category.ID).ToListAsync();

            items.AddRange(mobilePhones);

            var videoGames = await _context.VideoGames.AsNoTracking()
                .Where(v => v.CategoryId == category.ID).ToListAsync();

            items.AddRange(videoGames);

            return items.Any() ? items.AsQueryable() : Enumerable.Empty<Item>().AsQueryable();
        }

        public async Task<Category> AddCategory(Category data, List<int> DepartmentsIds)
        {
            await _context.Categories.AddAsync(data);
            await _context.SaveChangesAsync();

            foreach (var id in DepartmentsIds)
            {
                var categoryDepartment = new CategoryDepartments()
                {
                    CategoryId = data.ID,
                    DepartmentId = id
                };

                await _context.CategoryDepartments.AddAsync(categoryDepartment);
            }

            await _context.SaveChangesAsync();

            return data;
        }

        public async Task<Category> UpdateCategory(Category data, List<int> DepartmentsIds)
        {
            var category = await _context.Categories.FindAsync(data.ID);

            if (category is not null)
            {
                category.Name = data.Name;
                category.dbImage = data.dbImage;

                await _context.SaveChangesAsync();
            }

            var existingCategoryDepartments = await _context.CategoryDepartments.AsNoTracking()
                .Where(cd => cd.CategoryId == category.ID).ToListAsync();

            _context.CategoryDepartments.RemoveRange(existingCategoryDepartments);
            await _context.SaveChangesAsync();

            foreach (var id in DepartmentsIds)
            {
                var categoryDepartment = new CategoryDepartments()
                {
                    CategoryId = category.ID,
                    DepartmentId = id
                };
                await _context.CategoryDepartments.AddAsync(categoryDepartment);
            }

            await _context.SaveChangesAsync();

            return data;
        }

        public List<int> GetCategoryIDsFromExpression<T>() where T : Item
        {
            var entityType = typeof(T);
            var propertyInfo = entityType.GetProperty("CategoryId", BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo == null || propertyInfo.PropertyType != typeof(int))
            {
                throw new InvalidOperationException($"{entityType.Name} does not have an int property named 'CategoryId'.");
            }

            var parameter = Expression.Parameter(entityType, "x");
            var propertyAccess = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var dbSet = _context.Set<T>().AsQueryable();

            var selectMethod = typeof(Queryable).GetMethods()
                .First(m =>
                m.Name == "Select" &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
                .MakeGenericMethod(entityType, typeof(int));


            var selectedQuery = selectMethod.Invoke(null, new object[] { dbSet, lambda });

            var distinctMethod = typeof(Queryable).GetMethods()
                .Where(m => m.Name == "Distinct" && m.GetParameters().Length == 1)
                .Single()
                .MakeGenericMethod(typeof(int));

            var distinctQuery = distinctMethod.Invoke(null, new object[] { selectedQuery });

            var toListMethod = typeof(Enumerable).GetMethod("ToList")
                .MakeGenericMethod(typeof(int));

            var result = toListMethod.Invoke(null, new object[] { distinctQuery });

            return (List<int>)result;
        }
    }
}