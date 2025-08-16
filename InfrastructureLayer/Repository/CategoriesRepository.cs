using DomainLayer.DTOs;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
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

        public Task<IEnumerable<Category>> AllCategoriesWithDepartment(int pageNumber, int pageSize)
        {
            var categories = _context.Categories.AsNoTracking().AsSplitQuery().OrderBy(c => c.Name).Include(c => c.CategoryDepartments).ThenInclude(d => d.Department).ToPagedList(pageNumber, pageSize);

            return Task.FromResult(categories.Any() ? categories : Enumerable.Empty<Category>());
        }

        public Task<IQueryable<Category>> GetSpecificCategories(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var departmentId = _context.Departments.AsNoTracking().FirstOrDefault(d => d.Name.ToLower() == key.ToLower()).ID;

                if (departmentId != null)
                {
                    var categoryIds = _context.CategoryDepartments.AsNoTracking().Where(cd => cd.DepartmentId == departmentId).Select(cd => cd.CategoryId).ToList();

                    var categories = _context.Categories.AsNoTracking().Where(c => categoryIds.Contains(c.ID));

                    return Task.FromResult(categories);

                }
                throw new ArgumentException("Invalid department name.");
            }
            return Task.FromResult(Enumerable.Empty<Category>().AsQueryable());
        }

        public async Task<List<int>> GetCategoryDepartments(int id)
        {
            var departmentsIds = await _context.CategoryDepartments.AsNoTracking().Where(c => c.CategoryId == id).Select(d => d.DepartmentId).ToListAsync();

            if (departmentsIds.Any())
                return departmentsIds;
            else
                return Enumerable.Empty<int>().ToList();
        }

        public async Task AddCategory(CategoryDTO data)
        {
            if (data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);

                var category = new Category()
                {
                    ID = data.Id,
                    Name = data.Name,
                    dbImage = stream.ToArray()
                };
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                foreach (var id in data.DepartmentsIds)
                {
                    var categoryDepartment = new CategoryDepartments()
                    {
                        CategoryId = category.ID,
                        DepartmentId = id
                    };
                    await _context.CategoryDepartments.AddAsync(categoryDepartment);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateCategory(CategoryDTO data)
        {
            if (data.clientFile is not null)
            {
                var stream = new MemoryStream();
                await data.clientFile.CopyToAsync(stream);

                var category = _context.Categories.Find(data.Id);
                if (category is not null)
                {
                    category.Name = data.Name;
                    category.dbImage = stream.ToArray();
                    await _context.SaveChangesAsync();
                }

                var existingCategoryDepartments = await _context.CategoryDepartments.AsNoTracking().Where(cd => cd.CategoryId == category.ID).ToListAsync();

                _context.CategoryDepartments.RemoveRange(existingCategoryDepartments);
                await _context.SaveChangesAsync();

                foreach (var id in data.DepartmentsIds)
                {
                    var categoryDepartment = new CategoryDepartments()
                    {
                        CategoryId = category.ID,
                        DepartmentId = id
                    };
                    await _context.CategoryDepartments.AddAsync(categoryDepartment);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCategory(CategoryDTO data)
        {
            var category = await _context.Categories.FindAsync(data.Id);

            if (category is not null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            else
                throw new InvalidOperationException();

            if (data.DepartmentsIds.Any())
            {
                foreach (var id in data.DepartmentsIds)
                {
                    var categoryDepartment = new CategoryDepartments()
                    {
                        CategoryId = category.ID,
                        DepartmentId = id
                    };
                    _context.CategoryDepartments.Remove(categoryDepartment);
                }
                await _context.SaveChangesAsync();
            }
        }

        public Task<IQueryable<Item>> GetCategoryItems(Category category)
        {
            var items = new List<Item>();

            var airConditioners = _context.AirConditioners.Where(a => a.CategoryId == category.ID);
            items.AddRange(airConditioners);

            var cookers = _context.Cookers.Where(c => c.CategoryId == category.ID);
            items.AddRange(cookers);

            var fridges = _context.Fridges.Where(f => f.CategoryId == category.ID);
            items.AddRange(fridges);

            var washingMachines = _context.WashingMachines.Where(w => w.CategoryId == category.ID);
            items.AddRange(washingMachines);

            var headPhones = _context.HeadPhones.Where(h => h.CategoryId == category.ID);
            items.AddRange(headPhones);

            var laptops = _context.Laptops.Where(l => l.CategoryId == category.ID);
            items.AddRange(laptops);

            var tvs = _context.TVs.Where(t => t.CategoryId == category.ID);
            items.AddRange(tvs);

            var mobilePhones = _context.MobilePhones.Where(m => m.CategoryId == category.ID);
            items.AddRange(mobilePhones);

            var videoGames = _context.VideoGames.Where(v => v.CategoryId == category.ID);
            items.AddRange(videoGames);


            return Task.FromResult(items.Any() ? items.AsQueryable() : Enumerable.Empty<Item>().AsQueryable());
        }
    }
}