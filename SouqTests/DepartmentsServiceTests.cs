using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using FakeItEasy;
using Xunit;

namespace SouqTests
{
    public class DepartmentsServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DepartmentsService _service;

        public DepartmentsServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _service = new DepartmentsService(_unitOfWork);
        }

        [Fact]
        public async Task GetDepartment_ById_ShouldReturnDepartment()
        {
            var department = new Department { ID = 1, Name = "Electronics" };

            A.CallTo(() => _unitOfWork.Departments.GetById(1))
                .Returns(department);

            var result = await _service.GetDepartment(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.ID);
            Assert.Equal("Electronics", result.Name);
        }

        [Fact]
        public async Task GetDepartment_ByName_ShouldReturnDepartment()
        {
            var department = new Department { ID = 1, Name = "Electronics" };

            A.CallTo(() => _unitOfWork.Departments.GetByName("Electronics"))
                .Returns(department);

            var result = await _service.GetDepartment("Electronics");

            Assert.NotNull(result);
            Assert.Equal("Electronics", result.Name);
        }

        [Fact]
        public async Task GetDepartments_ShouldReturnDepartments()
        {
            var departments = new List<Department>
    {
        new Department { ID = 1, Name = "Electronics" },
        new Department { ID = 2, Name = "Appliances" }
    };

            A.CallTo(() => _unitOfWork.Departments.GetAll())
                .Returns(departments);

            var result = await _service.GetDepartments();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllDepartmentsCategories_ShouldReturnDistinctCategories()
        {
            var departments = new List<Department>
    {
        new Department { ID = 1, Name = "Electronics" },
        new Department { ID = 2, Name = "Appliances" }
    };

            var categoryDepartments1 = new List<CategoryDepartments>
    {
        new CategoryDepartments { CategoryId = 1 },
        new CategoryDepartments { CategoryId = 2 }
    };

            var categoryDepartments2 = new List<CategoryDepartments>
    {
        new CategoryDepartments { CategoryId = 2 },
        new CategoryDepartments { CategoryId = 3 }
    };

            var categories = new List<Category>
    {
        new Category { ID = 1, Name = "Laptops" },
        new Category { ID = 2, Name = "Samsung" },
        new Category { ID = 3, Name = "AC" }
    };

            A.CallTo(() =>
                _unitOfWork.CategoryDepartments
                    .GetAllCategoryDepartmentsWithDepartmentID(1))
                .Returns(categoryDepartments1.AsQueryable());

            A.CallTo(() =>
                _unitOfWork.CategoryDepartments
                    .GetAllCategoryDepartmentsWithDepartmentID(2))
                .Returns(categoryDepartments2.AsQueryable());

            A.CallTo(() => _unitOfWork.Categories.GetAll())
                .Returns(categories);

            var result = await _service.GetAllDepartmentsCategories(departments);

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllDepartmentsCategories_ShouldReturnEmpty_WhenNoCategories()
        {
            var departments = new List<Department>
    {
        new Department { ID = 1, Name = "Electronics" }
    };

            A.CallTo(() =>
                _unitOfWork.CategoryDepartments
                    .GetAllCategoryDepartmentsWithDepartmentID(1))
                .Returns(new List<CategoryDepartments>().AsQueryable());

            A.CallTo(() => _unitOfWork.Categories.GetAll())
                .Returns(new List<Category>());

            var result = await _service.GetAllDepartmentsCategories(departments);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDepartmentItems_Appliances_ShouldReturnAllApplianceItems()
        {
            var department = new Department
            {
                ID = 1,
                Name = "Appliances"
            };

            var categoryDepartments = new List<CategoryDepartments>
    {
        new CategoryDepartments { CategoryId = 10 }
    };

            var ids = categoryDepartments.Select(x => x.CategoryId);

            var airConditioners = new List<AirConditioner>
    {
        new AirConditioner { ID = 1 }
    };

            var cookers = new List<Cooker>
    {
        new Cooker { ID = 2 }
    };

            var fridges = new List<Fridge>
    {
        new Fridge { ID = 3 }
    };

            var washingMachines = new List<WashingMachine>
    {
        new WashingMachine { ID = 4 }
    };

            A.CallTo(() => _unitOfWork.CategoryDepartments
                .GetAllCategoryDepartmentsWithDepartmentID(1))
                .Returns(categoryDepartments.AsQueryable());

            A.CallTo(() => _unitOfWork.AirConditioners.GetAllByCategoryIDs(ids))
                .Returns(airConditioners.AsQueryable());

            A.CallTo(() => _unitOfWork.Cookers.GetAllByCategoryIDs(ids))
                .Returns(cookers.AsQueryable());

            A.CallTo(() => _unitOfWork.Fridges.GetAllByCategoryIDs(ids))
                .Returns(fridges.AsQueryable());

            A.CallTo(() => _unitOfWork.WashingMachines.GetAllByCategoryIDs(ids))
                .Returns(washingMachines.AsQueryable());

            var result = await _service.GetDepartmentItems(department);

            Assert.Equal(4, result.Count());
        }

        [Fact]
        public async Task GetDepartmentItems_Electronics_ShouldReturnAllElectronicItems()
        {
            var department = new Department
            {
                ID = 1,
                Name = "Electronics"
            };

            var categoryDepartments = new List<CategoryDepartments>
    {
        new CategoryDepartments { CategoryId = 10 }
    };

            var laptops = new List<Laptop>
    {
        new Laptop { ID = 1 }
    };

            var tvs = new List<TV>
    {
        new TV { ID = 2 }
    };

            var headphones = new List<HeadPhone>
    {
        new HeadPhone { ID = 3 }
    };

            A.CallTo(() => _unitOfWork.CategoryDepartments
                .GetAllCategoryDepartmentsWithDepartmentID(1))
                .Returns(categoryDepartments.AsQueryable());

            A.CallTo(() => _unitOfWork.Laptops.GetAllByCategoryIDs(A<IEnumerable<int>>._))
                .Returns(laptops.AsQueryable());

            A.CallTo(() => _unitOfWork.TVs.GetAllByCategoryIDs(A<IEnumerable<int>>._))
                .Returns(tvs.AsQueryable());

            A.CallTo(() => _unitOfWork.HeadPhones.GetAllByCategoryIDs(A<IEnumerable<int>>._))
                .Returns(headphones.AsQueryable());

            var result = await _service.GetDepartmentItems(department);

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetDepartmentItems_MobilePhones_ShouldReturnMobilePhones()
        {
            var department = new Department
            {
                ID = 1,
                Name = "Mobile Phones"
            };

            var categoryDepartments = new List<CategoryDepartments>
    {
        new CategoryDepartments { CategoryId = 10 }
    };

            var phones = new List<MobilePhone>
    {
        new MobilePhone { ID = 1 },
        new MobilePhone { ID = 2 }
    };

            A.CallTo(() => _unitOfWork.CategoryDepartments
                .GetAllCategoryDepartmentsWithDepartmentID(1))
                .Returns(categoryDepartments.AsQueryable());

            A.CallTo(() => _unitOfWork.MobilePhones.GetAllByCategoryIDs(A<IEnumerable<int>>._))
                .Returns(phones.AsQueryable());

            var result = await _service.GetDepartmentItems(department);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetDepartmentItems_VideoGames_ShouldReturnVideoGames()
        {
            var department = new Department
            {
                ID = 1,
                Name = "Video Games"
            };

            var categoryDepartments = new List<CategoryDepartments>
    {
        new CategoryDepartments { CategoryId = 10 }
    };

            var games = new List<VideoGame>
    {
        new VideoGame { ID = 1 },
        new VideoGame { ID = 2 }
    };

            A.CallTo(() => _unitOfWork.CategoryDepartments
                .GetAllCategoryDepartmentsWithDepartmentID(1))
                .Returns(categoryDepartments.AsQueryable());

            A.CallTo(() => _unitOfWork.VideoGames.GetAllByCategoryIDs(A<IEnumerable<int>>._))
                .Returns(games.AsQueryable());

            var result = await _service.GetDepartmentItems(department);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetDepartmentItems_UnknownDepartment_ShouldReturnEmpty()
        {
            var department = new Department
            {
                ID = 1,
                Name = "Unknown"
            };

            var result = await _service.GetDepartmentItems(department);

            Assert.Empty(result);
        }

        [Fact]
        public async Task Add_ShouldReturnSuccess_WhenAdded()
        {
            var department = new Department
            {
                ID = 1,
                Name = "Electronics"
            };

            A.CallTo(() => _unitOfWork.Departments.Add(department))
                .Returns(department);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            var result = await _service.Add(department);

            Assert.True(result.Success);

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Add_ShouldReturnFailure_WhenAddReturnsNull()
        {
            var department = new Department
            {
                ID = 1,
                Name = "Electronics"
            };

            A.CallTo(() => _unitOfWork.Departments.Add(department))
                .Returns((Department)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            var result = await _service.Add(department);

            Assert.False(result.Success);
            Assert.Equal("An error occured while adding.", result.Error);
        }

        [Fact]
        public async Task Update_ShouldReturnSuccess_WhenUpdated()
        {
            var department = new Department
            {
                ID = 1,
                Name = "Electronics"
            };

            A.CallTo(() => _unitOfWork.Departments.Update(department))
                .Returns(department);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            var result = await _service.Update(department);

            Assert.True(result.Success);

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenUpdateReturnsNull()
        {
            var department = new Department { ID = 1 };

            A.CallTo(() => _unitOfWork.Departments.Update(department))
                .Returns((Department)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            var result = await _service.Update(department);

            Assert.False(result.Success);
            Assert.Equal("An error occured while updating.", result.Error);
        }

        [Fact]
        public async Task Delete_ShouldReturnSuccess_WhenDeleted()
        {
            var department = new Department
            {
                ID = 1,
                Name = "Electronics"
            };

            A.CallTo(() => _unitOfWork.Departments.Delete(department))
                .Returns(department);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            var result = await _service.Delete(department);

            Assert.True(result.Success);

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Delete_ShouldReturnFailure_WhenDeleteReturnsNull()
        {
            var department = new Department { ID = 1 };

            A.CallTo(() => _unitOfWork.Departments.Delete(department))
                .Returns((Department)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            var result = await _service.Delete(department);

            Assert.False(result.Success);
            Assert.Equal("An error occured while deleting.", result.Error);
        }


    }
}
