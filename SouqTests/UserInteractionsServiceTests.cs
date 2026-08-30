using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using FakeItEasy;
using Xunit;

namespace SouqTests
{
  

    public class UserInteractionsServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;
        private readonly IItemsService _itemsService;

        private readonly UserInteractionsService _service;

        public UserInteractionsServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _servicesInstanceProvider = A.Fake<IServicesInstanceProvider>();
            _itemsService = A.Fake<IItemsService>();

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(_itemsService);

            _service = new UserInteractionsService(
                _unitOfWork,
                _servicesInstanceProvider);
        }

        [Fact]
        public async Task AddComment_ShouldReturnSuccess_WhenCommentIsAdded()
        {
            // Arrange
            var comment = new Comment
            {
                ItemId = 1,
                UserId = "user1"
            };

            A.CallTo(() => _unitOfWork.Comments.Add(comment))
                .Returns(comment);

            // Act
            var result = await _service.AddComment(comment);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);

            A.CallTo(() => _unitOfWork.Comments.Add(comment))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AddComment_ShouldReturnFailure_WhenCommentIsNotAdded()
        {
            // Arrange
            var comment = new Comment
            {
                ItemId = 1,
                UserId = "user1"
            };

            A.CallTo(() => _unitOfWork.Comments.Add(comment))
                .Returns((Comment)null);

            // Act
            var result = await _service.AddComment(comment);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(
                "An Error occured while making comment.",
                result.Error);

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AddRate_ShouldAddRateAndSetRate_WhenNoPreviousRateExists()
        {
            // Arrange
            var rate = new Rate
            {
                UserId = "user1",
                ItemId = 1,
                ItemType = "Laptops",
                Value = 5
            };

            var existingRates = new List<Rate>();

            A.CallTo(() => _unitOfWork.Rates.GetAll())
                .Returns(Task.FromResult<IEnumerable<Rate>>(existingRates));

            A.CallTo(() => _unitOfWork.Rates.Add(rate))
                .Returns(rate);

            A.CallTo(() => _itemsService.SetRate(rate))
                .Returns(true);

            // Act
            var result = await _service.AddRate(rate);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);

            A.CallTo(() => _unitOfWork.Rates.Add(rate))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Rates.Delete(A<Rate>._))
                .MustNotHaveHappened();

            A.CallTo(() => _itemsService.SetRate(rate))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AddRate_ShouldDeleteOldRateAndAddNewRate_WhenPreviousRateExists()
        {
            // Arrange
            var oldRate = new Rate
            {
                UserId = "user1",
                ItemId = 1,
                ItemType = "Laptops",
                Value = 3
            };

            var newRate = new Rate
            {
                UserId = "user1",
                ItemId = 1,
                ItemType = "Laptops",
                Value = 5
            };

            var existingRates = new List<Rate>
        {
            oldRate
        };

            A.CallTo(() => _unitOfWork.Rates.GetAll())
                .Returns(Task.FromResult<IEnumerable<Rate>>(existingRates));

            A.CallTo(() => _unitOfWork.Rates.Add(newRate))
                .Returns(newRate);

            A.CallTo(() => _itemsService.SetRate(newRate))
                .Returns(true);

            // Act
            var result = await _service.AddRate(newRate);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() => _unitOfWork.Rates.Delete(oldRate))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Rates.Add(newRate))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _itemsService.SetRate(newRate))
                .MustHaveHappenedOnceExactly();

            // One Commit for deleting old rate
            // One Commit for adding new rate
            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task AddRate_ShouldReturnFailure_WhenSetRateFails()
        {
            // Arrange
            var rate = new Rate
            {
                UserId = "user1",
                ItemId = 1,
                ItemType = "Laptops",
                Value = 4
            };

            A.CallTo(() => _unitOfWork.Rates.GetAll())
                .Returns(Task.FromResult<IEnumerable<Rate>>(
                    new List<Rate>()));

            A.CallTo(() => _unitOfWork.Rates.Add(rate))
                .Returns(rate);

            A.CallTo(() => _itemsService.SetRate(rate))
                .Returns(false);

            // Act
            var result = await _service.AddRate(rate);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(
                "An error occured while setting rate.",
                result.Error);

            A.CallTo(() => _itemsService.SetRate(rate))
                .MustHaveHappenedOnceExactly();
        }
    }
}
