using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IDepartmentsService
    {
        Task<Department> GetDepartment(int id);
        Task<Department> GetDepartment(string name);
        Task<IEnumerable<Department>> GetDepartments();
        Task<Result> Add(Department department);
        Task<Result> Update(Department department);
        Task<Result> Delete(Department department);
        Task<IEnumerable<Category>> GetAllDepartmentsCategories(IEnumerable<Department> departments);
        Task<IEnumerable<Item>> GetDepartmentItems(Department department);
    }
}