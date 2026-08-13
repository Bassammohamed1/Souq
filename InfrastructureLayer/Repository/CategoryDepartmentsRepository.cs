using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repository
{
    public class CategoryDepartmentsRepository : Repository<CategoryDepartments>, ICategoryDepartmentsRepository
    {
        private readonly AppDbContext _context;

        public CategoryDepartmentsRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<CategoryDepartments> GetAllCategoryDepartmentsWithDepartmentID(int departmentID)
        {
            return _context.CategoryDepartments.AsNoTracking()
                .Where(cd => cd.DepartmentId == departmentID);
        }
    }
}