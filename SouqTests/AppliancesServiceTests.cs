using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using FakeItEasy;
using Xunit;

namespace SouqTests
{
    public class AppliancesServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;
        private readonly AppliancesService _service;

        public AppliancesServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userService = A.Fake<IUsersService>();
            _servicesInstanceProvider = A.Fake<IServicesInstanceProvider>();

            _service = new AppliancesService(
              _unitOfWork,
              _userService,
              _servicesInstanceProvider);
        }

        [Fact]
        public async Task GetAllAppliances_ShouldReturnCategoriesAndOffers()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();
            var departmentsService = A.Fake<IDepartmentsService>();
            var offersService = A.Fake<IOffersService>();

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => _servicesInstanceProvider.GetDepartmentsServiceInstance())
                .Returns(departmentsService);

            A.CallTo(() => _servicesInstanceProvider.GetOffersServiceInstance())
                .Returns(offersService);

            var category1 = new Category { ID = 1, Name = "Samsung" };
            var category2 = new Category { ID = 2, Name = "LG" };
            var duplicateCategory = new Category { ID = 1, Name = "Samsung" };

            A.CallTo(() => itemsService.GetItemsCategories("Appliances"))
                .ReturnsNextFromSequence(
                    new List<Category> { category1 },
                    new List<Category> { category2 },
                    new List<Category> { duplicateCategory },
                    new List<Category> { category1 }
                );

            var department = new Department
            {
                Name = "Appliances"
            };

            A.CallTo(() => departmentsService.GetDepartment("Appliances"))
                .Returns(department);

            var offers = new List<Offer>();

            A.CallTo(() => offersService.GetOffers("Appliances", null, null))
                .Returns(offers.AsQueryable());

            // Act
            var result = await _service.GetAllAppliances();

            // Assert
            Assert.NotNull(result);

            Assert.Equal(2, result.Categories.Count());

            Assert.Contains(result.Categories, c => c.ID == 1);
            Assert.Contains(result.Categories, c => c.ID == 2);

            Assert.Equal(offers, result.Offers.ToList());

            A.CallTo(() => itemsService.GetItemsCategories("Appliances"))
                .MustHaveHappened(4, Times.Exactly);

            A.CallTo(() => departmentsService.GetDepartment("Appliances"))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => offersService.GetOffers("Appliances", null, null))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetAppliancesWithPriceFilter_ShouldReturnAllApplianceTypes()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();
            var wishlistService = A.Fake<IWishingListService>();

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => _servicesInstanceProvider.GetWishingListServiceInstance())
                .Returns(wishlistService);

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            int price1 = 10000;
            int price2 = 30000;

            var category = new Category
            {
                ID = 1,
                Name = "Samsung"
            };

            var airConditioner = new AirConditioner
            {
                ID = 1,
                Name = "Samsung AC",
                Price = 20000,
                Category = category
            };

            var fridge = new Fridge
            {
                ID = 2,
                Name = "Samsung Fridge",
                Price = 25000,
                Category = category
            };

            var cooker = new Cooker
            {
                ID = 3,
                Name = "Samsung Cooker",
                Price = 15000,
                Category = category
            };

            var washingMachine = new WashingMachine
            {
                ID = 4,
                Name = "Samsung Washing Machine",
                Price = 22000,
                Category = category
            };

            // Total counts
            A.CallTo(() => itemsService.TotalItems<AirConditioner>(
                "Price", price1, price2, null))
                .Returns(9);

            A.CallTo(() => itemsService.TotalItems<Fridge>(
                "Price", price1, price2, null))
                .Returns(18);

            A.CallTo(() => itemsService.TotalItems<Cooker>(
                "Price", price1, price2, null))
                .Returns(9);

            A.CallTo(() => itemsService.TotalItems<WashingMachine>(
                "Price", price1, price2, null))
                .Returns(0);

            // Filtered items
            A.CallTo(() => itemsService.GetItemsFilteredByPrice<AirConditioner>(
                price1, price2, 1, 3, "ID", false))
                .Returns(new[] { airConditioner }.AsQueryable());

            A.CallTo(() => itemsService.GetItemsFilteredByPrice<Fridge>(
                price1, price2, 1, 3, "ID", false))
                .Returns(new[] { fridge }.AsQueryable());

            A.CallTo(() => itemsService.GetItemsFilteredByPrice<Cooker>(
                price1, price2, 1, 3, "ID", false))
                .Returns(new[] { cooker }.AsQueryable());

            A.CallTo(() => itemsService.GetItemsFilteredByPrice<WashingMachine>(
                price1, price2, 1, 3, "ID", false))
                .Returns(new[] { washingMachine }.AsQueryable());

            // Act
            var result = await _service.GetAppliancesWithPriceFilter(
                null, null, price1, price2, null);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(1, result.CurrentPage);

            Assert.Equal(4, result.TotalPages);

            Assert.Equal("PriceFilter", result.ActionName);

            Assert.Equal(price1, result.Price1);
            Assert.Equal(price2, result.Price2);

            Assert.Equal(4, result.Items.Count());

            Assert.Contains(result.Items, x => x is AirConditionerDTO);
            Assert.Contains(result.Items, x => x is FridgeDTO);
            Assert.Contains(result.Items, x => x is CookerDTO);
            Assert.Contains(result.Items, x => x is WashingMachineDTO);
        }

        [Fact]
        public async Task GetBrandsAppliances_ShouldUseDefaultValues_WhenOptionalParametersAreNull()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();
            var wishlistService = A.Fake<IWishingListService>();

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => _servicesInstanceProvider.GetWishingListServiceInstance())
                .Returns(wishlistService);

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            A.CallTo(() => itemsService.TotalItems<AirConditioner>(
                "Brands", null, null, "Samsung"))
                .Returns(0);

            A.CallTo(() => itemsService.TotalItems<Fridge>(
                "Brands", null, null, "Samsung"))
                .Returns(0);

            A.CallTo(() => itemsService.TotalItems<Cooker>(
                "Brands", null, null, "Samsung"))
                .Returns(0);

            A.CallTo(() => itemsService.TotalItems<WashingMachine>(
                "Brands", null, null, "Samsung"))
                .Returns(0);

            A.CallTo(() => itemsService.GetCategoryItems<AirConditioner>(
                "Samsung", 1, 3, "ID", false))
                .Returns(Enumerable.Empty<AirConditioner>().AsQueryable());

            A.CallTo(() => itemsService.GetCategoryItems<Fridge>(
                "Samsung", 1, 3, "ID", false))
                .Returns(Enumerable.Empty<Fridge>().AsQueryable());

            A.CallTo(() => itemsService.GetCategoryItems<Cooker>(
                "Samsung", 1, 3, "ID", false))
                .Returns(Enumerable.Empty<Cooker>().AsQueryable());

            A.CallTo(() => itemsService.GetCategoryItems<WashingMachine>(
                "Samsung", 1, 3, "ID", false))
                .Returns(Enumerable.Empty<WashingMachine>().AsQueryable());

            // Act
            var result = await _service.GetBrandsAppliances(
                null, null, "Samsung", null);

            // Assert
            Assert.Equal(1, result.CurrentPage);

            A.CallTo(() => itemsService.GetCategoryItems<AirConditioner>(
                "Samsung", 1, 3, "ID", false))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => itemsService.GetCategoryItems<Fridge>(
                "Samsung", 1, 3, "ID", false))
                .MustHaveHappenedOnceExactly();
        }
    }
}
