using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Category> GetCategorie(int id)
        {
            return await _unitOfWork.Categories.GetById(id);
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _unitOfWork.Categories.GetAll();
        }

        public IEnumerable<Category> GetAllCategoriesWithDepartment(int pageNumber, int pageSize)
        {
            return _unitOfWork.Categories.AllCategoriesWithDepartment(pageNumber, pageSize);
        }

        public async Task<IEnumerable<Category>> GetSpecificCategories(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var departmentId = (await _unitOfWork.Departments.GetByName(key)).ID;

                if (departmentId != null)
                {
                    var categoryIds = _unitOfWork.CategoryDepartments.GetAllCategoryDepartmentsWithDepartmentID(departmentId)
                        .Select(cd => cd.CategoryId).ToList();

                    return (await _unitOfWork.Categories.GetAll())
                         .Where(c => categoryIds.Contains(c.ID));
                }

                throw new ArgumentException("Invalid department name.");
            }

            return Enumerable.Empty<Category>();
        }

        public async Task<List<int>> GetCategoryDepartments(int id)
        {
            return await _unitOfWork.Categories.GetCategoryDepartments(id);
        }

        public async Task<IQueryable<Item>> GetCategoryItems(Category category)
        {
            return await _unitOfWork.Categories.GetCategoryItems(category);
        }

        public async Task<Result> Add(CategoryDTO data)
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

                var result = await _unitOfWork.Categories.AddCategory(category, data.DepartmentsIds);

                await _unitOfWork.Commit();

                return result is not null ? new Result() { Success = true } :
                    new Result()
                    {
                        Success = false,
                        Error = "An Error occured while adding."
                    };
            }
            return new Result()
            {
                Success = false,
                Error = "Client file is missing."
            };
        }

        public async Task<Result> Update(CategoryDTO data)
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

                var result = await _unitOfWork.Categories.UpdateCategory(category, data.DepartmentsIds);

                await _unitOfWork.Commit();

                return result is not null ? new Result() { Success = true } :
                    new Result()
                    {
                        Success = false,
                        Error = "An Error occured while updating."
                    };
            }

            return new Result()
            {
                Success = false,
                Error = "Client file is missing."
            };
        }

        public async Task<Result> Delete(Category data)
        {
            var result = _unitOfWork.Categories.Delete(data);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true } :
                   new Result()
                   {
                       Success = false,
                       Error = "An Error occured while deleting."
                   };
        }

        public async Task<IEnumerable<CategoryIndexDTO>> GetAllCategoriesForIndexPage(int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var totalCount = (await this.GetCategories()).Count();

            var categories = this.GetAllCategoriesWithDepartment(pageNumber, pageSize);

            return categories.Select(c => new CategoryIndexDTO
            {
                Id = c.ID,
                Name = c.Name,
                Image = c.imageSrc,
                Departments = c.CategoryDepartments.Select(cd => cd.Department.Name).ToList(),
                TotalPages = totalCount,
                CurrentPage = pageNumber
            }).ToList();
        }
    }
}
