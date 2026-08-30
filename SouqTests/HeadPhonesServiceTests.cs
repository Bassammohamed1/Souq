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
    public class HeadPhonesServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        private readonly IRepository<HeadPhone> _HeadPhonesRepository;

        private readonly HeadPhonesService _service;

        public HeadPhonesServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userService = A.Fake<IUsersService>();
            _servicesInstanceProvider = A.Fake<IServicesInstanceProvider>();

            _HeadPhonesRepository = A.Fake<IRepository<HeadPhone>>();

            A.CallTo(() => _unitOfWork.HeadPhones)
                .Returns(_HeadPhonesRepository);

            _service = new HeadPhonesService(
                _unitOfWork,
                _userService,
                _servicesInstanceProvider);
        }

        [Fact]
        public async Task GetHeadPhone_ShouldReturnHeadPhone()
        {
            // Arrange
            var HeadPhone = new HeadPhone
            {
                ID = 1,
                Name = "Samsung"
            };

            A.CallTo(() => _HeadPhonesRepository.GetById(1))
                .Returns(HeadPhone);

            // Act
            var result = await _service.GetHeadPhone(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ID);
            Assert.Equal("Samsung", result.Name);
        }

        [Fact]
        public async Task GetHeadPhone_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            A.CallTo(() => _HeadPhonesRepository.GetById(1))
                .Returns((HeadPhone)null);

            // Act
            var result = await _service.GetHeadPhone(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetHeadPhones_ShouldReturnHeadPhones()
        {
            // Arrange
            var HeadPhones = new List<HeadPhone>
        {
            new HeadPhone { ID = 1, Name = "Samsung" },
            new HeadPhone { ID = 2, Name = "LG" }
        };

            A.CallTo(() => _HeadPhonesRepository.GetAll(1, 10))
                .Returns(HeadPhones.ToPagedList());

            // Act
            var result = _service.GetHeadPhones(1, 10);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().ID);
        }

        [Fact]
        public async Task Add_ShouldReturnSuccess_WhenRepositoryAddsSuccessfully()
        {
            // Arrange
            var HeadPhone = new HeadPhone
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

            HeadPhone.clientFile = file;

            A.CallTo(() => _HeadPhonesRepository.Add(HeadPhone))
                .Returns(HeadPhone);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Add(HeadPhone);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() => _HeadPhonesRepository.Add(HeadPhone))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Add_ShouldReturnFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            var HeadPhone = new HeadPhone
            {
                ID = 1
            };

            var file = A.Fake<IFormFile>();

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Returns(Task.CompletedTask);

            HeadPhone.clientFile = file;

            A.CallTo(() => _HeadPhonesRepository.Add(HeadPhone))
                .Returns((HeadPhone)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Add(HeadPhone);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("An error occured while adding.", result.Error);
        }

        [Fact]
        public async Task Update_ShouldReturnSuccess_WhenRepositoryUpdatesSuccessfully()
        {
            // Arrange
            var HeadPhone = new HeadPhone
            {
                ID = 1
            };

            var file = A.Fake<IFormFile>();

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Returns(Task.CompletedTask);

            HeadPhone.clientFile = file;

            A.CallTo(() => _HeadPhonesRepository.Update(HeadPhone))
                .Returns(HeadPhone);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Update(HeadPhone);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() => _HeadPhonesRepository.Update(HeadPhone))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            var HeadPhone = new HeadPhone
            {
                ID = 1
            };

            var file = A.Fake<IFormFile>();

            A.CallTo(() => file.CopyToAsync(A<Stream>._, A<CancellationToken>._))
                .Returns(Task.CompletedTask);

            HeadPhone.clientFile = file;

            A.CallTo(() => _HeadPhonesRepository.Update(HeadPhone))
                .Returns((HeadPhone)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Update(HeadPhone);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("An error occured while updating.", result.Error);
        }

        [Fact]
        public async Task Delete_ShouldReturnSuccess_WhenRepositoryDeletesSuccessfully()
        {
            // Arrange
            var HeadPhone = new HeadPhone
            {
                ID = 1
            };

            A.CallTo(() => _HeadPhonesRepository.Delete(HeadPhone))
                .Returns(HeadPhone);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Delete(HeadPhone);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() => _HeadPhonesRepository.Delete(HeadPhone))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Delete_ShouldReturnFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            var HeadPhone = new HeadPhone
            {
                ID = 1
            };

            A.CallTo(() => _HeadPhonesRepository.Delete(HeadPhone))
                .Returns((HeadPhone)null);

            A.CallTo(() => _unitOfWork.Commit())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.Delete(HeadPhone);

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

            A.CallTo(() => categoriesService.GetSpecificCategories("Electronics"))
                .Returns(categories);

            // Act
            var result = await _service.GetSpecificCategoriesForSelectList();

            // Assert
            Assert.Equal(2, result.Count());

            A.CallTo(() => categoriesService.GetSpecificCategories("Electronics"))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetBrandsHeadPhones_ShouldReturnCorrectData()
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

            var HeadPhones = new List<HeadPhone>
    {
        new HeadPhone
        {
            ID = 1,
            Name = "Samsung AC",
            Price = 20000,
            NewPrice = 18000,
            Rate = 4.5,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<HeadPhone>(
                "Brands", null, null, "Samsung"))
                .Returns(10);

            A.CallTo(() => itemsService.GetCategoryItems<HeadPhone>(
                "Samsung", 1, 9, "ID", false))
                .Returns(HeadPhones.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 1, "HeadPhones"))
                .Returns(true);

            A.CallTo(() => itemsService.GetItemRates(1, "HeadPhones"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetBrandsHeadPhones(
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
        public async Task GetDiscountedHeadPhones_ShouldReturnDiscountedItems()
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

            var items = new List<HeadPhone>
    {
        new HeadPhone
        {
            ID = 1,
            Name = "Samsung AC",
            Price = 20000,
            NewPrice = 17000,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<HeadPhone>("Discounted", null, null, null))
                .Returns(18);

            A.CallTo(() => itemsService.GetDiscountedItems<HeadPhone>(
                1, 9, "ID", false))
                .Returns(items.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 1, "HeadPhones"))
                .Returns(false);

            A.CallTo(() => itemsService.GetItemRates(1, "HeadPhones"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetDiscountedHeadPhones(
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
        public async Task GetTopRatedHeadPhones_ShouldReturnTopRatedItems()
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

            var items = new List<HeadPhone>
    {
        new HeadPhone
        {
            ID = 5,
            Name = "LG AC",
            Price = 25000,
            Rate = 4.9,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<HeadPhone>("Rated", null, null, null))
                .Returns(10);

            A.CallTo(() => itemsService.GetTopRatedItems<HeadPhone>(
                1, 9, "ID", false))
                .Returns(items.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 5, "HeadPhones"))
                .Returns(true);

            A.CallTo(() => itemsService.GetItemRates(5, "HeadPhones"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetTopRatedHeadPhones(
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
        public async Task GetLatestHeadPhones_ShouldReturnLatestItems()
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

            var items = new List<HeadPhone>
    {
        new HeadPhone
        {
            ID = 10,
            Name = "Tornado AC",
            Price = 15000,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<HeadPhone>("Latest", null, null, null))
                .Returns(9);

            A.CallTo(() => itemsService.GetLatestItems<HeadPhone>(
                1, 9, "ID", false))
                .Returns(items.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 10, "HeadPhones"))
                .Returns(false);

            A.CallTo(() => itemsService.GetItemRates(10, "HeadPhones"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetLatestHeadPhones(
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
        public async Task GetHeadPhonesWithPriceFilter_ShouldReturnFilteredItems()
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

            var items = new List<HeadPhone>
    {
        new HeadPhone
        {
            ID = 3,
            Name = "Samsung AC",
            Price = 20000,
            Category = category
        }
    };

            A.CallTo(() => itemsService.TotalItems<HeadPhone>(
                "Price", 15000, 25000, null))
                .Returns(12);

            A.CallTo(() => itemsService.GetItemsFilteredByPrice<HeadPhone>(
                15000, 25000, 1, 9, "ID", false))
                .Returns(items.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 3, "HeadPhones"))
                .Returns(true);

            A.CallTo(() => itemsService.GetItemRates(3, "HeadPhones"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = await _service.GetHeadPhonesWithPriceFilter(
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
        public void GetHeadPhonesWithRelatedOnes_ShouldReturnAllSections()
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

            var item = new HeadPhone
            {
                ID = 1,
                Name = "Samsung AC",
                Price = 20000,
                Rate = 4.5,
                Category = category
            };

            A.CallTo(() => itemsService.GetItemCategories<HeadPhone>())
                .Returns(new List<Category> { category });

            A.CallTo(() => itemsService.GetDiscountedItems<HeadPhone>(
                1, 10, "ID", false))
                .Returns(new List<HeadPhone> { item }.AsQueryable());

            A.CallTo(() => itemsService.GetTopRatedItems<HeadPhone>(
                1, 10, "ID", false))
                .Returns(new List<HeadPhone> { item }.AsQueryable());

            A.CallTo(() => itemsService.GetLatestItems<HeadPhone>(
                1, 10, "ID", false))
                .Returns(new List<HeadPhone> { item }.AsQueryable());

            A.CallTo(() => wishlistService.HasUserLiked(
                A<string>._, 1, "HeadPhones"))
                .Returns(true);

            A.CallTo(() => itemsService.GetItemRates(1, "HeadPhones"))
                .Returns(new List<Rate>());

            A.CallTo(() => _userService.GetUserId())
                .Returns("user1");

            // Act
            var result = _service.GetHeadPhonesWithRelatedOnes();

            // Assert
            Assert.NotNull(result);

            Assert.Single(result.ItemCategories);
            Assert.Single(result.DiscountedItems);
            Assert.Single(result.TopRatedItems);
            Assert.Single(result.latestItems);
        }

        [Fact]
        public async Task GetHeadPhoneAllComments_ShouldReturnNull_WhenHeadPhoneDoesNotExist()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();

            A.CallTo(() => _unitOfWork.HeadPhones)
                .Returns(_HeadPhonesRepository);

            A.CallTo(() => _HeadPhonesRepository.GetById(1))
                .Returns((HeadPhone)null);

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => itemsService.GetItemRates(1, "HeadPhones"))
                .Returns(new List<Rate>());

            A.CallTo(() => itemsService.GetItemRateDetails<HeadPhone>(
                1, "HeadPhones"))
                .Returns(new int[] { 5, 3, 2 });

            // Act
            var result = await _service.GetHeadPhoneAllComments(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetHeadPhoneAllComments_ShouldReturnNull_WhenThereAreNoComments()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();

            var category = new Category
            {
                ID = 1,
                Name = "Samsung"
            };

            var HeadPhone = new HeadPhone
            {
                ID = 1,
                Name = "Samsung AC",
                Rate = 4.5,
                Category = category
            };

            A.CallTo(() => _unitOfWork.HeadPhones)
                .Returns(_HeadPhonesRepository);

            A.CallTo(() => _HeadPhonesRepository.GetById(1))
                .Returns(HeadPhone);

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => itemsService.GetItemRates(
                1, "HeadPhones"))
                .Returns(new List<Rate>());

            A.CallTo(() => itemsService.GetItemRateDetails<HeadPhone>(
                1, "HeadPhones"))
                .Returns(new int[] { 5, 3, 2 });

            A.CallTo(() => itemsService.GetItemComments(
                1, "HeadPhones", "All"))
                .Returns(new List<Comment>());

            // Act
            var result = await _service.GetHeadPhoneAllComments(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetHeadPhoneAllComments_ShouldReturnDto_WhenCommentsExist()
        {
            // Arrange
            var itemsService = A.Fake<IItemsService>();

            var category = new Category
            {
                ID = 1,
                Name = "Samsung"
            };

            var HeadPhone = new HeadPhone
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

            A.CallTo(() => _unitOfWork.HeadPhones)
                .Returns(_HeadPhonesRepository);

            A.CallTo(() => _HeadPhonesRepository.GetById(1))
                .Returns(HeadPhone);

            A.CallTo(() => _servicesInstanceProvider.GetItemsServiceInstance())
                .Returns(itemsService);

            A.CallTo(() => itemsService.GetItemRates(
                1, "HeadPhones"))
                .Returns(rates);

            A.CallTo(() => itemsService.GetItemRateDetails<HeadPhone>(
                1, "HeadPhones"))
                .Returns(starCounts);

            A.CallTo(() => itemsService.GetItemComments(
                1, "HeadPhones", "All"))
                .Returns(comments);

            // Act
            var result = await _service.GetHeadPhoneAllComments(1);

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
