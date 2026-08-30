using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Enums;
using DomainLayer.Models;
using FakeItEasy;
using Xunit;

namespace SouqTests
{
    public class HomePageServiceTests
    {
        private readonly IItemsService _items;
        private readonly IServicesInstanceProvider _provider;
        private readonly HomePageService _service;

        public HomePageServiceTests()
        {
            _items = A.Fake<IItemsService>();
            _provider = A.Fake<IServicesInstanceProvider>();

            _service = new HomePageService(_items, _provider);
        }

        [Fact]
        public async Task GetHomePageRelatedData_ShouldReturnData()
        {
            var departments = new List<Department>
    {
        new Department { ID = 1, Name = "Electronics" }
    };

            var items = new List<Item>
    {
        new Laptop { ID = 1, Name = "Item 1", AddedOn = DateTime.Now, Rate = 5 },
        new Fridge { ID = 2, Name = "Item 2", AddedOn = DateTime.Now.AddDays(-1), Rate = 4 }
    };

            var offers = new List<OfferDTO>
    {
        new OfferDTO { OfferType = OfferType.FixedDiscount },
        new OfferDTO { OfferType = OfferType.PromoCode }
    };

            var departmentsService = A.Fake<IDepartmentsService>();
            var offersService = A.Fake<IOffersService>();

            A.CallTo(() => _provider.GetDepartmentsServiceInstance())
                .Returns(departmentsService);

            A.CallTo(() => _provider.GetOffersServiceInstance())
                .Returns(offersService);

            A.CallTo(() => departmentsService.GetDepartments())
                .Returns(departments);

            A.CallTo(() => _items.GetItems(1, int.MaxValue))
                .Returns(items);

            A.CallTo(() => offersService.GetAllOffers())
                .Returns(offers);

            var result = await _service.GetHomePageRelatedData();

            Assert.NotNull(result);
            Assert.Single(result.Departments);
            Assert.Equal(2, result.Latest.Count());
            Assert.Equal(2, result.Featured.Count());
            Assert.Single(result.Offers);
        }

        [Fact]
        public async Task GetItemType_ShouldReturnItemType()
        {
            var item = new Laptop { ID = 1 };

            A.CallTo(() => _items.GetItem(1))
                .Returns(item);

            var result = await _service.GetItemType(1);

            Assert.Equal(nameof(Laptop), result);
        }

