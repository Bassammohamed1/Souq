using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface ICategoriesRepository : IRepository<Category>
    {
        Task<Category> GetCategoryByName(string name);
        IEnumerable<Category> AllCategoriesWithDepartment(int pageNumber, int pageSize);
        Task<List<int>> GetCategoryDepartments(int id);
        Task<IQueryable<Item>> GetCategoryItems(Category category);
        Task<Category> AddCategory(Category data, List<int> DepartmentsIds);
        Task<Category> UpdateCategory(Category data, List<int> DepartmentsIds);
        List<int> GetCategoryIDsFromExpression<T>() where T : Item;
    }
}