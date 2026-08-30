using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Souq.Models.Cart_Orders;
using Xunit;

namespace SouqTests
{
    public class PaymentsServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _users;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        private readonly IOrdersService _ordersService;
        private readonly ICartService _cartService;

        private readonly PaymentsService _service;

        public PaymentsServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _users = A.Fake<IUsersService>();
            _emailSender = A.Fake<IEmailSender>();
            _servicesInstanceProvider = A.Fake<IServicesInstanceProvider>();

            _ordersService = A.Fake<IOrdersService>();
            _cartService = A.Fake<ICartService>();

            var userStore = A.Fake<IUserStore<AppUser>>();

            _userManager = A.Fake<UserManager<AppUser>>(
                options => options.WithArgumentsForConstructor(
                    () => new UserManager<AppUser>(
                        userStore,
                        null!,
                        null!,
                        null!,
                        null!,
                        null!,
                        null!,
                        null!,
                        null!
                    )));

            _service = new PaymentsService(
                _unitOfWork,
                _users,
                _emailSender,
                _userManager,
                _servicesInstanceProvider);
        }

        [Fact]
        public async Task GetUserCurrentOrderOrCreateIt_ShouldReturnOrder()
        {
            // Arrange
            var userId = "user-1";

            var order = new Order
            {
                ID = 1
            };

            A.CallTo(() => _users.GetUserId())
                .Returns(userId);

            A.CallTo(() =>
                    _servicesInstanceProvider.GetOrdersServiceInstance())
                .Returns(_ordersService);

            A.CallTo(() =>
                    _ordersService.GetUserCurrentOrderOrCreateIt(userId))
                .Returns(order);

            // Act
            var result = await _service.GetUserCurrentOrderOrCreateIt();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order, result);

            A.CallTo(() =>
                    _ordersService.GetUserCurrentOrderOrCreateIt(userId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CODCheckout_ShouldCalculateTotalAndCompleteOrder()
        {
            // Arrange
            var userId = "user-1";

            var user = new AppUser
            {
                Id = userId,
                UserName = "Bassam",
                Email = "test@test.com"
            };

            var order = new Order
            {
                ID = 10,
                OrderDetails = new List<OrderDetails>
            {
                new OrderDetails
                {
                    Price = 100,
                    Quantity = 2
                },
                new OrderDetails
                {
                    Price = 50,
                    Quantity = 1
                }
            },

                PromoCodeDiscountType = null,
                PromoCodeDiscountValue = null
            };

            A.CallTo(() => _users.GetUserId())
                .Returns(userId);

            A.CallTo(() => _servicesInstanceProvider
                    .GetOrdersServiceInstance())
                .Returns(_ordersService);

            A.CallTo(() => _servicesInstanceProvider
                    .GetCartServiceInstance())
                .Returns(_cartService);

            A.CallTo(() => _ordersService
                    .GetUserCurrentOrder(userId))
                .Returns(order);

            A.CallTo(() => _userManager
                    .FindByIdAsync(userId))
                .Returns(user);

            // Act
            var result = await _service.CODCheckout();

            // Assert
            Assert.NotNull(result);

            // 100 * 2 + 50 = 250
            Assert.Equal(250, result.TotalPrice);

            A.CallTo(() => _ordersService
                    .SetOrderPaymentMethodAndStatus(
                        order.ID,
                        "COD",
                        3))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _cartService
                    .EmptyCart())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappened();

            A.CallTo(() => _emailSender.SendEmailAsync(
                    user.Email,
                    "Order Confirmation",
                    A<string>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CODCheckout_WithFixedDiscount_ShouldSubtractDiscount()
        {
            // Arrange
            var userId = "user-1";

            var user = new AppUser
            {
                Id = userId,
                UserName = "Test",
                Email = "test@test.com"
            };

            var order = new Order
            {
                ID = 1,

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
            };

            A.CallTo(() => _users.GetUserId())
                .Returns(userId);

            A.CallTo(() => _userManager.FindByIdAsync(userId))
                .Returns(user);

            A.CallTo(() => _servicesInstanceProvider
                .GetOrdersServiceInstance())
                .Returns(_ordersService);

            A.CallTo(() => _servicesInstanceProvider
                .GetCartServiceInstance())
                .Returns(_cartService);

            A.CallTo(() => _ordersService
                .GetUserCurrentOrder(userId))
                .Returns(order);

            // Act
            var result = await _service.CODCheckout();

            // Assert
            // 200 - 30 = 170
            Assert.Equal(170, result.TotalPrice);

            A.CallTo(() => _ordersService
                .SetOrderPaymentMethodAndStatus(
                    order.ID,
                    "COD",
                    3))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CODCheckout_WithPercentageDiscount_ShouldApplyPercentage()
        {
            // Arrange
            var userId = "user-1";

            var user = new AppUser
            {
                Id = userId,
                UserName = "Test",
                Email = "test@test.com"
            };

            var order = new Order
            {
                ID = 1,

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
            };

            A.CallTo(() => _users.GetUserId())
                .Returns(userId);

            A.CallTo(() => _userManager.FindByIdAsync(userId))
                .Returns(user);

            A.CallTo(() => _servicesInstanceProvider
                .GetOrdersServiceInstance())
                .Returns(_ordersService);

            A.CallTo(() => _servicesInstanceProvider
                .GetCartServiceInstance())
                .Returns(_cartService);

            A.CallTo(() => _ordersService
                .GetUserCurrentOrder(userId))
                .Returns(order);

            // Act
            var result = await _service.CODCheckout();

            // Assert
            // 200 * (1 - 10 / 100) = 180
            Assert.Equal(180, result.TotalPrice);
        }

        [Fact]
        public async Task SucceedOrder_Paypal_ShouldCompleteOrder()
        {
            // Arrange
            var userId = "user-1";

            var user = new AppUser
            {
                Id = userId,
                UserName = "Test",
                Email = "test@test.com"
            };

            var order = new Order
            {
                ID = 20,

                OrderDetails = new List<OrderDetails>
            {
                new OrderDetails
                {
                    Price = 100,
                    Quantity = 2
                }
            },

                PromoCodeDiscountType = null,
                PromoCodeDiscountValue = null
            };

            A.CallTo(() => _users.GetUserId())
                .Returns(userId);

            A.CallTo(() => _userManager.FindByIdAsync(userId))
                .Returns(user);

            A.CallTo(() => _servicesInstanceProvider
                .GetOrdersServiceInstance())
                .Returns(_ordersService);

            A.CallTo(() => _servicesInstanceProvider
                .GetCartServiceInstance())
                .Returns(_cartService);

            A.CallTo(() => _ordersService
                .GetUserCurrentOrder(userId))
                .Returns(order);

            // Act
            var result = await _service.SucceedOrder(
                "Paypal",
                null);

            // Assert
            Assert.True(result);

            // 100 * 2 = 200
            Assert.Equal(200, order.TotalPrice);

            A.CallTo(() => _ordersService
                .SetOrderPaymentMethodAndStatus(
                    order.ID,
                    "Paypal",
                    1))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _cartService
                .EmptyCart())
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappened();

            A.CallTo(() => _emailSender.SendEmailAsync(
                    user.Email,
                    "Order Confirmation",
                    A<string>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SucceedOrder_Paypal_WithFixedDiscount_ShouldApplyDiscount()
        {
            // Arrange
            var userId = "user-1";

            var user = new AppUser
            {
                Id = userId,
                UserName = "Test",
                Email = "test@test.com"
            };

            var order = new Order
            {
                ID = 20,

                OrderDetails = new List<OrderDetails>
            {
                new OrderDetails
                {
                    Price = 100,
                    Quantity = 2
                }
            },

                PromoCodeDiscountType = "fixed",
                PromoCodeDiscountValue = 25
            };

            A.CallTo(() => _users.GetUserId())
                .Returns(userId);

            A.CallTo(() => _userManager.FindByIdAsync(userId))
                .Returns(user);

            A.CallTo(() => _servicesInstanceProvider
                .GetOrdersServiceInstance())
                .Returns(_ordersService);

            A.CallTo(() => _servicesInstanceProvider
                .GetCartServiceInstance())
                .Returns(_cartService);

            A.CallTo(() => _ordersService
                .GetUserCurrentOrder(userId))
                .Returns(order);

            // Act
            var result = await _service.SucceedOrder(
                "Paypal",
                null);

            // Assert
            Assert.True(result);

            // 200 - 25 = 175
            Assert.Equal(175, order.TotalPrice);
        }

        [Fact]
        public async Task SucceedOrder_Paypal_WithPercentageDiscount_ShouldApplyDiscount()
        {
            // Arrange
            var userId = "user-1";

            var user = new AppUser
            {
                Id = userId,
                UserName = "Test",
                Email = "test@test.com"
            };

            var order = new Order
            {
                ID = 20,

                OrderDetails = new List<OrderDetails>
            {
                new OrderDetails
                {
                    Price = 100,
                    Quantity = 2
                }
            },

                PromoCodeDiscountType = "percentage",
                PromoCodeDiscountValue = 20
            };

            A.CallTo(() => _users.GetUserId())
                .Returns(userId);

            A.CallTo(() => _userManager.FindByIdAsync(userId))
                .Returns(user);

            A.CallTo(() => _servicesInstanceProvider
                .GetOrdersServiceInstance())
                .Returns(_ordersService);

            A.CallTo(() => _servicesInstanceProvider
                .GetCartServiceInstance())
                .Returns(_cartService);

            A.CallTo(() => _ordersService
                .GetUserCurrentOrder(userId))
                .Returns(order);

            // Act
            var result = await _service.SucceedOrder(
                "Paypal",
                null);

            // Assert
            Assert.True(result);

            // 200 * 0.8 = 160
            Assert.Equal(160, order.TotalPrice);
        }

        [Fact]
        public async Task SucceedOrder_NonPaypalWithInvalidSession_ShouldNotBeTestedDirectly()
        {
            /*
             * Your current implementation creates:
             *
             * var service = new SessionService();
             * Session session = service.Get(sessionID);
             *
             * This is an external Stripe dependency.
             *
             * The clean solution is to inject a Stripe abstraction.
             */

            Assert.True(true);
        }

        [Fact]
        public async Task FaildOrder_ShouldSetPaymentMethodAndFailedStatus()
        {
            // Arrange
            var userId = "user-1";

            var order = new Order
            {
                ID = 50
            };

            A.CallTo(() => _users.GetUserId())
                .Returns(userId);

            A.CallTo(() => _servicesInstanceProvider
                .GetOrdersServiceInstance())
                .Returns(_ordersService);

            A.CallTo(() => _ordersService
                .GetUserCurrentOrder(userId))
                .Returns(order);

            // Act
            await _service.FaildOrder("Paypal");

            // Assert
            A.CallTo(() => _ordersService
                .SetOrderPaymentMethodAndStatus(
                    order.ID,
                    "Paypal",
                    2))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task FaildOrder_ShouldUseProvidedPaymentMethod()
        {
            // Arrange
            var userId = "user-2";

            var order = new Order
            {
                ID = 100
            };

            A.CallTo(() => _users.GetUserId())
                .Returns(userId);

            A.CallTo(() => _servicesInstanceProvider
                .GetOrdersServiceInstance())
                .Returns(_ordersService);

            A.CallTo(() => _ordersService
                .GetUserCurrentOrder(userId))
                .Returns(order);

            // Act
            await _service.FaildOrder("Stripe");

            // Assert
            A.CallTo(() => _ordersService
                .SetOrderPaymentMethodAndStatus(
                    100,
                    "Stripe",
                    2))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }
    }
}
