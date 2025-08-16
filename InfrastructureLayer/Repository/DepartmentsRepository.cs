using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repository
{
    public class DepartmentsRepository : Repository<Department>, IDepartmentsRepository
    {
        private readonly AppDbContext _context;

        public DepartmentsRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllDepartmentsCategories(IEnumerable<Department> departments)
        {
            var allCategories = new List<Category>();

            foreach (var department in departments)
            {
                var categoriesID = _context.CategoryDepartments.AsNoTracking().Where(cd => cd.DepartmentId == department.ID).Select(cd => cd.CategoryId);

                var categories = await _context.Categories.AsNoTracking().Where(c => categoriesID.Contains(c.ID)).ToListAsync();

                allCategories.AddRange(categories);
            }

            allCategories = allCategories.DistinctBy(c => c.ID).ToList();

            return allCategories.Any() ? allCategories : Enumerable.Empty<Category>();
        }

        public Task<IEnumerable<Item>> GetDepartmentItems(Department department)
        {
            var items = new List<Item>();

            switch (department.Name)
            {
                case "Appliances":
                    var appliancesCategoriesID = _context.CategoryDepartments.Where(cd => cd.DepartmentId == department.ID).Select(cd => cd.CategoryId);

                    var airConditioners = _context.AirConditioners.Where(a => appliancesCategoriesID.Contains(a.CategoryId));
                    items.AddRange(airConditioners);

                    var cookers = _context.Cookers.Where(c => appliancesCategoriesID.Contains(c.CategoryId));
                    items.AddRange(cookers);

                    var fridges = _context.Fridges.Where(f => appliancesCategoriesID.Contains(f.CategoryId));
                    items.AddRange(fridges);

                    var washingMachines = _context.WashingMachines.Where(w => appliancesCategoriesID.Contains(w.CategoryId));
                    items.AddRange(washingMachines);

                    break;

                case "Electronics":
                    var electronicsCategoriesID = _context.CategoryDepartments.Where(cd => cd.DepartmentId == department.ID).Select(cd => cd.CategoryId);

                    var laptops = _context.Laptops.Where(l => electronicsCategoriesID.Contains(l.CategoryId));
                    items.AddRange(laptops);

                    var tvs = _context.TVs.Where(t => electronicsCategoriesID.Contains(t.CategoryId));
                    items.AddRange(tvs);

                    var headphones = _context.HeadPhones.Where(h => electronicsCategoriesID.Contains(h.CategoryId));
                    items.AddRange(headphones);

                    break;

                case "Mobile Phones":
                    var mobilePhonesCategoriesID = _context.CategoryDepartments.Where(cd => cd.DepartmentId == department.ID).Select(cd => cd.CategoryId);

                    var mobilePhones = _context.MobilePhones.Where(p => mobilePhonesCategoriesID.Contains(p.CategoryId));
                    items.AddRange(mobilePhones);

                    break;

                case "Video Games":
                    var videoGamesCategoriesID = _context.CategoryDepartments.Where(cd => cd.DepartmentId == department.ID).Select(cd => cd.CategoryId);

                    var videoGames = _context.VideoGames.Where(v => videoGamesCategoriesID.Contains(v.CategoryId));
                    items.AddRange(videoGames);

                    break;

                default:
                    break;
            }

            return Task.FromResult(items.Any() ? items : Enumerable.Empty<Item>());
        }
    }
}
