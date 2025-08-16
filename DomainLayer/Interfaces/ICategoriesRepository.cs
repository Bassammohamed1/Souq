using DomainLayer.DTOs;
using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface ICategoriesRepository : IRepository<Category>
    {
        Task AddCategory(CategoryDTO data);
        Task UpdateCategory(CategoryDTO data);
        Task DeleteCategory(CategoryDTO data);
        Task<IEnumerable<Category>> AllCategoriesWithDepartment(int pageNumber, int pageSize);
        Task<List<int>> GetCategoryDepartments(int id);
        Task<IQueryable<Category>> GetSpecificCategories(string key);
        Task<IQueryable<Item>> GetCategoryItems(Category category);
    }
}
