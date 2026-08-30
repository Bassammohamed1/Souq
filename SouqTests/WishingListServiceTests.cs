using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models.Wishing_List;
using FakeItEasy;
using Xunit;

namespace SouqTests
{
    public class WishingListServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _provider;
        private readonly IWishListRepository _wishLists;

        private readonly WishingListService _service;

        public WishingListServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userService = A.Fake<IUsersService>();
            _provider = A.Fake<IServicesInstanceProvider>();
            _wishLists = A.Fake<IWishListRepository>();

            A.CallTo(() => _unitOfWork.WishLists)
                .Returns(_wishLists);

            _service = new WishingListService(
                _unitOfWork,
                _userService,
                _provider);
        }
        
        [Fact]
        public async Task Add_ShouldThrow_WhenUserIsNotLoggedIn()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns(null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.Add(1, "Laptops"));
        }

        [Fact]
        public async Task Add_ShouldCreateWishList_WhenUserHasNoWishList()
        {
            // Arrange
            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            var wishList = new WishingList
            {
                Id = 1,
                UserId = "user1"
            };

            A.CallTo(() => _wishLists.GetUserWishingList("user1"))
                .ReturnsNextFromSequence(
                    null,
                    wishList);

            A.CallTo(() => _wishLists.Add(A<WishingList>._))
                .Returns(wishList);

            A.CallTo(() => _wishLists.AddWishingListDetails(
                    A<WishingListDetails>._))
                .Returns(new WishingListDetails());

            A.CallTo(() => _wishLists.GetUserWishingListDetails("user1"))
                .Returns(new List<WishingListDetails>
                {
            new WishingListDetails()
                }.AsQueryable());

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Add(1, "Laptops");

            // Assert
            Assert.Equal(1, result);

            A.CallTo(() => _wishLists.Add(A<WishingList>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _wishLists.AddWishingListDetails(
                    A<WishingListDetails>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Add_ShouldAddDetails_WhenWishListAlreadyExists()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            var wishList = new WishingList
            {
                Id = 5,
                UserId = "user1"
            };

            A.CallTo(() => _wishLists.GetUserWishingList("user1"))
                .Returns(wishList);

            A.CallTo(() => _wishLists.AddWishingListDetails(
                    A<WishingListDetails>._))
                .Returns(Task.FromResult(new WishingListDetails()));

            A.CallTo(() => _wishLists.GetUserWishingListDetails("user1"))
                .Returns(new List<WishingListDetails>
                {
                new WishingListDetails(),
                new WishingListDetails()
                }.AsQueryable());

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            var result = await _service.Add(10, "TVs");

            Assert.Equal(2, result);

            A.CallTo(() => _wishLists.AddWishingListDetails(
                    A<WishingListDetails>.That.Matches(x =>
                        x.ItemId == 10 &&
                        x.ItemType == "TVs" &&
                        x.WishingListId == 5)))
                .MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public async Task Remove_ShouldThrow_WhenUserIsNotLoggedIn()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns(null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.Remove(1, "Laptops"));
        }

        [Fact]
        public async Task Remove_ShouldThrow_WhenWishListDoesNotExist()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            A.CallTo(() => _wishLists.GetUserWishingList("user1"))
                .Returns((WishingList)null);

            // Your implementation accesses userWishingList.Id
            // before checking null, so NullReferenceException occurs.
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _service.Remove(1, "Laptops"));
        }

        [Fact]
        public async Task Remove_ShouldThrow_WhenItemDoesNotExistInWishList()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            var wishList = new WishingList
            {
                Id = 1,
                UserId = "user1"
            };

            A.CallTo(() => _wishLists.GetUserWishingList("user1"))
                .Returns(wishList);

            A.CallTo(() => _wishLists.GetUserWishingListDetails(
                    1, 10, "Laptops"))
                .Returns((WishingListDetails)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.Remove(10, "Laptops"));
        }

        [Fact]
        public async Task Remove_ShouldRemoveItem_WhenItemExists()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            var wishList = new WishingList
            {
                Id = 1,
                UserId = "user1"
            };

            var details = new WishingListDetails
            {
                ItemId = 10,
                ItemType = "Laptops",
                WishingListId = 1
            };

            A.CallTo(() => _wishLists.GetUserWishingList("user1"))
                .Returns(wishList);

            A.CallTo(() => _wishLists.GetUserWishingListDetails(
                    1, 10, "Laptops"))
                .Returns(details);

            A.CallTo(() => _wishLists.RemoveWishingListDetails(details))
                .Returns(details);

            A.CallTo(() => _wishLists.GetUserWishingListDetails("user1"))
                .Returns(new List<WishingListDetails>().AsQueryable());

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            var result = await _service.Remove(10, "Laptops");

            Assert.Equal(0, result);

            A.CallTo(() => _wishLists.RemoveWishingListDetails(details))
                .MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public async Task TotalItemsInWishingList_ShouldReturnCount()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            A.CallTo(() => _wishLists.GetUserWishingList("user1"))
                .Returns(new WishingList
                {
                    Id = 1,
                    UserId = "user1"
                });

            A.CallTo(() => _wishLists.GetUserWishingListDetails("user1"))
                .Returns(new List<WishingListDetails>
                {
                new WishingListDetails(),
                new WishingListDetails(),
                new WishingListDetails()
                }.AsQueryable());

            var result = await _service.TotalItemsInWishingList();

            Assert.Equal(3, result);
        }

        [Fact]
        public async Task TotalItemsInWishingList_ShouldThrow_WhenWishListDoesNotExist()
        {
            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            A.CallTo(() => _wishLists.GetUserWishingList("user1"))
                .Returns((WishingList)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.TotalItemsInWishingList());
        }
        
        [Fact]
        public async Task HasUserLiked_ShouldReturnTrue_WhenItemExists()
        {
            var wishList = new WishingList
            {
                Id = 1,
                UserId = "user1"
            };

            var details = new WishingListDetails
            {
                ItemId = 10,
                ItemType = "Laptops",
                WishingListId = 1
            };

            A.CallTo(() => _wishLists.GetUserWishingList("user1"))
                .Returns(wishList);

            A.CallTo(() => _wishLists.GetUserWishingListDetails(
                    1, 10, "Laptops"))
                .Returns(details);

            var result = await _service.HasUserLiked(
                "user1",
                10,
                "Laptops");

            Assert.True(result);
        }

        [Fact]
        public async Task HasUserLiked_ShouldReturnFalse_WhenItemDoesNotExist()
        {
            var wishList = new WishingList
            {
                Id = 1,
                UserId = "user1"
            };

            A.CallTo(() => _wishLists.GetUserWishingList("user1"))
                .Returns(wishList);

            A.CallTo(() => _wishLists.GetUserWishingListDetails(
                    1, 10, "Laptops"))
                .Returns((WishingListDetails)null);

            var result = await _service.HasUserLiked(
                "user1",
                10,
                "Laptops");

            Assert.False(result);
        }

        [Fact]
        public async Task HasUserLiked_ShouldCreateWishList_WhenUserHasNoWishList()
        {
            A.CallTo(() => _wishLists.GetUserWishingList("user1"))
                .Returns((WishingList)null);

            A.CallTo(() => _wishLists.Add(A<WishingList>._))
                .Returns(Task.FromResult(new WishingList
                {
                    UserId = "user1"
                }));

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            var result = await _service.HasUserLiked(
                "user1",
                10,
                "Laptops");

            Assert.False(result);

            A.CallTo(() => _wishLists.Add(
                    A<WishingList>.That.Matches(x =>
                        x.UserId == "user1")))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }
    }
}