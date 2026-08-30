using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Souq.Models.Cart_Orders;
using Xunit;

namespace SouqTests
{
    public class DashboardServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrdersService _ordersService;
        private readonly IServicesInstanceProvider _provider;
        private readonly UserManager<AppUser> _userManager;
        private readonly DashboardService _service;

        public DashboardServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _ordersService = A.Fake<IOrdersService>();
            _provider = A.Fake<IServicesInstanceProvider>();

            var store = A.Fake<IUserStore<AppUser>>();

            _userManager = new UserManager<AppUser>(
                store,
                null, null, null, null, null, null, null, null);

            _service = new DashboardService(
                _unitOfWork,
                _userManager,
                _provider);
        }

        [Fact]
        public async Task GetDashboardRelatedData_ShouldReturnCorrectData()
        {
            // Arrange

            var users = new List<AppUser>
    {
        new AppUser(),
        new AppUser(),
        new AppUser()
    };

            var orders = new List<Order>
    {
        new Order
        {
            PromoCodeDiscountType = "fixed",
            PromoCodeDiscountValue = 100,
            OrderDetails = new List<OrderDetails>
            {
                new OrderDetails { Price = 500, Quantity = 2 }
            }
        },

        new Order
        {
            PromoCodeDiscountType = "percent",
            PromoCodeDiscountValue = 10,
            OrderDetails = new List<OrderDetails>
            {
                new OrderDetails { Price = 1000, Quantity = 1 }
            }
        }
    };

            var products = new List<Item>
    {
        new Laptop { ID = 1 },
        new TV { ID = 2 },
        new HeadPhone { ID = 3 }
    };

            var ordersService = _ordersService;

            A.CallTo(() => _provider.GetOrdersServiceInstance())
                .Returns(ordersService);

            A.CallTo(() => ordersService.AllOrders())
                .Returns(orders);

            A.CallTo(() => _unitOfWork.Items.GetAll())
                .Returns(products);

            // Fake UserManager.Users
            var mockUserManager = A.Fake<UserManager<AppUser>>();

            A.CallTo(() => mockUserManager.Users)
                .Returns(users.AsQueryable());

            var service = new DashboardService(
                _unitOfWork,
                mockUserManager,
                _provider);

            // Act

            var result = await service.GetDashboardRelatedData();

            // Assert

            Assert.Equal(3, result.UsersCount);
            Assert.Equal(3, result.ProductsCount);
            Assert.Equal(2, result.OrdersCount);

            // Order 1: 500 * 2 - 100 = 900
            // Order 2: 1000 * (1 - 10/100) = 900
            // Total = 1800

            Assert.Equal(1800, result.TotalRevenue);
        }

        [Fact]
        public async Task GetDashboardRelatedData_ShouldReturnZeroRevenue_WhenNoOrders()
        {
            var users = new List<AppUser>
    {
        new AppUser()
    };

            var ordersService = A.Fake<IOrdersService>();

            A.CallTo(() => _provider.GetOrdersServiceInstance())
                .Returns(ordersService);

            A.CallTo(() => ordersService.AllOrders())
                .Returns(new List<Order>());

            A.CallTo(() => _unitOfWork.Items.GetAll())
                .Returns(new List<Item>());

            var mockUserManager = A.Fake<UserManager<AppUser>>();

            A.CallTo(() => mockUserManager.Users)
                .Returns(users.AsQueryable());

            var service = new DashboardService(
                _unitOfWork,
                mockUserManager,
                _provider);

            var result = await service.GetDashboardRelatedData();

            Assert.Equal(1, result.UsersCount);
            Assert.Equal(0, result.ProductsCount);
            Assert.Equal(0, result.OrdersCount);
            Assert.Equal(0, result.TotalRevenue);
        }
    }
}
