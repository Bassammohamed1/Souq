using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface IDepartmentsRepository : IRepository<Department>
    {
        Task<IEnumerable<Category>> GetAllDepartmentsCategories(IEnumerable<Department> departments);

        Task<IEnumerable<Item>> GetDepartmentItems(Department department);
    }
}
