using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface ICategoriesService
    {
        Task<Category> GetCategorie(int id);
        Task<IEnumerable<Category>> GetCategories();
        IEnumerable<Category> GetAllCategoriesWithDepartment(int pageNumber, int pageSize);
        Task<IEnumerable<Category>> GetSpecificCategories(string key);
        Task<List<int>> GetCategoryDepartments(int id);
        Task<IQueryable<Item>> GetCategoryItems(Category category);
        Task<Result> Add(CategoryDTO category);
        Task<Result> Update(CategoryDTO category);
        Task<Result> Delete(Category category);
        Task<IEnumerable<CategoryIndexDTO>> GetAllCategoriesForIndexPage(int? page);
    }
}
