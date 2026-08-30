using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using FakeItEasy;
using Xunit;

namespace SouqTests
{
    public class ElectronicsServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;
        private readonly ElectronicsService _service;

        public ElectronicsServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userService = A.Fake<IUsersService>();
            _servicesInstanceProvider = A.Fake<IServicesInstanceProvider>();

            _service = new ElectronicsService(
              _unitOfWork,
              _userService,
              _servicesInstanceProvider);
        }

        [Fact]
        public async Task GetAllElectronics_ShouldReturnCategoriesAndOffers()
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

            var categories = new List<Category>
    {
        new Category { ID = 1, Name = "Samsung" },
        new Category { ID = 2, Name = "LG" }
    };

            A.CallTo(() => itemsService.GetItemsCategories("Electronics"))
                .Returns(categories);

            var department = new Department
            {
                Name = "Electronics"
            };

            A.CallTo(() => departmentsService.GetDepartment("Electronics"))
                .Returns(department);

            var offers = new List<Offer>();

            A.CallTo(() => offersService.GetOffers("Electronics", null, null))
                .Returns(offers.AsQueryable());

            // Act
            var result = await _service.GetAllElectronics();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Categories.Count());
            Assert.Equal(offers, result.Offers.ToList());
        }

        [Fact]
        public async Task GetBrandsElectronics_ShouldReturnAllElectronicTypes()
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

            var category = new Category
            {
                ID = 1,
                Name = "Samsung"
            };

            var laptop = new Laptop
            {
                ID = 1,
                Name = "Samsung Laptop",
                Price = 30000,
                Category = category
            };

            var tv = new TV
            {
                ID = 2,
                Name = "Samsung TV",
                Price = 20000,
                Category = category
            };

            var headphone = new HeadPhone
            {
                ID = 3,
                Name = "Samsung Headphone",
                Price = 5000,
                Category = category
            };

            // Total pages
            A.CallTo(() => itemsService.TotalItems<Laptop>(
                "Brands", null, null, "Samsung")).Returns(9);

            A.CallTo(() => itemsService.TotalItems<TV>(
                "Brands", null, null, "Samsung")).Returns(9);

            A.CallTo(() => itemsService.TotalItems<HeadPhone>(
                "Brands", null, null, "Samsung")).Returns(9);

            // Items
            A.CallTo(() => itemsService.GetCategoryItems<Laptop>(
                "Samsung", 1, 3, "ID", false))
                .Returns(new[] { laptop }.AsQueryable());

            A.CallTo(() => itemsService.GetCategoryItems<TV>(
                "Samsung", 1, 3, "ID", false))
                .Returns(new[] { tv }.AsQueryable());

            A.CallTo(() => itemsService.GetCategoryItems<HeadPhone>(
                "Samsung", 1, 3, "ID", false))
                .Returns(new[] { headphone }.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, A<int>._, A<string>._))
                .Returns(false);

            // Act
            var result = await _service.GetBrandsElectronics(
                null, null, "Samsung", null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(3, result.TotalPages);
            Assert.Equal("Brands", result.ActionName);
            Assert.Equal("Samsung", result.Brand);

            Assert.Equal(3, result.Items.Count());

            Assert.Contains(result.Items, x => x is LaptopDTO);
            Assert.Contains(result.Items, x => x is TVDTO);
            Assert.Contains(result.Items, x => x is HeadPhoneDTO);
        }

        [Fact]
        public async Task GetElectronicsWithPriceFilter_ShouldReturnAllElectronicTypes()
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

            int price1 = 1000;
            int price2 = 30000;

            var category = new Category
            {
                ID = 1,
                Name = "Samsung"
            };

            var laptop = new Laptop
            {
                ID = 1,
                Name = "Laptop",
                Price = 20000,
                Category = category
            };

            var tv = new TV
            {
                ID = 2,
                Name = "TV",
                Price = 15000,
                Category = category
            };

            var headphone = new HeadPhone
            {
                ID = 3,
                Name = "Headphone",
                Price = 5000,
                Category = category
            };

            A.CallTo(() => itemsService.TotalItems<Laptop>(
                "Price", price1, price2, null)).Returns(9);

            A.CallTo(() => itemsService.TotalItems<TV>(
                "Price", price1, price2, null)).Returns(9);

            A.CallTo(() => itemsService.TotalItems<HeadPhone>(
                "Price", price1, price2, null)).Returns(9);

            A.CallTo(() => itemsService.GetItemsFilteredByPrice<Laptop>(
                price1, price2, 1, 3, "ID", false))
                .Returns(new[] { laptop }.AsQueryable());

            A.CallTo(() => itemsService.GetItemsFilteredByPrice<TV>(
                price1, price2, 1, 3, "ID", false))
                .Returns(new[] { tv }.AsQueryable());

            A.CallTo(() => itemsService.GetItemsFilteredByPrice<HeadPhone>(
                price1, price2, 1, 3, "ID", false))
                .Returns(new[] { headphone }.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, A<int>._, A<string>._))
                .Returns(false);

            // Act
            var result = await _service.GetElectronicsWithPriceFilter(
                null, null, price1, price2, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(3, result.TotalPages);

            Assert.Equal("PriceFilter", result.ActionName);
            Assert.Equal(price1, result.Price1);
            Assert.Equal(price2, result.Price2);

            Assert.Equal(3, result.Items.Count());

            Assert.Contains(result.Items, x => x is LaptopDTO);
            Assert.Contains(result.Items, x => x is TVDTO);
            Assert.Contains(result.Items, x => x is HeadPhoneDTO);
        }
    }
}
