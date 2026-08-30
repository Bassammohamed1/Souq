using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using FakeItEasy;
using Souq.Models.Cart_Orders;
using Xunit;

namespace SouqTests
{
    public class CartServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _provider;

        private readonly ICartRepository _carts;
        private readonly IRepository<AirConditioner> _airConditioners;
        private readonly IRepository<Fridge> _fridges;
        private readonly IRepository<Cooker> _cookers;
        private readonly IRepository<WashingMachine> _washingMachines;
        private readonly IRepository<Laptop> _laptops;
        private readonly IRepository<HeadPhone> _headPhones;
        private readonly IRepository<TV> _tvs;
        private readonly IRepository<MobilePhone> _mobilePhones;
        private readonly IRepository<VideoGame> _videoGames;

        private readonly IOffersService _offersService;
        private readonly IOrdersService _ordersService;
        private readonly IItemsService _itemsService;

        private readonly CartService _service;

        public CartServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userService = A.Fake<IUsersService>();
            _provider = A.Fake<IServicesInstanceProvider>();

            _carts = A.Fake<ICartRepository>();

            _airConditioners = A.Fake<IRepository<AirConditioner>>();
            _fridges = A.Fake<IRepository<Fridge>>();
            _cookers = A.Fake<IRepository<Cooker>>();
            _washingMachines = A.Fake<IRepository<WashingMachine>>();
            _laptops = A.Fake<IRepository<Laptop>>();
            _headPhones = A.Fake<IRepository<HeadPhone>>();
            _tvs = A.Fake<IRepository<TV>>();
            _mobilePhones = A.Fake<IRepository<MobilePhone>>();
            _videoGames = A.Fake<IRepository<VideoGame>>();

            A.CallTo(() => _unitOfWork.Carts).Returns(_carts);

            A.CallTo(() => _unitOfWork.AirConditioners).Returns(_airConditioners);
            A.CallTo(() => _unitOfWork.Fridges).Returns(_fridges);
            A.CallTo(() => _unitOfWork.Cookers).Returns(_cookers);
            A.CallTo(() => _unitOfWork.WashingMachines).Returns(_washingMachines);
            A.CallTo(() => _unitOfWork.Laptops).Returns(_laptops);
            A.CallTo(() => _unitOfWork.HeadPhones).Returns(_headPhones);
            A.CallTo(() => _unitOfWork.TVs).Returns(_tvs);
            A.CallTo(() => _unitOfWork.MobilePhones).Returns(_mobilePhones);
            A.CallTo(() => _unitOfWork.VideoGames).Returns(_videoGames);

            A.CallTo(() => _provider.GetOffersServiceInstance())
                .Returns(_offersService = A.Fake<IOffersService>());

            A.CallTo(() => _provider.GetOrdersServiceInstance())
                .Returns(_ordersService = A.Fake<IOrdersService>());

            A.CallTo(() => _provider.GetItemsServiceInstance())
                .Returns(_itemsService = A.Fake<IItemsService>());

