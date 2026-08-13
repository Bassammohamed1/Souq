using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface ICategoryDepartmentsRepository : IRepository<CategoryDepartments>
    {
        IQueryable<CategoryDepartments> GetAllCategoryDepartmentsWithDepartmentID(int departmentID);
    }
}
