using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using X.PagedList.Extensions;
using Xunit;

namespace SouqTests
{
    public class VideoGamesServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        private readonly IRepository<VideoGame> _VideoGamesRepository;

        private readonly VideoGamesService _service;

        public VideoGamesServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userService = A.Fake<IUsersService>();
            _servicesInstanceProvider = A.Fake<IServicesInstanceProvider>();

            _VideoGamesRepository = A.Fake<IRepository<VideoGame>>();

            A.CallTo(() => _unitOfWork.VideoGames)
                .Returns(_VideoGamesRepository);

            _service = new VideoGamesService(
                _unitOfWork,
                _userService,
                _servicesInstanceProvider);
        }

        [Fact]
        public async Task GetVideoGame_ShouldReturnVideoGame()
        {
            // Arrange
            var VideoGame = new VideoGame
            {
                ID = 1,
                Name = "Samsung"
            };

            A.CallTo(() => _VideoGamesRepository.GetById(1))
                .Returns(VideoGame);

            // Act
            var result = await _service.GetVideoGame(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ID);
            Assert.Equal("Samsung", result.Name);
        }

        [Fact]
        public async Task GetVideoGame_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            A.CallTo(() => _VideoGamesRepository.GetById(1))
                .Returns((VideoGame)null);

            // Act
            var result = await _service.GetVideoGame(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetVideoGames_ShouldReturnVideoGames()
        {
            // Arrange
            var VideoGames = new List<VideoGame>
        {
            new VideoGame { ID = 1, Name = "Samsung" },
            new VideoGame { ID = 2, Name = "LG" }
        };

            A.CallTo(() => _VideoGamesRepository.GetAll(1, 10))
                .Returns(VideoGames.ToPagedList());

            // Act
            var result = _service.GetVideoGames(1, 10);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().ID);
        }

        [Fact]
        public async Task Add_ShouldReturnSuccess_WhenRepositoryAddsSuccessfully()
        {
            // Arrange
            var VideoGame = new VideoGame
            {
                ID = 1
            };

            var file = A.Fake<IFormFile>();

            var imageBytes = new byte[] { 1, 2, 3 };

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Invokes((Stream stream, CancellationToken _) =>
                {
                    stream.Write(imageBytes);
                })
                .Returns(Task.CompletedTask);

            VideoGame.clientFile = file;

            A.CallTo(() => _VideoGamesRepository.Add(VideoGame))
                .Returns(VideoGame);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Add(VideoGame);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() => _VideoGamesRepository.Add(VideoGame))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Add_ShouldReturnFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            var VideoGame = new VideoGame
            {
                ID = 1
            };

            var file = A.Fake<IFormFile>();

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Returns(Task.CompletedTask);

            VideoGame.clientFile = file;

            A.CallTo(() => _VideoGamesRepository.Add(VideoGame))
                .Returns((VideoGame)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Add(VideoGame);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("An error occured while adding.", result.Error);
        }

        [Fact]
        public async Task Update_ShouldReturnSuccess_WhenRepositoryUpdatesSuccessfully()
        {
            // Arrange
            var VideoGame = new VideoGame
            {
                ID = 1
            };

            var file = A.Fake<IFormFile>();

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Returns(Task.CompletedTask);

            VideoGame.clientFile = file;

            A.CallTo(() => _VideoGamesRepository.Update(VideoGame))
                .Returns(VideoGame);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Update(VideoGame);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() => _VideoGamesRepository.Update(VideoGame))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            var VideoGame = new VideoGame
            {
                ID = 1
            };

            var file = A.Fake<IFormFile>();

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Returns(Task.CompletedTask);

            VideoGame.clientFile = file;

            A.CallTo(() => _VideoGamesRepository.Update(VideoGame))
                .Returns((VideoGame)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Update(VideoGame);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("An error occured while updating.", result.Error);
        }

        [Fact]
        public async Task Delete_ShouldReturnSuccess_WhenRepositoryDeletesSuccessfully()
        {
            // Arrange
            var VideoGame = new VideoGame
            {
                ID = 1
            };

            A.CallTo(() => _VideoGamesRepository.Delete(VideoGame))
                .Returns(VideoGame);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Delete(VideoGame);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() => _VideoGamesRepository.Delete(VideoGame))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Delete_ShouldReturnFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            var VideoGame = new VideoGame
            {
                ID = 1
            };

            A.CallTo(() => _VideoGamesRepository.Delete(VideoGame))
                .Returns((VideoGame)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Delete(VideoGame);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("An error occured while deleting.", result.Error);
        }

        [Fact]
        public async Task GetSpecificCategoriesForSelectList_ShouldReturnCategories()
        {
            // Arrange
            var categories = new List<Category>
        {
            new Category { ID = 1 },
            new Category { ID = 2 }
        };

            var categoriesService = A.Fake<ICategoriesService>();

            A.CallTo(() => _servicesInstanceProvider.GetCategoriesServiceInstance())
                .Returns(categoriesService);

            A.CallTo(() => categoriesService.GetSpecificCategories("Video Games"))
                .Returns(categories);

            // Act
            var result = await _service.GetSpecificCategoriesForSelectList();

            // Assert
            Assert.Equal(2, result.Count());

            A.CallTo(() => categoriesService.GetSpecificCategories("Video Games"))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetBrandsVideoGames_ShouldReturnCorrectData()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();
            var wishlistService = A.Fake<IWishingListService>();

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => _servicesInstanceProvider.GetWishingListServiceInstance())
                .Returns(wishlistService);

            var category = new Category
            {
                ID = 1,
                Name = "Samsung"
            };

            var VideoGames = new List<VideoGame>
    {
        new VideoGame
        {
            ID = 1,
            Name = "Samsung AC",
            Price = 20000,
            NewPrice = 18000,
            Rate = 4.5,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<VideoGame>(
                "Brands", null, null, "Samsung"))
                .Returns(10);

            A.CallTo(() => itemsService.GetCategoryItems<VideoGame>(
                "Samsung", 1, 9, "ID", false))
                .Returns(VideoGames.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 1, "VideoGames"))
                .Returns(true);

            A.CallTo(() => itemsService.GetItemRates(1, "VideoGames"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetBrandsVideoGames(
                null, null, "Samsung", null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal("Brands", result.ActionName);
            Assert.Equal("Samsung", result.Brand);

            var item = result.Items.First();

            Assert.Equal(1, item.Id);
            Assert.Equal("Samsung AC", item.Name);
            Assert.Equal(20000, item.Price);
            Assert.Equal(18000, item.NewPrice);
            Assert.True(item.isLiked);
        }

        [Fact]
        public async Task GetDiscountedVideoGames_ShouldReturnDiscountedItems()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();
            var wishlistService = A.Fake<IWishingListService>();

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => _servicesInstanceProvider.GetWishingListServiceInstance())
                .Returns(wishlistService);

            var category = new Category
            {
                ID = 1,
                Name = "Samsung"
            };

            var items = new List<VideoGame>
    {
        new VideoGame
        {
            ID = 1,
            Name = "Samsung AC",
            Price = 20000,
            NewPrice = 17000,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<VideoGame>("Discounted", null, null, null))
                .Returns(18);

            A.CallTo(() => itemsService.GetDiscountedItems<VideoGame>(
                1, 9, "ID", false))
                .Returns(items.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 1, "VideoGames"))
                .Returns(false);

            A.CallTo(() => itemsService.GetItemRates(1, "VideoGames"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetDiscountedVideoGames(
                null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal("Discounted", result.ActionName);

            var item = result.Items.First();

            Assert.Equal(1, item.Id);
            Assert.Equal(17000, item.NewPrice);
            Assert.False(item.isLiked);
        }

        [Fact]
        public async Task GetTopRatedVideoGames_ShouldReturnTopRatedItems()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();
            var wishlistService = A.Fake<IWishingListService>();

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => _servicesInstanceProvider.GetWishingListServiceInstance())
                .Returns(wishlistService);

            var category = new Category
            {
                ID = 1,
                Name = "LG"
            };

            var items = new List<VideoGame>
    {
        new VideoGame
        {
            ID = 5,
            Name = "LG AC",
            Price = 25000,
            Rate = 4.9,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<VideoGame>("Rated", null, null, null))
                .Returns(10);

            A.CallTo(() => itemsService.GetTopRatedItems<VideoGame>(
                1, 9, "ID", false))
                .Returns(items.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 5, "VideoGames"))
                .Returns(true);

            A.CallTo(() => itemsService.GetItemRates(5, "VideoGames"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetTopRatedVideoGames(
                null, null, null);

            // Assert
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal("TopRated", result.ActionName);

            var item = result.Items.First();

            Assert.Equal(5, item.Id);
            Assert.Equal("LG AC", item.Name);
            Assert.Equal(4.9, item.Rate);
            Assert.True(item.isLiked);
        }

        [Fact]
        public async Task GetLatestVideoGames_ShouldReturnLatestItems()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();
            var wishlistService = A.Fake<IWishingListService>();

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => _servicesInstanceProvider.GetWishingListServiceInstance())
                .Returns(wishlistService);

            var category = new Category
            {
                ID = 1,
                Name = "Tornado"
            };

            var items = new List<VideoGame>
    {
        new VideoGame
        {
            ID = 10,
            Name = "Tornado AC",
            Price = 15000,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<VideoGame>("Latest", null, null, null))
                .Returns(9);

            A.CallTo(() => itemsService.GetLatestItems<VideoGame>(
                1, 9, "ID", false))
                .Returns(items.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 10, "VideoGames"))
                .Returns(false);

            A.CallTo(() => itemsService.GetItemRates(10, "VideoGames"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetLatestVideoGames(
                null, null, null);

            // Assert
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal("Latest", result.ActionName);

            var item = result.Items.First();

            Assert.Equal(10, item.Id);
            Assert.Equal("Tornado AC", item.Name);
        }

        [Fact]
        public async Task GetVideoGamesWithPriceFilter_ShouldReturnFilteredItems()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();
            var wishlistService = A.Fake<IWishingListService>();

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => _servicesInstanceProvider.GetWishingListServiceInstance())
                .Returns(wishlistService);

            var category = new Category
            {
                ID = 1,
                Name = "Samsung"
            };

            var items = new List<VideoGame>
    {
        new VideoGame
        {
            ID = 3,
            Name = "Samsung AC",
            Price = 20000,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<VideoGame>(
                "Price", 15000, 25000, null))
                .Returns(12);

            A.CallTo(() => itemsService.GetItemsFilteredByPrice<VideoGame>(
                15000, 25000, 1, 9, "ID", false))
                .Returns(items.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 3, "VideoGames"))
                .Returns(true);

            A.CallTo(() => itemsService.GetItemRates(3, "VideoGames"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetVideoGamesWithPriceFilter(
                null, null, 15000, 25000, null);

            // Assert
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal("PriceFilter", result.ActionName);
            Assert.Equal(15000, result.Price1);
            Assert.Equal(25000, result.Price2);

            Assert.Equal(3, result.Items.First().Id);
        }

        [Fact]
        public void GetVideoGamesWithRelatedOnes_ShouldReturnAllSections()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();
            var wishlistService = A.Fake<IWishingListService>();

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => _servicesInstanceProvider.GetWishingListServiceInstance())
                .Returns(wishlistService);

            var category = new Category
            {
                ID = 1,
                Name = "Samsung"
            };

            var item = new VideoGame
            {
                ID = 1,
                Name = "Samsung AC",
                Price = 20000,
                Rate = 4.5,
                Category = category
            };

            A.CallTo(() => itemsService.GetItemCategories<VideoGame>())
                .Returns(new List<Category> { category });

            A.CallTo(() => itemsService.GetDiscountedItems<VideoGame>(
                1, 10, "ID", false))
                .Returns(new List<VideoGame> { item }.AsQueryable());

            A.CallTo(() => itemsService.GetTopRatedItems<VideoGame>(
                1, 10, "ID", false))
                .Returns(new List<VideoGame> { item }.AsQueryable());

            A.CallTo(() => itemsService.GetLatestItems<VideoGame>(
                1, 10, "ID", false))
                .Returns(new List<VideoGame> { item }.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 1, "VideoGames"))
                .Returns(true);

            A.CallTo(() => itemsService.GetItemRates(1, "VideoGames"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = _service.GetVideoGamesWithRelatedOnes();

            // Assert
            Assert.NotNull(result);

            Assert.Single(result.ItemCategories);
            Assert.Single(result.DiscountedItems);
            Assert.Single(result.TopRatedItems);
            Assert.Single(result.latestItems);
        }

        [Fact]
        public async Task GetVideoGameAllComments_ShouldReturnNull_WhenVideoGameDoesNotExist()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();

            A.CallTo(() => _unitOfWork.VideoGames)
                .Returns(_VideoGamesRepository);

            A.CallTo(() => _VideoGamesRepository.GetById(1))
                .Returns((VideoGame)null);

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => itemsService.GetItemRates(1, "VideoGames"))
                .Returns(new List<Rate>());

            A.CallTo(() => itemsService.GetItemRateDetails<VideoGame>(
                1, "VideoGames"))
                .Returns(new int[] { 5, 3, 2 });

            // Act
            var result = await _service.GetVideoGameAllComments(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetVideoGameAllComments_ShouldReturnNull_WhenThereAreNoComments()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();

            var category = new Category
            {
                ID = 1,
                Name = "Samsung"
            };

            var VideoGame = new VideoGame
            {
                ID = 1,
                Name = "Samsung AC",
                Rate = 4.5,
                Category = category
            };

            A.CallTo(() => _unitOfWork.VideoGames)
                .Returns(_VideoGamesRepository);

            A.CallTo(() => _VideoGamesRepository.GetById(1))
                .Returns(VideoGame);

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => itemsService.GetItemRates(
                1, "VideoGames"))
                .Returns(new List<Rate>());

            A.CallTo(() => itemsService.GetItemRateDetails<VideoGame>(
                1, "VideoGames"))
                .Returns(new int[] { 5, 3, 2 });

            A.CallTo(() => itemsService.GetItemComments(
                1, "VideoGames", "All"))
                .Returns(new List<Comment>());

            // Act
            var result = await _service.GetVideoGameAllComments(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetVideoGameAllComments_ShouldReturnDto_WhenCommentsExist()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();

            var category = new Category
            {
                ID = 1,
                Name = "Samsung"
            };

            var VideoGame = new VideoGame
            {
                ID = 1,
                Name = "Samsung AC",
                Rate = 4.5,
                Category = category
            };

            var comments = new List<Comment>
    {
        new Comment(),
        new Comment()
    };

            var rates = new List<Rate>
    {
        new Rate(),
        new Rate(),
        new Rate()
    };

            var starCounts = new[] { 5, 2, 1 };

            A.CallTo(() => _unitOfWork.VideoGames)
                .Returns(_VideoGamesRepository);

            A.CallTo(() => _VideoGamesRepository.GetById(1))
                .Returns(VideoGame);

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => itemsService.GetItemRates(
                1, "VideoGames"))
                .Returns(rates);

            A.CallTo(() => itemsService.GetItemRateDetails<VideoGame>(
                1, "VideoGames"))
                .Returns(starCounts);

            A.CallTo(() => itemsService.GetItemComments(
                1, "VideoGames", "All"))
                .Returns(comments);

            // Act
            var result = await _service.GetVideoGameAllComments(1);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(1, result.Id);
            Assert.Equal("Samsung AC", result.Name);
            Assert.Equal(4.5, result.Rate);
            Assert.Equal("Samsung", result.CategoryName);

            Assert.Equal(2, result.Comments.Count());
            Assert.Equal(3, result.RateCount);

            Assert.Equal(starCounts, result.StarCounts);
        }
    }
}
