using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using FakeItEasy;
using Souq.Models.Cart_Orders;
using X.PagedList.Extensions;
using Xunit;

namespace SouqTests
{
    public class OrdersServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        private readonly IOrdersRepository _ordersRepository;
        private readonly ICartService _cartService;

        private readonly OrdersService _service;

        public OrdersServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userService = A.Fake<IUsersService>();
            _servicesInstanceProvider = A.Fake<IServicesInstanceProvider>();

            _ordersRepository = A.Fake<IOrdersRepository>();
            _cartService = A.Fake<ICartService>();

            A.CallTo(() => _unitOfWork.Orders)
                .Returns(_ordersRepository);

            _service = new OrdersService(
                _unitOfWork,
                _userService,
                _servicesInstanceProvider);
        }
        
        [Fact]
        public async Task AllOrders_ShouldReturnAllOrders()
        {
            // Arrange

            var orders = new List<Order>
        {
            new Order { ID = 1 },
            new Order { ID = 2 }
        };

            A.CallTo(() => _ordersRepository.GetAllOrders())
                .Returns(orders);

            // Act

            var result = await _service.AllOrders();

            // Assert

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            A.CallTo(() => _ordersRepository.GetAllOrders())
                .MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public async Task AllOrders_WithPage_ShouldReturnPagedOrders()
        {
            // Arrange

            var orders = new List<Order>
        {
            new Order
            {
                ID = 1,
                User = new AppUser
                {
                    UserName = "Bassam"
                },
                OrderDetails = new List<OrderDetails>
                {
                    new OrderDetails
                    {
                        Price = 100,
                        Quantity = 2
                    }
                },
                PaymentMethod = "COD",
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            }
        };

            A.CallTo(() => _ordersRepository.GetAllOrders())
                .Returns(orders);

            A.CallTo(() => _ordersRepository.GetAllOrders(1, 10))
                .Returns(orders);

            // Act

            var result = await _service.AllOrders(1);

            // Assert

            Assert.NotNull(result);

            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);

            Assert.Single(result.Orders);

            var order = result.Orders.First();

            Assert.Equal(1, order.Id);
            Assert.Equal("Bassam", order.UserName);

            // 100 * 2 = 200
            Assert.Equal(200, order.TotalPrice);

            A.CallTo(() => _ordersRepository.GetAllOrders())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _ordersRepository.GetAllOrders(1, 10))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AllOrders_WithFixedDiscount_ShouldCalculateCorrectTotal()
        {
            // Arrange

            var orders = new List<Order>
        {
            new Order
            {
                ID = 1,

                User = new AppUser
                {
                    UserName = "Bassam"
                },

                OrderDetails = new List<OrderDetails>
                {
                    new OrderDetails
                    {
                        Price = 100,
                        Quantity = 2
                    }
                },

                PromoCodeDiscountType = "fixed",
                PromoCodeDiscountValue = 30
            }
        };

            A.CallTo(() => _ordersRepository.GetAllOrders())
                .Returns(orders);

            A.CallTo(() => _ordersRepository.GetAllOrders(1, 10))
                .Returns(orders);

            // Act

            var result = await _service.AllOrders(1);

            // Assert

            // 200 - 30 = 170

            Assert.Equal(170, result.Orders.First().TotalPrice);
        }

        [Fact]
        public async Task AllOrders_WithPercentageDiscount_ShouldCalculateCorrectTotal()
        {
            // Arrange

            var orders = new List<Order>
        {
            new Order
            {
                ID = 1,

                User = new AppUser
                {
                    UserName = "Bassam"
                },

                OrderDetails = new List<OrderDetails>
                {
                    new OrderDetails
                    {
                        Price = 200,
                        Quantity = 1
                    }
                },

                PromoCodeDiscountType = "percentage",
                PromoCodeDiscountValue = 10
            }
        };

            A.CallTo(() => _ordersRepository.GetAllOrders())
                .Returns(orders);

            A.CallTo(() => _ordersRepository.GetAllOrders(1, 10))
                .Returns(orders);

            // Act

            var result = await _service.AllOrders(1);

            // Assert

            // 200 * (1 - 10 / 100) = 180

            Assert.Equal(180, result.Orders.First().TotalPrice);
        }
    
        [Fact]
        public void UserOrders_ShouldReturnUserOrders()
        {
            // Arrange

            var userId = "user-1";

            var orders = new List<Order>
        {
            new Order
            {
                ID = 10,

                User = new AppUser
                {
                    UserName = "Bassam"
                },

                OrderDetails = new List<OrderDetails>
                {
                    new OrderDetails
                    {
                        Price = 100,
                        Quantity = 2
                    }
                },

                PaymentMethod = "COD",
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            }
        };

            A.CallTo(() => _ordersRepository.GetUserOrders(
                    1,
                    int.MaxValue,
                    userId))
                .Returns(orders.AsQueryable());

            A.CallTo(() => _ordersRepository.GetUserOrders(
                    1,
                    10,
                    userId))
                .Returns(orders.AsQueryable());

            // Act

            var result = _service.UserOrders(1, userId);

            // Assert

            Assert.NotNull(result);

            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);

            Assert.Single(result.Orders);

            var order = result.Orders.First();

            Assert.Equal(10, order.Id);
            Assert.Equal("Bassam", order.UserName);

            // 100 * 2 = 200

            Assert.Equal(200, order.TotalPrice);

            A.CallTo(() => _ordersRepository.GetUserOrders(
                    1,
                    int.MaxValue,
                    userId))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _ordersRepository.GetUserOrders(
                    1,
                    10,
                    userId))
                .MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public async Task CreateOrder_ShouldCreateOrder()
        {
            // Arrange

            var userId = "user-1";

            var details = new List<OrderDetails>
        {
            new OrderDetails
            {
                ItemID = 1,
                ItemType = "Laptops",
                Price = 100,
                Quantity = 2
            }
        };

            A.CallTo(() => _ordersRepository.GetUserPendingOrder(userId))
                .Returns((Order)null);

            A.CallTo(() => _ordersRepository.Add(A<Order>._))
                .ReturnsLazily((Order order) =>
                {
                    order.ID = 10;
                    return order;
                });

            A.CallTo(() => _ordersRepository.AddOrderDetails(A<OrderDetails>._))
                .ReturnsLazily((OrderDetails detail) => detail);

            // Act

            var result = await _service.CreateOrder(userId, details);

            // Assert

            Assert.NotNull(result);

            Assert.Equal(10, result.ID);
            Assert.Equal(userId, result.UserID);
            Assert.Equal(OrderStatus.Pending, result.Status);

            A.CallTo(() => _ordersRepository.Add(A<Order>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _ordersRepository.AddOrderDetails(A<OrderDetails>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappened();
        }

        [Fact]
        public async Task CreateOrder_WhenPreviousPendingOrderExists_ShouldDeleteIt()
        {
            // Arrange

            var userId = "user-1";

            var previousOrder = new Order
            {
                ID = 5,
                UserID = userId,
                Status = OrderStatus.Pending
            };

            A.CallTo(() => _ordersRepository.GetUserPendingOrder(userId))
                .Returns(previousOrder);

            A.CallTo(() => _ordersRepository.Add(A<Order>._))
                .ReturnsLazily((Order order) =>
                {
                    order.ID = 10;
                    return order;
                });

            // Act

            var result = await _service.CreateOrder(
                userId,
                Enumerable.Empty<OrderDetails>());

            // Assert

            Assert.NotNull(result);

            A.CallTo(() => _ordersRepository.Delete(previousOrder))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _ordersRepository.Add(A<Order>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappened();
        }

        [Fact]
        public async Task CreateOrder_WithNullUserId_ShouldThrowException()
        {
            // Arrange

            var data = new List<OrderDetails>();

            // Act & Assert

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateOrder(null, data));
        }
        
        [Fact]
        public async Task SetOrderPaymentMethodAndStatus_ShouldUpdateOrder()
        {
            // Arrange

            var userId = "user-1";

            var order = new Order
            {
                ID = 10,
                UserID = userId,
                Status = OrderStatus.Pending
            };

            A.CallTo(() => _userService.GetUserId())
                .Returns(userId);

            A.CallTo(() => _ordersRepository.GetUserPendingOrder(userId))
                .Returns(order);

            // Act

            await _service.SetOrderPaymentMethodAndStatus(
                10,
                "COD",
                3);

            // Assert

            Assert.Equal("COD", order.PaymentMethod);
            Assert.Equal((OrderStatus)3, order.Status);

            A.CallTo(() => _ordersRepository.GetUserPendingOrder(userId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SetOrderPaymentMethodAndStatus_WhenOrderDoesNotExist_ShouldThrowException()
        {
            // Arrange

            var userId = "user-1";

            A.CallTo(() => _userService.GetUserId())
                .Returns(userId);

            A.CallTo(() => _ordersRepository.GetUserPendingOrder(userId))
                .Returns((Order)null);

            // Act & Assert

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.SetOrderPaymentMethodAndStatus(
                    10,
                    "COD",
                    3));
        }
   
        [Fact]
        public async Task GetUserCurrentOrder_ShouldReturnPendingOrder()
        {
            // Arrange

            var userId = "user-1";

            var order = new Order
            {
                ID = 10,
                UserID = userId,
                Status = OrderStatus.Pending
            };

            A.CallTo(() => _ordersRepository.GetUserPendingOrderWithDetails(userId))
                .Returns(order);

            // Act

            var result = await _service.GetUserCurrentOrder(userId);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(10, result.ID);
            Assert.Equal(userId, result.UserID);

            A.CallTo(() => _ordersRepository.GetUserPendingOrderWithDetails(userId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetUserCurrentOrder_WhenOrderDoesNotExist_ShouldReturnNull()
        {
            // Arrange

            var userId = "user-1";

            A.CallTo(() => _ordersRepository.GetUserPendingOrderWithDetails(userId))
                .Returns((Order)null);

            // Act

            var result = await _service.GetUserCurrentOrder(userId);

            // Assert

            Assert.Null(result);
        }
        
        [Fact]
        public async Task GetUserCurrentOrderOrCreateIt_WhenOrderExists_ShouldReturnExistingOrder()
        {
            // Arrange

            var userId = "user-1";

            var order = new Order
            {
                ID = 20,
                UserID = userId,
                Status = OrderStatus.Pending
            };

            A.CallTo(() => _ordersRepository.GetUserPendingOrderWithDetails(userId))
                .Returns(order);

            // Act

            var result =
                await _service.GetUserCurrentOrderOrCreateIt(userId);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(20, result.ID);

            A.CallTo(() => _servicesInstanceProvider
                .GetCartServiceInstance())
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task GetUserCurrentOrderOrCreateIt_WhenOrderDoesNotExist_ShouldCreateOrderFromCart()
        {
            // Arrange

            var userId = "user-1";

            var cartItems = new List<RepositoryCartDTO>
        {
            new RepositoryCartDTO
            {
                ItemId = 1,
                ItemType = "Laptops",
                Price = 500,
                Quantity = 2
            }
        };

            A.CallTo(() => _ordersRepository
                    .GetUserPendingOrderWithDetails(userId))
                .Returns((Order)null);

            A.CallTo(() => _servicesInstanceProvider
                    .GetCartServiceInstance())
                .Returns(_cartService);

            A.CallTo(() => _cartService
                    .GetCartItems())
                .Returns(cartItems.ToPagedList());

            A.CallTo(() => _ordersRepository
                    .GetUserPendingOrder(userId))
                .Returns((Order)null);

            A.CallTo(() => _ordersRepository
                    .Add(A<Order>._))
                .ReturnsLazily((Order order) =>
                {
                    order.ID = 50;
                    return order;
                });

            A.CallTo(() => _ordersRepository
                    .AddOrderDetails(A<OrderDetails>._))
                .ReturnsLazily((OrderDetails detail) => detail);

            // Act

            var result =
                await _service.GetUserCurrentOrderOrCreateIt(userId);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(50, result.ID);
            Assert.Equal(userId, result.UserID);
            Assert.Equal(OrderStatus.Pending, result.Status);

            A.CallTo(() => _cartService
                    .GetCartItems())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _ordersRepository
                    .Add(A<Order>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _ordersRepository
                    .AddOrderDetails(A<OrderDetails>._))
                .MustHaveHappenedOnceExactly();
        }
    }
}
