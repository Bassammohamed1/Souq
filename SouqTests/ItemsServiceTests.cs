using ApplicationLayer.DTOs;
using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using FakeItEasy;
using Xunit;

namespace SouqTests
{
    public class ItemsServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ItemsService _service;

        public ItemsServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _service = new ItemsService(_unitOfWork);
        }

        [Fact]
        public async Task GetItem_ShouldReturnItem()
        {
            var item = new AirConditioner
            {
                ID = 1,
                Name = "AC"
            };

            A.CallTo(() => _unitOfWork.Items.FindItemByID(1))
                .Returns(item);

            var result = await _service.GetItem(1);

            Assert.Same(item, result);

            A.CallTo(() => _unitOfWork.Items.FindItemByID(1))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetItems_ShouldReturnItems()
        {
            var items = new List<Item>
        {
            new AirConditioner { ID = 1 },
            new Fridge { ID = 2 }
        };

            A.CallTo(() => _unitOfWork.Items.GetAllItems(1, 10))
                .Returns(items);

            var result = await _service.GetItems(1, 10);

            Assert.Equal(2, result.Count());

            A.CallTo(() => _unitOfWork.Items.GetAllItems(1, 10))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetFilteredItems_AirConditioners_ShouldReturnFilteredItems()
        {
            var airConditioners = new List<AirConditioner>
        {
            new AirConditioner { ID = 1 },
            new AirConditioner { ID = 2 }
        };

            A.CallTo(() => _unitOfWork.AirConditioners.GetAll())
                .Returns(airConditioners);

            var result = await _service.GetFilteredItems(
                new List<string> { "Air Conditioners" },
                1,
                10);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetFilteredItems_Cookers_ShouldReturnFilteredItems()
        {
            var cookers = new List<Cooker>
        {
            new Cooker { ID = 1 },
            new Cooker { ID = 2 }
        };

            A.CallTo(() => _unitOfWork.Cookers.GetAll())
                .Returns(cookers);

            var result = await _service.GetFilteredItems(
                new List<string> { "Cookers" },
                1,
                10);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetFilteredItems_Fridges_ShouldReturnFilteredItems()
        {
            var fridges = new List<Fridge>
        {
            new Fridge { ID = 1 },
            new Fridge { ID = 2 }
        };

            A.CallTo(() => _unitOfWork.Fridges.GetAll())
                .Returns(fridges);

            var result = await _service.GetFilteredItems(
                new List<string> { "Fridges" },
                1,
                10);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetFilteredItems_WashingMachines_ShouldReturnFilteredItems()
        {
            var washingMachines = new List<WashingMachine>
        {
            new WashingMachine { ID = 1 },
            new WashingMachine { ID = 2 }
        };

            A.CallTo(() => _unitOfWork.WashingMachines.GetAll())
                .Returns(washingMachines);

            var result = await _service.GetFilteredItems(
                new List<string> { "Washing Machines" },
                1,
                10);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetFilteredItems_Laptops_ShouldReturnFilteredItems()
        {
            var laptops = new List<Laptop>
        {
            new Laptop { ID = 1 },
            new Laptop { ID = 2 }
        };

            A.CallTo(() => _unitOfWork.Laptops.GetAll())
                .Returns(laptops);

            var result = await _service.GetFilteredItems(
                new List<string> { "Laptops" },
                1,
                10);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetFilteredItems_TVs_ShouldReturnFilteredItems()
        {
            var tvs = new List<TV>
        {
            new TV { ID = 1 },
            new TV { ID = 2 }
        };

            A.CallTo(() => _unitOfWork.TVs.GetAll())
                .Returns(tvs);

            var result = await _service.GetFilteredItems(
                new List<string> { "TVs" },
                1,
                10);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetFilteredItems_Headphones_ShouldReturnFilteredItems()
        {
            var headphones = new List<HeadPhone>
        {
            new HeadPhone { ID = 1 },
            new HeadPhone { ID = 2 }
        };

            A.CallTo(() => _unitOfWork.HeadPhones.GetAll())
                .Returns(headphones);

            var result = await _service.GetFilteredItems(
                new List<string> { "Headphones" },
                1,
                10);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetFilteredItems_UnknownFilter_ShouldReturnEmpty()
        {
            var result = await _service.GetFilteredItems(
                new List<string> { "Unknown" },
                1,
                10);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFilteredItems_ShouldApplyPagination()
        {
            var laptops = new List<Laptop>
        {
            new Laptop { ID = 1 },
            new Laptop { ID = 2 },
            new Laptop { ID = 3 },
            new Laptop { ID = 4 },
            new Laptop { ID = 5 }
        };

            A.CallTo(() => _unitOfWork.Laptops.GetAll())
                .Returns(laptops);

            var result = await _service.GetFilteredItems(
                new List<string> { "Laptops" },
                2,
                2);

            Assert.Equal(2, result.Count());
            Assert.Equal(3, result.First().ID);
            Assert.Equal(4, result.Last().ID);
        }

        [Fact]
        public void GetItemCategories_ShouldReturnCategories()
        {
            var categoryIds = new List<int> { 1, 2 };

            var categories = new List<Category>
        {
            new Category { ID = 1, Name = "AC" },
            new Category { ID = 2, Name = "Fridges" }
        };

            A.CallTo(() => _unitOfWork.Categories
                .GetCategoryIDsFromExpression<AirConditioner>())
                .Returns(categoryIds);

            A.CallTo(() => _unitOfWork.Categories
                .GetAllByIDs(categoryIds))
                .Returns(categories.AsQueryable());

            var result = _service.GetItemCategories<AirConditioner>();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetItemCategories_NoCategories_ShouldReturnEmpty()
        {
            var categoryIds = new List<int>();

            A.CallTo(() => _unitOfWork.Categories
                .GetCategoryIDsFromExpression<AirConditioner>())
                .Returns(categoryIds);

            A.CallTo(() => _unitOfWork.Categories
                .GetAllByIDs(categoryIds))
                .Returns(Enumerable.Empty<Category>().AsQueryable());

            var result = _service.GetItemCategories<AirConditioner>();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetItemsCategories_ShouldReturnCategories()
        {
            var department = new Department
            {
                ID = 1,
                Name = "Appliances"
            };

            var categoryDepartments = new List<CategoryDepartments>
        {
            new CategoryDepartments { CategoryId = 10 },
            new CategoryDepartments { CategoryId = 20 }
        };

            var categories = new List<Category>
        {
            new Category { ID = 10, Name = "AC" },
            new Category { ID = 20, Name = "Fridges" }
        };

            A.CallTo(() => _unitOfWork.Departments.GetByName("Appliances"))
                .Returns(department);

            A.CallTo(() => _unitOfWork.CategoryDepartments
                .GetAllCategoryDepartmentsWithDepartmentID(1))
                .Returns(categoryDepartments.AsQueryable());

            A.CallTo(() => _unitOfWork.Categories.GetAllByIDs(
                A<IEnumerable<int>>._))
                .Returns(categories.AsQueryable());

            var result = await _service.GetItemsCategories("Appliances");

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetItemsCategories_EmptyKey_ShouldThrowArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.GetItemsCategories(""));
        }

        [Fact]
        public async Task GetItemsCategories_NullKey_ShouldThrowArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.GetItemsCategories(null));
        }

        [Fact]
        public async Task GetAllItemsWithSort_ShouldReturnCorrectPagination()
        {
            var items = new List<Item>
        {
            new AirConditioner { ID = 1 },
            new AirConditioner { ID = 2 },
            new AirConditioner { ID = 3 }
        };

            A.CallTo(() => _unitOfWork.Items.GetAllItems(1, int.MaxValue))
                .Returns(items);

            A.CallTo(() => _unitOfWork.Items.SortItems(
                    items,
                    "ID",
                    false))
                .Returns(items);

            var result = await _service.GetAllItemsWithSort(
                1,
                null,
                false);

            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal("ID", result.OrderIndex);
            Assert.False(result.Des);
            Assert.Equal(3, result.Items.Count());
        }
        
        [Fact]
        public async Task GetAllItemsWithFilter_ShouldReturnProducts()
        {
            var items = new List<Laptop>
        {
            new Laptop { ID = 1 },
            new Laptop { ID = 2 }
        };

            var data = new ProductsDTO
            {
                SelectedFilters = new List<string> { "Laptops" }
            };

            A.CallTo(() => _unitOfWork.Laptops.GetAll())
                .Returns(items);

            var result = await _service.GetAllItemsWithFilter(
                data,
                null,
                null);

            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal("ID", result.OrderIndex);
            Assert.False(result.Des);
            Assert.Equal(2, result.Items.Count());
        }

        [Fact]
        public void GetLatestItems_Descending_ShouldCallDescendingRepository()
        {
            var query = new List<AirConditioner>
        {
            new AirConditioner { ID = 1 }
        }.AsQueryable();

            A.CallTo(() => _unitOfWork.Items
                .GetLatestItemsDesOrder<AirConditioner>(
                    1, 10, "ID"))
                .Returns(query);

            var result = _service.GetLatestItems<AirConditioner>(
                1, 10, "ID", true);

            Assert.Single(result);

            A.CallTo(() => _unitOfWork.Items
                .GetLatestItemsDesOrder<AirConditioner>(
                    1, 10, "ID"))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void GetLatestItems_Ascending_ShouldCallAscendingRepository()
        {
            var query = new List<AirConditioner>
        {
            new AirConditioner { ID = 1 }
        }.AsQueryable();

            A.CallTo(() => _unitOfWork.Items
                .GetLatestItemsAesOrder<AirConditioner>(
                    1, 10, "ID"))
                .Returns(query);

            var result = _service.GetLatestItems<AirConditioner>(
                1, 10, "ID", false);

            Assert.Single(result);

            A.CallTo(() => _unitOfWork.Items
                .GetLatestItemsAesOrder<AirConditioner>(
                    1, 10, "ID"))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetItemComments_ItemDoesNotExist_ShouldReturnEmpty()
        {
            A.CallTo(() => _unitOfWork.Items.GetById(1))
                .Returns((Item)null);

            var result = await _service.GetItemComments(
                1,
                "AirConditioners",
                "All");

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetItemComments_Default_ShouldReturnMaximumTwo()
        {
            var item = new AirConditioner
            {
                ID = 1
            };

            var comments = new List<Comment>
        {
            new Comment
            {
                ItemId = 1,
                ItemType = "AirConditioners",
                CommentTime = DateTime.Now.AddMinutes(-3)
            },
            new Comment
            {
                ItemId = 1,
                ItemType = "AirConditioners",
                CommentTime = DateTime.Now.AddMinutes(-2)
            },
            new Comment
            {
                ItemId = 1,
                ItemType = "AirConditioners",
                CommentTime = DateTime.Now
            }
        };

            A.CallTo(() => _unitOfWork.Items.GetById(1))
                .Returns(item);

            A.CallTo(() => _unitOfWork.Comments.GetAllComments())
                .Returns(comments);

            var result = await _service.GetItemComments(
                1,
                "AirConditioners",
                "Default");

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetItemComments_All_ShouldReturnAllMatchingComments()
        {
            var item = new AirConditioner
            {
                ID = 1
            };

            var comments = new List<Comment>
        {
            new Comment
            {
                ItemId = 1,
                ItemType = "AirConditioners"
            },
            new Comment
            {
                ItemId = 1,
                ItemType = "AirConditioners"
            },
            new Comment
            {
                ItemId = 2,
                ItemType = "AirConditioners"
            }
        };

            A.CallTo(() => _unitOfWork.Items.GetById(1))
                .Returns(item);

            A.CallTo(() => _unitOfWork.Comments.GetAllComments())
                .Returns(comments);

            var result = await _service.GetItemComments(
                1,
                "AirConditioners",
                "All");

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetItemRates_ItemDoesNotExist_ShouldReturnEmpty()
        {
            A.CallTo(() => _unitOfWork.Items.GetById(1))
                .Returns((Item)null);

            var result = await _service.GetItemRates(
                1,
                "AirConditioners");

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetItemRates_ShouldReturnMatchingRates()
        {
            var item = new AirConditioner
            {
                ID = 1
            };

            var rates = new List<Rate>
        {
            new Rate
            {
                ItemId = 1,
                ItemType = "AirConditioners",
                Value = 5
            },
            new Rate
            {
                ItemId = 1,
                ItemType = "AirConditioners",
                Value = 4
            },
            new Rate
            {
                ItemId = 2,
                ItemType = "AirConditioners",
                Value = 3
            }
        };

            A.CallTo(() => _unitOfWork.Items.GetById(1))
                .Returns(item);

            A.CallTo(() => _unitOfWork.Rates.GetAll())
                .Returns(rates);

            var result = await _service.GetItemRates(
                1,
                "AirConditioners");

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task SetRate_ItemDoesNotExist_ShouldReturnFalse()
        {
            var rate = new Rate
            {
                ItemId = 1,
                ItemType = "AirConditioners",
                Value = 5
            };

            A.CallTo(() => _unitOfWork.Items.GetById(1))
                .Returns((Item)null);

            var result = await _service.SetRate(rate);

            Assert.False(result);
        }

        [Fact]
        public async Task SetRate_NoExistingRates_ShouldReturnFalse()
        {
            var item = new AirConditioner
            {
                ID = 1,
                Rate = 0
            };

            var rate = new Rate
            {
                ItemId = 1,
                ItemType = "AirConditioners",
                Value = 5
            };

            A.CallTo(() => _unitOfWork.Items.GetById(1))
                .Returns(item);

            A.CallTo(() => _unitOfWork.Rates.GetAll())
                .Returns(new List<Rate>());

            var result = await _service.SetRate(rate);

            Assert.False(result);
        }

        [Fact]
        public async Task SetRate_WithExistingRates_ShouldUpdateAverageRate()
        {
            var item = new AirConditioner
            {
                ID = 1,
                Rate = 0
            };

            var rate = new Rate
            {
                ItemId = 1,
                ItemType = "AirConditioners",
                Value = 5
            };

            var rates = new List<Rate>
        {
            new Rate
            {
                ItemId = 1,
                ItemType = "AirConditioners",
                Value = 4
            },
            new Rate
            {
                ItemId = 1,
                ItemType = "AirConditioners",
                Value = 2
            }
        };

            A.CallTo(() => _unitOfWork.Items.GetById(1))
                .Returns(item);

            A.CallTo(() => _unitOfWork.Rates.GetAll())
                .Returns(rates);

            A.CallTo(() => _unitOfWork.Items.Update(item))
                .Returns(item);

            var result = await _service.SetRate(rate);

            Assert.True(result);
            Assert.Equal(3, item.Rate);

            A.CallTo(() => _unitOfWork.Items.Update(item))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public void SortItems_ShouldCallRepository()
        {
            var items = new List<Item>
        {
            new AirConditioner { ID = 1 }
        };

            A.CallTo(() => _unitOfWork.Items.SortItems(
                items,
                "Price",
                true))
                .Returns(items);

            var result = _service.SortItems(
                items,
                "Price",
                true);

            Assert.Same(items, result);

            A.CallTo(() => _unitOfWork.Items.SortItems(
                items,
                "Price",
                true))
                .MustHaveHappenedOnceExactly();
        }
    }
}