        [Fact]
        public async Task GetItemType_ShouldReturnNull_WhenItemDoesNotExist()
        {
            A.CallTo(() => _items.GetItem(1))
                .Returns((Item)null);

            var result = await _service.GetItemType(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllItems_ShouldFilterSortAndPaginate()
        {
            var category = new Category { ID = 1, Name = "Laptops" };

            var items = new List<Item>
    {
        new Laptop { ID = 1, Name = "A", Category = category },
        new Laptop { ID = 2, Name = "B", Category = category },
        new Laptop { ID = 3, Name = "C", Category = new Category { Name = "TVs" } }
    };

            A.CallTo(() => _items.GetItems(1, int.MaxValue))
                .Returns(items);

            A.CallTo(() => _items.SortItems(
                    A<IEnumerable<Item>>._,
                    "ID",
                    false))
                .Returns(items);

            var result = await _service.GetAllItems("Laptops", null, null);

            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(3, result.Items.Count());
            Assert.Equal("Laptops", result.Brand);
        }

        [Fact]
        public async Task GetFilteredItems_ShouldReturnItems_WhenNameMatches()
        {
            var items = new List<Item>
    {
        new Laptop { ID = 1, Name = "Dell Laptop" },
        new Laptop { ID = 2, Name = "HP Laptop" },
        new Laptop { ID = 3, Name = "Samsung TV" }
    };

            A.CallTo(() => _items.GetItems(1, int.MaxValue))
                .Returns(items);

            var result = await _service.GetFilteredItems("Laptop", null, null);

            Assert.Equal("Laptop", result.SearchPhrase);
            Assert.Equal(2, result.MatchedItems.Count());
            Assert.Equal(1, result.CurrentPage);
        }

        [Fact]
        public async Task GetFilteredItems_ShouldReturnDepartmentItems_WhenDepartmentMatches()
        {
            var items = new List<Item>();

            var departments = new List<Department>
    {
        new Department { ID = 1, Name = "Electronics" }
    };

            var departmentItems = new List<Item>
    {
        new Laptop { ID = 1, Name = "Dell" }
    };

            var departmentsService = A.Fake<IDepartmentsService>();

            A.CallTo(() => _provider.GetDepartmentsServiceInstance())
                .Returns(departmentsService);

            A.CallTo(() => _items.GetItems(1, int.MaxValue))
                .Returns(items);

            A.CallTo(() => departmentsService.GetDepartments())
                .Returns(departments);

            A.CallTo(() => departmentsService.GetDepartmentItems(departments[0]))
                .Returns(departmentItems);

            var result = await _service.GetFilteredItems("Electronics", null, null);

            Assert.Single(result.MatchedItems);
            Assert.Equal(1, result.MatchedItems.First().ID);
        }

        [Fact]
        public async Task GetFilteredItems_ShouldReturnCategoryItems_WhenCategoryMatches()
        {
            var items = new List<Item>();

            var departments = new List<Department>();

            var categories = new List<Category>
    {
        new Category { ID = 1, Name = "Laptops" }
    };

            var categoryItems = new List<Item>
    {
        new Laptop { ID = 1, Name = "Dell" }
    };

            var departmentsService = A.Fake<IDepartmentsService>();
            var categoriesService = A.Fake<ICategoriesService>();

            A.CallTo(() => _items.GetItems(1, int.MaxValue))
                .Returns(items);

            A.CallTo(() => _provider.GetDepartmentsServiceInstance())
                .Returns(departmentsService);

            A.CallTo(() => _provider.GetCategoriesServiceInstance())
                .Returns(categoriesService);

            A.CallTo(() => departmentsService.GetDepartments())
                .Returns(departments);

            A.CallTo(() => categoriesService.GetCategories())
                .Returns(categories);

            A.CallTo(() => categoriesService.GetCategoryItems(categories[0]))
                .Returns(categoryItems.AsQueryable());

            var result = await _service.GetFilteredItems("Laptops", null, null);

            Assert.Single(result.MatchedItems);
            Assert.Equal(1, result.MatchedItems.First().ID);
        }

        [Fact]
        public async Task GetFilteredItems_ShouldReturnEmpty_WhenNothingMatches()
        {
            A.CallTo(() => _items.GetItems(1, int.MaxValue))
                .Returns(new List<Item>());

            var departmentsService = A.Fake<IDepartmentsService>();
            var categoriesService = A.Fake<ICategoriesService>();

            A.CallTo(() => _provider.GetDepartmentsServiceInstance())
                .Returns(departmentsService);

            A.CallTo(() => _provider.GetCategoriesServiceInstance())
                .Returns(categoriesService);

            A.CallTo(() => departmentsService.GetDepartments())
                .Returns(new List<Department>());

            A.CallTo(() => categoriesService.GetCategories())
                .Returns(new List<Category>());

            var result = await _service.GetFilteredItems("Nothing", null, null);

            Assert.Empty(result.MatchedItems);
        }

        [Fact]
        public async Task GetHomePageOfferDetails_ShouldReturnIndex_WhenOfferDoesNotExist()
        {
            var offersService = A.Fake<IOffersService>();

            A.CallTo(() => _provider.GetOffersServiceInstance())
                .Returns(offersService);

            A.CallTo(() => offersService.GetOffer(1))
                .Returns((Offer)null);

            var result = await _service.GetHomePageOfferDetails(1);

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task GetHomePageOfferDetails_ShouldReturnDetails_ForBOGO()
        {
            var offersService = A.Fake<IOffersService>();

            var offer = new Offer
            {
                OfferType = OfferType.BuyOneGetOne,
                ItemOneID = 5
            };

            A.CallTo(() => _provider.GetOffersServiceInstance())
                .Returns(offersService);

            A.CallTo(() => offersService.GetOffer(1))
                .Returns(offer);

            A.CallTo(() => _items.GetItem(5))
                .Returns(new Laptop { ID = 5 });

            var result = await _service.GetHomePageOfferDetails(1);

            Assert.Equal("Details", result.ActionName);
            Assert.Equal("Laptops", result.ItemType);
            Assert.Equal(5, result.ItemOneID);
        }

        [Fact]
        public async Task GetHomePageOfferDetails_ShouldReturnDepartmentIndex()
        {
            var offersService = A.Fake<IOffersService>();

            var offer = new Offer
            {
                OfferType = OfferType.PercentDiscount,
                DepartmentName = "Mobile Phones"
            };

            A.CallTo(() => _provider.GetOffersServiceInstance())
                .Returns(offersService);

            A.CallTo(() => offersService.GetOffer(1))
                .Returns(offer);

            var result = await _service.GetHomePageOfferDetails(1);

            Assert.Equal("Index", result.ActionName);
            Assert.Equal("MobilePhones", result.ControllerName);
        }

        [Fact]
        public async Task GetHomePageOfferDetails_ShouldReturnItems_ForCategory()
        {
            var offersService = A.Fake<IOffersService>();

            var offer = new Offer
            {
                OfferType = OfferType.FixedDiscount,
                CategoryName = "Laptops"
            };

            A.CallTo(() => _provider.GetOffersServiceInstance())
                .Returns(offersService);

            A.CallTo(() => offersService.GetOffer(1))
                .Returns(offer);

            var result = await _service.GetHomePageOfferDetails(1);

            Assert.Equal("Items", result.ActionName);
            Assert.Equal("Laptops", result.CategoryName);
        }

        [Fact]
        public async Task GetHomePageOfferDetails_ShouldReturnDetails_ForItem()
        {
            var offersService = A.Fake<IOffersService>();

            var offer = new Offer
            {
                OfferType = OfferType.FixedDiscount,
                ItemID = 10
            };

            A.CallTo(() => _provider.GetOffersServiceInstance())
                .Returns(offersService);

            A.CallTo(() => offersService.GetOffer(1))
                .Returns(offer);

            A.CallTo(() => _items.GetItem(10))
                .Returns(new TV { ID = 10 });

            var result = await _service.GetHomePageOfferDetails(1);

            Assert.Equal("Details", result.ActionName);
            Assert.Equal("TVs", result.ItemType);
            Assert.Equal(10, result.ItemID);
        }

        [Fact]
        public async Task GetHomePageOfferDetails_ShouldReturnIndex_WhenNoTargetExists()
        {
            var offersService = A.Fake<IOffersService>();

            var offer = new Offer
            {
                OfferType = OfferType.FixedDiscount
            };

            A.CallTo(() => _provider.GetOffersServiceInstance())
                .Returns(offersService);

            A.CallTo(() => offersService.GetOffer(1))
                .Returns((Offer)null);

            var result = await _service.GetHomePageOfferDetails(1);

            Assert.Equal("Index", result.ActionName);
        }
    }
}
