using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class DepartmentsService : IDepartmentsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Department> GetDepartment(int id)
        {
            return await _unitOfWork.Departments.GetById(id);
        }

        public async Task<Department> GetDepartment(string name)
        {
            return await _unitOfWork.Departments.GetByName(name);
        }

        public async Task<IEnumerable<Department>> GetDepartments()
        {
            return  await _unitOfWork.Departments.GetAll();
        }

        public async Task<IEnumerable<Category>> GetAllDepartmentsCategories(IEnumerable<Department> departments)
        {
            var allCategories = new List<Category>();

            foreach (var department in departments)
            {
                var categoriesID = _unitOfWork.CategoryDepartments.GetAllCategoryDepartmentsWithDepartmentID(department.ID)
                    .Select(cd => cd.CategoryId);

                var categories = (await _unitOfWork.Categories.GetAll())
                    .Where(c => categoriesID.Contains(c.ID));

                allCategories.AddRange(categories);
            }

            allCategories = allCategories.DistinctBy(c => c.ID).ToList();

            return allCategories.Any() ? allCategories : Enumerable.Empty<Category>();
        }

        public async Task<IEnumerable<Item>> GetDepartmentItems(Department department)
        {
            var items = new List<Item>();

            switch (department.Name)
            {
                case "Appliances":
                    var appliancesCategoriesID = _unitOfWork.CategoryDepartments.GetAllCategoryDepartmentsWithDepartmentID(department.ID)
                        .Select(cd => cd.CategoryId);

                    var airConditioners = _unitOfWork.AirConditioners.GetAllByCategoryIDs(appliancesCategoriesID);

                    items.AddRange(airConditioners);

                    var cookers = _unitOfWork.Cookers.GetAllByCategoryIDs(appliancesCategoriesID);

                    items.AddRange(cookers);

                    var fridges = _unitOfWork.Fridges.GetAllByCategoryIDs(appliancesCategoriesID);

                    items.AddRange(fridges);

                    var washingMachines = _unitOfWork.WashingMachines.GetAllByCategoryIDs(appliancesCategoriesID);

                    items.AddRange(washingMachines);

                    break;

                case "Electronics":
                    var electronicsCategoriesID = _unitOfWork.CategoryDepartments.GetAllCategoryDepartmentsWithDepartmentID(department.ID)
                        .Select(cd => cd.CategoryId);

                    var laptops = _unitOfWork.Laptops.GetAllByCategoryIDs(electronicsCategoriesID);

                    items.AddRange(laptops);

                    var tvs = _unitOfWork.TVs.GetAllByCategoryIDs(electronicsCategoriesID);

                    items.AddRange(tvs);

                    var headphones = _unitOfWork.HeadPhones.GetAllByCategoryIDs(electronicsCategoriesID);

                    items.AddRange(headphones);

                    break;

                case "Mobile Phones":
                    var mobilePhonesCategoriesID = _unitOfWork.CategoryDepartments.GetAllCategoryDepartmentsWithDepartmentID(department.ID)
                        .Select(cd => cd.CategoryId);

                    var mobilePhones = _unitOfWork.MobilePhones.GetAllByCategoryIDs(mobilePhonesCategoriesID);

                    items.AddRange(mobilePhones);

                    break;

                case "Video Games":
                    var videoGamesCategoriesID = _unitOfWork.CategoryDepartments.GetAllCategoryDepartmentsWithDepartmentID(department.ID)
                        .Select(cd => cd.CategoryId);

                    var videoGames = _unitOfWork.VideoGames.GetAllByCategoryIDs(videoGamesCategoriesID);
                    items.AddRange(videoGames);

                    break;

                default:
                    break;
            }

            return items.Any() ? items : Enumerable.Empty<Item>();
        }

        public async Task<Result> Add(Department department)
        {
            var result = await _unitOfWork.Departments.Add(department);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true } :
                new Result() { Success = false, Error = "An error occured while adding." };
        }

        public async Task<Result> Update(Department department)
        {
            var result = _unitOfWork.Departments.Update(department);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true } :
               new Result() { Success = false, Error = "An error occured while updating." };
        }

        public async Task<Result> Delete(Department department)
        {
            var result = _unitOfWork.Departments.Delete(department);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true } :
               new Result() { Success = false, Error = "An error occured while deleting." };
        }
    }
}