            _service = new CartService(
                _unitOfWork,
                _userService,
                _provider);
        }
        
        [Fact]
        public async Task Add_ShouldThrow_WhenItemTypeIsInvalid()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.Add(1, "InvalidType", null));
        }

        [Fact]
        public async Task Add_ShouldThrow_WhenUserIsNotLoggedIn()
        {
            var item = new Laptop
            {
                ID = 1,
                Amount = 5,
                Price = 1000
            };

            A.CallTo(() => _laptops.GetById(1))
                .Returns(item);

            A.CallTo(() => _userService.GetUserId())
                .Returns(null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.Add(1, "Laptops", null));
        }

        [Fact]
        public async Task Add_ShouldReturnMinusOne_WhenCartDetailsCannotBeAdded()
        {
            var item = new Laptop
            {
                ID = 1,
                Amount = 5,
                Price = 1000
            };

            A.CallTo(() => _laptops.GetById(1))
                .Returns(item);

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            var cart = new ShoppingCart
            {
                Id = 1,
                UserId = "user1"
            };

            A.CallTo(() => _carts.GetUserShoppingCart("user1"))
                .Returns(cart);

            A.CallTo(() => _carts.AddCartDetails(A<CartDetails>._))
                .Returns(Task.FromResult<CartDetails>(null));

            var result = await _service.Add(1, "Laptops", null);

            Assert.Equal(-1, result);
        }

        [Fact]
        public async Task Add_ShouldAddNewCart_WhenUserHasNoCart()
        {
            var item = new Laptop
            {
                ID = 1,
                Amount = 5,
                Price = 1000
            };

            A.CallTo(() => _laptops.GetById(1))
                .Returns(item);

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            A.CallTo(() => _carts.GetUserShoppingCart("user1"))
                .Returns((ShoppingCart)null);

            var cart = new ShoppingCart
            {
                Id = 1,
                UserId = "user1"
            };

            A.CallTo(() => _carts.Add(A<ShoppingCart>._))
                .Returns(Task.FromResult(cart));

            A.CallTo(() => _carts.AddCartDetails(A<CartDetails>._))
                .Returns(Task.FromResult(new CartDetails()));

            A.CallTo(() => _carts.GetUserCartDetails("user1"))
                .Returns(new List<CartDetails>().AsQueryable());

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            var result = await _service.Add(1, "Laptops", null);

            Assert.Equal(0, result);
            Assert.Equal(4, item.Amount);

            A.CallTo(() => _carts.Add(A<ShoppingCart>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _carts.AddCartDetails(A<CartDetails>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Add_ShouldIncreaseQuantity_WhenItemAlreadyExistsInCart()
        {
            var item = new Laptop
            {
                ID = 1,
                Amount = 5,
                Price = 1000
            };

            A.CallTo(() => _laptops.GetById(1))
                .Returns(item);

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            var cart = new ShoppingCart
            {
                Id = 1,
                UserId = "user1"
            };

            var details = new CartDetails
            {
                ItemID = 1,
                ItemType = "Laptops",
                Quantity = 1,
                ShoppingCartID = 1
            };

            A.CallTo(() => _carts.GetUserShoppingCart("user1"))
                .Returns(cart);

            A.CallTo(() => _carts.GetUserCartDetails(1, 1, "Laptops"))
                .Returns(details);

            A.CallTo(() => _carts.GetUserCartDetails("user1"))
                .Returns(new List<CartDetails>
                {
                details
                }.AsQueryable());

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            var result = await _service.Add(1, "Laptops", 1);

            Assert.Equal(2, details.Quantity);
            Assert.Equal(4, item.Amount);
            Assert.Equal(2, result);
        }
      
        [Fact]
        public async Task Remove_ShouldThrow_WhenUserIsNotLoggedIn()
        {
            A.CallTo(() => _laptops.GetById(1))
                .Returns(new Laptop { ID = 1 });

            A.CallTo(() => _userService.GetUserId())
                .Returns(null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.Remove(1, "Laptops"));
        }

        [Fact]
        public async Task Remove_ShouldDecreaseQuantity_WhenQuantityGreaterThanOne()
        {
            var item = new Laptop
            {
                ID = 1,
                Amount = 5
            };

            var cart = new ShoppingCart
            {
                Id = 1,
                UserId = "user1"
            };

            var details = new CartDetails
            {
                ItemID = 1,
                ItemType = "Laptops",
                Quantity = 2,
                ShoppingCartID = 1
            };

            A.CallTo(() => _laptops.GetById(1))
                .Returns(item);

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            A.CallTo(() => _carts.GetUserShoppingCart("user1"))
                .Returns(cart);

            A.CallTo(() => _carts.GetUserCartDetails(1, 1, "Laptops"))
                .Returns(details);

            A.CallTo(() => _carts.GetUserCartDetails("user1"))
                .Returns(new List<CartDetails>
                {
                details
                }.AsQueryable());

            var result = await _service.Remove(1, "Laptops");

            Assert.Equal(1, details.Quantity);
            Assert.Equal(6, item.Amount);
        }

        [Fact]
        public async Task Remove_ShouldRemoveCartDetails_WhenQuantityIsOne()
        {
            var item = new Laptop
            {
                ID = 1,
                Amount = 5
            };

            var cart = new ShoppingCart
            {
                Id = 1,
                UserId = "user1"
            };

            var details = new CartDetails
            {
                ItemID = 1,
                ItemType = "Laptops",
                Quantity = 1,
                ShoppingCartID = 1
            };

            A.CallTo(() => _laptops.GetById(1))
                .Returns(item);

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            A.CallTo(() => _carts.GetUserShoppingCart("user1"))
                .Returns(cart);

            A.CallTo(() => _carts.GetUserCartDetails(1, 1, "Laptops"))
                .Returns(details);

            A.CallTo(() => _carts.RemoveCartDetails(details))
                .Returns(details);

            A.CallTo(() => _carts.GetUserCartDetails("user1"))
                .Returns(new List<CartDetails>().AsQueryable());

            var result = await _service.Remove(1, "Laptops");

            Assert.Equal(6, item.Amount);

            A.CallTo(() => _carts.RemoveCartDetails(details))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Remove_ShouldThrow_WhenUserHasNoCart()
        {
            A.CallTo(() => _laptops.GetById(1))
                .Returns(new Laptop { ID = 1 });

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            A.CallTo(() => _carts.GetUserShoppingCart("user1"))
                .Returns((ShoppingCart)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.Remove(1, "Laptops"));
        }
        
        [Fact]
        public async Task TotalItemsInCart_ShouldReturnZero_WhenCartDoesNotExist()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            A.CallTo(() => _carts.GetUserShoppingCart("user1"))
                .Returns((ShoppingCart)null);

            var result = await _service.TotalItemsInCart();

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task TotalItemsInCart_ShouldReturnTotalQuantity()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            A.CallTo(() => _carts.GetUserShoppingCart("user1"))
                .Returns(new ShoppingCart
                {
                    Id = 1,
                    UserId = "user1"
                });

            A.CallTo(() => _carts.GetUserCartDetails("user1"))
                .Returns(new List<CartDetails>
                {
                new CartDetails { Quantity = 2 },
                new CartDetails { Quantity = 3 }
                }.AsQueryable());

            var result = await _service.TotalItemsInCart();

            Assert.Equal(5, result);
        }

        [Fact]
        public async Task TotalItemQuantityInCart_ShouldReturnZero_WhenItemNotInCart()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            A.CallTo(() => _carts.GetUserShoppingCart("user1"))
                .Returns(new ShoppingCart
                {
                    Id = 1
                });

            A.CallTo(() => _carts.GetUserCartDetails(1, 10, "Laptops"))
                .Returns((CartDetails)null);

            var result = await _service.TotalItemQuantityInCart(
                10,
                "Laptops");

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task TotalItemQuantityInCart_ShouldReturnQuantity()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            A.CallTo(() => _carts.GetUserShoppingCart("user1"))
                .Returns(new ShoppingCart
                {
                    Id = 1
                });

            A.CallTo(() => _carts.GetUserCartDetails(1, 10, "Laptops"))
                .Returns(new CartDetails
                {
                    Quantity = 4
                });

            var result = await _service.TotalItemQuantityInCart(
                10,
                "Laptops");

            Assert.Equal(4, result);
        }
       
        [Fact]
        public async Task EmptyCart_ShouldRemoveCartDetails()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            var cart = new ShoppingCart
            {
                Id = 1,
                UserId = "user1"
            };

            A.CallTo(() => _carts.GetUserShoppingCart("user1"))
                .Returns(cart);

            await _service.EmptyCart();

            A.CallTo(() => _carts.RemoveCartDetails(1))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EmptyCart_ShouldThrow_WhenCartDoesNotExist()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            A.CallTo(() => _carts.GetUserShoppingCart("user1"))
                .Returns((ShoppingCart)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.EmptyCart());
        }
        
        [Fact]
        public async Task ApplyPromoCode_ShouldReturnFailure_WhenPromoCodeDoesNotExist()
        {
            A.CallTo(() => _offersService.IsPromoCodeExist("INVALID"))
                .Returns((Offer)null);

            var result = await _service.ApplyPromoCode("INVALID");

            Assert.False(result.Success);
            Assert.Equal("Invalid promo code.", result.Error);
        }
    }
}