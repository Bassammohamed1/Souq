using ApplicationLayer.DTOs;
using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace SouqTests
{
    public class CategoriesServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CategoriesService _service;

        public CategoriesServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();

            _service = new CategoriesService(_unitOfWork);
        }

        [Fact]
        public async Task GetCategorie_ShouldReturnCategory()
        {
            // Arrange
            var category = new Category { ID = 1, Name = "Laptops" };

            A.CallTo(() => _unitOfWork.Categories.GetById(1))
                .Returns(category);

            // Act
            var result = await _service.GetCategorie(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ID);
            Assert.Equal("Laptops", result.Name);
        }

        [Fact]
        public async Task GetCategorie_ShouldReturnNull_WhenNotFound()
        {
            A.CallTo(() => _unitOfWork.Categories.GetById(1))
                .Returns((Category)null);

            var result = await _service.GetCategorie(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetCategories_ShouldReturnCategories()
        {
            // Arrange
            var categories = new List<Category>
    {
        new Category { ID = 1, Name = "Laptops" },
        new Category { ID = 2, Name = "TVs" }
    };

            A.CallTo(() => _unitOfWork.Categories.GetAll())
                .Returns(categories);

            // Act
            var result = await _service.GetCategories();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetAllCategoriesWithDepartment_ShouldReturnCategories()
        {
            // Arrange
            var categories = new List<Category>
    {
        new Category { ID = 1, Name = "Laptops" }
    };

            A.CallTo(() => _unitOfWork.Categories
                .AllCategoriesWithDepartment(1, 10))
                .Returns(categories);

            // Act
            var result = _service.GetAllCategoriesWithDepartment(1, 10);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetSpecificCategories_ShouldReturnEmpty_WhenKeyIsEmpty()
        {
            var result = await _service.GetSpecificCategories("");

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSpecificCategories_ShouldThrow_WhenDepartmentDoesNotExist()
        {
            var departments = A.Fake<IRepository<Department>>();

            A.CallTo(() => _unitOfWork.Departments)
                .Returns(departments);

            A.CallTo(() => departments.GetByName("Invalid"))
                .Returns((Department)null);

            await Assert.ThrowsAsync<NullReferenceException>(
                () => _service.GetSpecificCategories("Invalid"));
        }

        [Fact]
        public async Task GetCategoryDepartments_ShouldReturnDepartmentIds()
        {
            // Arrange
            var ids = new List<int> { 1, 2, 3 };

            A.CallTo(() => _unitOfWork.Categories.GetCategoryDepartments(5))
                .Returns(ids);

            // Act
            var result = await _service.GetCategoryDepartments(5);

            // Assert
            Assert.Equal(ids, result);
        }

        [Fact]
        public async Task Delete_ShouldReturnSuccess_WhenDeleteSucceeds()
        {
            // Arrange
            var category = new Category { ID = 1 };

            A.CallTo(() => _unitOfWork.Categories.Delete(category))
                .Returns(category);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Delete(category);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Delete_ShouldReturnFailure_WhenDeleteFails()
        {
            var category = new Category { ID = 1 };

            A.CallTo(() => _unitOfWork.Categories.Delete(category))
                .Returns((Category)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            var result = await _service.Delete(category);

            Assert.False(result.Success);
            Assert.Equal("An Error occured while deleting.", result.Error);
        }

        [Fact]
        public async Task GetCategoryItems_ShouldReturnItems()
        {
            // Arrange
            var category = new Category { ID = 1, Name = "Laptops" };

            var items = new List<Item>
    {
        new Laptop { ID = 1, Name = "Laptop 1" },
        new Laptop { ID = 2, Name = "Laptop 2" }
    }.AsQueryable();

            A.CallTo(() => _unitOfWork.Categories.GetCategoryItems(category))
                .Returns(items);

            // Act
            var result = await _service.GetCategoryItems(category);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Add_ShouldReturnFailure_WhenFileIsMissing()
        {
            // Arrange
            var data = new CategoryDTO
            {
                Id = 1,
                Name = "Laptops",
                clientFile = null
            };

            // Act
            var result = await _service.Add(data);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Client file is missing.", result.Error);

            A.CallTo(() => _unitOfWork.Commit())
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task Add_ShouldReturnSuccess_WhenCategoryIsAdded()
        {
            // Arrange
            var file = A.Fake<IFormFile>();

            var imageBytes = new byte[] { 1, 2, 3 };

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Invokes((Stream stream, CancellationToken _) =>
                {
                    stream.Write(imageBytes, 0, imageBytes.Length);
                })
                .Returns(Task.CompletedTask);

            var data = new CategoryDTO
            {
                Id = 1,
                Name = "Laptops",
                clientFile = file,
                DepartmentsIds = new List<int> { 1, 2 }
            };

            var category = new Category
            {
                ID = 1,
                Name = "Laptops"
            };

            A.CallTo(() => _unitOfWork.Categories.AddCategory(
                A<Category>._,
                data.DepartmentsIds))
                .Returns(category);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Add(data);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() => _unitOfWork.Categories.AddCategory(
                A<Category>.That.Matches(c =>
                    c.ID == 1 &&
                    c.Name == "Laptops" &&
                    c.dbImage.SequenceEqual(imageBytes)),
                data.DepartmentsIds))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Add_ShouldReturnFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            var file = A.Fake<IFormFile>();

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Returns(Task.CompletedTask);

            var data = new CategoryDTO
            {
                Id = 1,
                Name = "Laptops",
                clientFile = file,
                DepartmentsIds = new List<int> { 1 }
            };

            A.CallTo(() => _unitOfWork.Categories.AddCategory(
                A<Category>._,
                data.DepartmentsIds))
                .Returns((Category)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Add(data);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("An Error occured while adding.", result.Error);
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenFileIsMissing()
        {
            // Arrange
            var data = new CategoryDTO
            {
                Id = 1,
                Name = "Laptops",
                clientFile = null
            };

            // Act
            var result = await _service.Update(data);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Client file is missing.", result.Error);

            A.CallTo(() => _unitOfWork.Commit())
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task Update_ShouldReturnSuccess_WhenCategoryIsUpdated()
        {
            // Arrange
            var file = A.Fake<IFormFile>();

            var imageBytes = new byte[] { 10, 20, 30 };

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Invokes((Stream stream, CancellationToken _) =>
                {
                    stream.Write(imageBytes, 0, imageBytes.Length);
                })
                .Returns(Task.CompletedTask);

            var data = new CategoryDTO
            {
                Id = 1,
                Name = "Updated Laptops",
                clientFile = file,
                DepartmentsIds = new List<int> { 1 }
            };

            var category = new Category
            {
                ID = 1,
                Name = "Updated Laptops"
            };

            A.CallTo(() => _unitOfWork.Categories.UpdateCategory(
                A<Category>._,
                data.DepartmentsIds))
                .Returns(category);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Update(data);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() => _unitOfWork.Categories.UpdateCategory(
                A<Category>.That.Matches(c =>
                    c.ID == 1 &&
                    c.Name == "Updated Laptops" &&
                    c.dbImage.SequenceEqual(imageBytes)),
                data.DepartmentsIds))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            var file = A.Fake<IFormFile>();

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Returns(Task.CompletedTask);

            var data = new CategoryDTO
            {
                Id = 1,
                Name = "Laptops",
                clientFile = file,
                DepartmentsIds = new List<int> { 1 }
            };

            A.CallTo(() => _unitOfWork.Categories.UpdateCategory(
                A<Category>._,
                data.DepartmentsIds))
                .Returns((Category)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Update(data);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("An Error occured while updating.", result.Error);
        }

        [Fact]
        public async Task GetAllCategoriesForIndexPage_ShouldReturnMappedCategories()
        {
            // Arrange
            var department1 = new Department { Name = "Electronics" };
            var department2 = new Department { Name = "Appliances" };

            var category = new Category
            {
                ID = 1,
                Name = "Laptops",
                CategoryDepartments = new List<CategoryDepartments>
        {
            new CategoryDepartments { Department = department1 },
            new CategoryDepartments { Department = department2 }
        }
            };

            var categories = new List<Category> { category };

            A.CallTo(() => _unitOfWork.Categories.GetAll())
                .Returns(categories);

            A.CallTo(() => _unitOfWork.Categories
                .AllCategoriesWithDepartment(1, 10))
                .Returns(categories);

            // Act
            var result = await _service.GetAllCategoriesForIndexPage(null);

            // Assert
            Assert.NotNull(result);

            var item = result.First();

            Assert.Equal(1, item.Id);
            Assert.Equal("Laptops", item.Name);

            Assert.Equal(1, item.CurrentPage);
            Assert.Equal(1, item.TotalPages);

            Assert.Contains("Electronics", item.Departments);
            Assert.Contains("Appliances", item.Departments);
        }

        [Fact]
        public async Task GetAllCategoriesForIndexPage_ShouldUseProvidedPage()
        {
            // Arrange
            var categories = new List<Category>
    {
        new Category
        {
            ID = 1,
            Name = "Laptops",
            CategoryDepartments = new List<CategoryDepartments>()
        }
    };

            A.CallTo(() => _unitOfWork.Categories.GetAll())
                .Returns(categories);

            A.CallTo(() => _unitOfWork.Categories
                .AllCategoriesWithDepartment(3, 10))
                .Returns(categories);

            // Act
            var result = await _service.GetAllCategoriesForIndexPage(3);

            // Assert
            Assert.Equal(3, result.First().CurrentPage);

            A.CallTo(() => _unitOfWork.Categories
                .AllCategoriesWithDepartment(3, 10))
                .MustHaveHappenedOnceExactly();
        }
    }
}
