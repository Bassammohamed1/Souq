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
    public class CookersServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        private readonly IRepository<Cooker> _cookersRepository;

        private readonly CookersService _service;

        public CookersServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userService = A.Fake<IUsersService>();
            _servicesInstanceProvider = A.Fake<IServicesInstanceProvider>();

            _cookersRepository = A.Fake<IRepository<Cooker>>();

            A.CallTo(() => _unitOfWork.Cookers)
                .Returns(_cookersRepository);

            _service = new CookersService(
                _unitOfWork,
                _userService,
                _servicesInstanceProvider);
        }

        [Fact]
        public async Task GetCooker_ShouldReturnCooker()
        {
            // Arrange
            var airConditioner = new Cooker
            {
                ID = 1,
                Name = "Samsung"
            };

            A.CallTo(() => _cookersRepository.GetById(1))
                .Returns(airConditioner);

            // Act
            var result = await _service.GetCooker(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ID);
            Assert.Equal("Samsung", result.Name);
        }

        [Fact]
        public async Task GetCooker_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            A.CallTo(() => _cookersRepository.GetById(1))
                .Returns((Cooker)null);

            // Act
            var result = await _service.GetCooker(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetCookers_ShouldReturnCookers()
        {
            // Arrange
            var airConditioners = new List<Cooker>
        {
            new Cooker { ID = 1, Name = "Samsung" },
            new Cooker { ID = 2, Name = "LG" }
        };

            A.CallTo(() => _cookersRepository.GetAll(1, 10))
                .Returns(airConditioners.ToPagedList());

            // Act
            var result = _service.GetCookers(1, 10);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().ID);
        }

        [Fact]
        public async Task Add_ShouldReturnSuccess_WhenRepositoryAddsSuccessfully()
        {
            // Arrange
            var airConditioner = new Cooker
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

            airConditioner.clientFile = file;

            A.CallTo(() => _cookersRepository.Add(airConditioner))
                .Returns(airConditioner);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Add(airConditioner);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() => _cookersRepository.Add(airConditioner))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Add_ShouldReturnFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            var airConditioner = new Cooker
            {
                ID = 1
            };

            var file = A.Fake<IFormFile>();

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Returns(Task.CompletedTask);

            airConditioner.clientFile = file;

            A.CallTo(() => _cookersRepository.Add(airConditioner))
                .Returns((Cooker)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Add(airConditioner);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("An error occured while adding.", result.Error);
        }

        [Fact]
        public async Task Update_ShouldReturnSuccess_WhenRepositoryUpdatesSuccessfully()
        {
            // Arrange
            var airConditioner = new Cooker
            {
                ID = 1
            };

            var file = A.Fake<IFormFile>();

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Returns(Task.CompletedTask);

            airConditioner.clientFile = file;

            A.CallTo(() => _cookersRepository.Update(airConditioner))
                .Returns(airConditioner);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Update(airConditioner);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() => _cookersRepository.Update(airConditioner))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            var airConditioner = new Cooker
            {
                ID = 1
            };

            var file = A.Fake<IFormFile>();

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Returns(Task.CompletedTask);

            airConditioner.clientFile = file;

            A.CallTo(() => _cookersRepository.Update(airConditioner))
                .Returns((Cooker)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Update(airConditioner);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("An error occured while updating.", result.Error);
        }

        [Fact]
        public async Task Delete_ShouldReturnSuccess_WhenRepositoryDeletesSuccessfully()
        {
            // Arrange
            var airConditioner = new Cooker
            {
                ID = 1
            };

            A.CallTo(() => _cookersRepository.Delete(airConditioner))
                .Returns(airConditioner);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Delete(airConditioner);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() => _cookersRepository.Delete(airConditioner))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Delete_ShouldReturnFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            var airConditioner = new Cooker
            {
                ID = 1
            };

            A.CallTo(() => _cookersRepository.Delete(airConditioner))
                .Returns((Cooker)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Delete(airConditioner);

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

            A.CallTo(() => categoriesService.GetSpecificCategories("Appliances"))
                .Returns(categories);

            // Act
            var result = await _service.GetSpecificCategoriesForSelectList();

            // Assert
            Assert.Equal(2, result.Count());

            A.CallTo(() => categoriesService.GetSpecificCategories("Appliances"))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetBrandsCookers_ShouldReturnCorrectData()
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

            var airConditioners = new List<Cooker>
    {
        new Cooker
        {
            ID = 1,
            Name = "Samsung AC",
            Price = 20000,
            NewPrice = 18000,
            Rate = 4.5,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<Cooker>(
                "Brands", null, null, "Samsung"))
                .Returns(10);

            A.CallTo(() => itemsService.GetCategoryItems<Cooker>(
                "Samsung", 1, 9, "ID", false))
                .Returns(airConditioners.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 1, "Cookers"))
                .Returns(true);

            A.CallTo(() => itemsService.GetItemRates(1, "Cookers"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetBrandsCookers(
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
        public async Task GetDiscountedCookers_ShouldReturnDiscountedItems()
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

            var items = new List<Cooker>
    {
        new Cooker
        {
            ID = 1,
            Name = "Samsung AC",
            Price = 20000,
            NewPrice = 17000,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<Cooker>("Discounted", null, null, null))
                .Returns(18);

            A.CallTo(() => itemsService.GetDiscountedItems<Cooker>(
                1, 9, "ID", false))
                .Returns(items.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 1, "Cookers"))
                .Returns(false);

            A.CallTo(() => itemsService.GetItemRates(1, "Cookers"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetDiscountedCookers(
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
        public async Task GetTopRatedCookers_ShouldReturnTopRatedItems()
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

            var items = new List<Cooker>
    {
        new Cooker
        {
            ID = 5,
            Name = "LG AC",
            Price = 25000,
            Rate = 4.9,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<Cooker>("Rated", null, null, null))
                .Returns(10);

            A.CallTo(() => itemsService.GetTopRatedItems<Cooker>(
                1, 9, "ID", false))
                .Returns(items.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 5, "Cookers"))
                .Returns(true);

            A.CallTo(() => itemsService.GetItemRates(5, "Cookers"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetTopRatedCookers(
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
        public async Task GetLatestCookers_ShouldReturnLatestItems()
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

            var items = new List<Cooker>
    {
        new Cooker
        {
            ID = 10,
            Name = "Tornado AC",
            Price = 15000,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<Cooker>("Latest", null, null, null))
                .Returns(9);

            A.CallTo(() => itemsService.GetLatestItems<Cooker>(
                1, 9, "ID", false))
                .Returns(items.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 10, "Cookers"))
                .Returns(false);

            A.CallTo(() => itemsService.GetItemRates(10, "Cookers"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetLatestCookers(
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
        public async Task GetCookersWithPriceFilter_ShouldReturnFilteredItems()
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

            var items = new List<Cooker>
    {
        new Cooker
        {
            ID = 3,
            Name = "Samsung AC",
            Price = 20000,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<Cooker>(
                "Price", 15000, 25000, null))
                .Returns(12);

            A.CallTo(() => itemsService.GetItemsFilteredByPrice<Cooker>(
                15000, 25000, 1, 9, "ID", false))
                .Returns(items.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 3, "Cookers"))
                .Returns(true);

            A.CallTo(() => itemsService.GetItemRates(3, "Cookers"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetCookersWithPriceFilter(
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
        public void GetCookersWithRelatedOnes_ShouldReturnAllSections()
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

            var item = new Cooker
            {
                ID = 1,
                Name = "Samsung AC",
                Price = 20000,
                Rate = 4.5,
                Category = category
            };

            A.CallTo(() => itemsService.GetItemCategories<Cooker>())
                .Returns(new List<Category> { category });

            A.CallTo(() => itemsService.GetDiscountedItems<Cooker>(
                1, 10, "ID", false))
                .Returns(new List<Cooker> { item }.AsQueryable());

            A.CallTo(() => itemsService.GetTopRatedItems<Cooker>(
                1, 10, "ID", false))
                .Returns(new List<Cooker> { item }.AsQueryable());

            A.CallTo(() => itemsService.GetLatestItems<Cooker>(
                1, 10, "ID", false))
                .Returns(new List<Cooker> { item }.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 1, "Cookers"))
                .Returns(true);

            A.CallTo(() => itemsService.GetItemRates(1, "Cookers"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = _service.GetCookersWithRelatedOnes();

            // Assert
            Assert.NotNull(result);

            Assert.Single(result.ItemCategories);
            Assert.Single(result.DiscountedItems);
            Assert.Single(result.TopRatedItems);
            Assert.Single(result.latestItems);
        }

        [Fact]
        public async Task GetCookerAllComments_ShouldReturnNull_WhenCookerDoesNotExist()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();

            A.CallTo(() => _unitOfWork.Cookers)
                .Returns(_cookersRepository);

            A.CallTo(() => _cookersRepository.GetById(1))
                .Returns((Cooker)null);

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => itemsService.GetItemRates(1, "Cookers"))
                .Returns(new List<Rate>());

            A.CallTo(() => itemsService.GetItemRateDetails<Cooker>(
                1, "Cookers"))
                .Returns(new int[] { 5, 3, 2 });

            // Act
            var result = await _service.GetCookerAllComments(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCookerAllComments_ShouldReturnNull_WhenThereAreNoComments()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();

            var category = new Category
            {
                ID = 1,
                Name = "Samsung"
            };

            var airConditioner = new Cooker
            {
                ID = 1,
                Name = "Samsung AC",
                Rate = 4.5,
                Category = category
            };

            A.CallTo(() => _unitOfWork.Cookers)
                .Returns(_cookersRepository);

            A.CallTo(() => _cookersRepository.GetById(1))
                .Returns(airConditioner);

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => itemsService.GetItemRates(
                1, "Cookers"))
                .Returns(new List<Rate>());

            A.CallTo(() => itemsService.GetItemRateDetails<Cooker>(
                1, "Cookers"))
                .Returns(new int[] { 5, 3, 2 });

            A.CallTo(() => itemsService.GetItemComments(
                1, "Cookers", "All"))
                .Returns(new List<Comment>());

            // Act
            var result = await _service.GetCookerAllComments(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCookerAllComments_ShouldReturnDto_WhenCommentsExist()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();

            var category = new Category
            {
                ID = 1,
                Name = "Samsung"
            };

            var airConditioner = new Cooker
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

            A.CallTo(() => _unitOfWork.Cookers)
                .Returns(_cookersRepository);

            A.CallTo(() => _cookersRepository.GetById(1))
                .Returns(airConditioner);

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => itemsService.GetItemRates(
                1, "Cookers"))
                .Returns(rates);

            A.CallTo(() => itemsService.GetItemRateDetails<Cooker>(
                1, "Cookers"))
                .Returns(starCounts);

            A.CallTo(() => itemsService.GetItemComments(
                1, "Cookers", "All"))
                .Returns(comments);

            // Act
            var result = await _service.GetCookerAllComments(1);

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
