using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Xunit;

namespace SouqTests
{
    public class OffersServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        private readonly IOffersRepository _offersRepository;
        private readonly IRepository<Department> _departmentsRepository;
        private readonly ICategoriesRepository _categoriesRepository;

        private readonly IItemsService _itemsService;
        private readonly IDepartmentsService _departmentsService;
        private readonly ICategoriesService _categoriesService;

        private readonly OffersService _service;

        public OffersServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _emailSender = A.Fake<IEmailSender>();
            _servicesInstanceProvider = A.Fake<IServicesInstanceProvider>();

            _offersRepository = A.Fake<IOffersRepository>();
            _departmentsRepository = A.Fake<IRepository<Department>>();
            _categoriesRepository = A.Fake<ICategoriesRepository>();

            _itemsService = A.Fake<IItemsService>();
            _departmentsService = A.Fake<IDepartmentsService>();
            _categoriesService = A.Fake<ICategoriesService>();

            A.CallTo(() => _unitOfWork.Offers)
                .Returns(_offersRepository);

            A.CallTo(() => _unitOfWork.Departments)
                .Returns(_departmentsRepository);

            A.CallTo(() => _unitOfWork.Categories)
                .Returns(_categoriesRepository);

            A.CallTo(() => _servicesInstanceProvider
                .GetItemsServiceInstance())
                .Returns(_itemsService);

            A.CallTo(() => _servicesInstanceProvider
                .GetDepartmentsServiceInstance())
                .Returns(_departmentsService);

            A.CallTo(() => _servicesInstanceProvider
                .GetCategoriesServiceInstance())
                .Returns(_categoriesService);

           
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

            _service = new OffersService(
                _unitOfWork,
                _emailSender,
                _userManager,
                _servicesInstanceProvider);
        }
        
        [Fact]
        public async Task GetOffer_ShouldReturnOffer()
        {
            // Arrange
            var offer = new Offer
            {
                ID = 1
            };

            A.CallTo(() => _offersRepository.GetById(1))
                .Returns(offer);

            // Act
            var result = await _service.GetOffer(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ID);

            A.CallTo(() => _offersRepository.GetById(1))
                .MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public async Task GetAllOffers_ShouldReturnBuyOneGetOneOffer()
        {
            // Arrange
            var offers = new List<Offer>
        {
            new Offer
            {
                ID = 1,
                OfferType = OfferType.BuyOneGetOne,
                ItemOneID = 10,
                ItemTwoID = 20
            }
        };

            A.CallTo(() => _offersRepository.GetAll())
                .Returns(offers);

            // Act
            var result = (await _service.GetAllOffers()).ToList();

            // Assert
            Assert.Single(result);

            Assert.Equal(1, result[0].ID);
            Assert.Equal(OfferType.BuyOneGetOne, result[0].OfferType);
            Assert.Equal(10, result[0].ItemOneID);
            Assert.Equal(20, result[0].ItemTwoID);
        }

        [Fact]
        public async Task GetAllOffers_ShouldReturnDiscountOffer()
        {
            // Arrange
            var offer = new Offer
            {
                ID = 1,
                OfferType = OfferType.FixedDiscount,
                DepartmentName = "Electronics",
                CategoryName = "Laptops",
                ItemID = 10,
                FixedDiscountValue = 100
            };

            A.CallTo(() => _offersRepository.GetAll())
                .Returns(new List<Offer> { offer });

            A.CallTo(() => _itemsService.GetItem(10))
                .Returns(new Laptop
                {
                    ID = 10,
                    Name = "Dell Laptop"
                });

            // Act
            var result = (await _service.GetAllOffers()).ToList();

            // Assert
            Assert.Single(result);

            Assert.Equal(1, result[0].ID);
            Assert.Equal(OfferType.FixedDiscount, result[0].OfferType);
            Assert.Equal(10, result[0].ItemID);
            Assert.Equal("Electronics", result[0].DepartmentName);
        }

        [Fact]
        public async Task GetAllOffers_ShouldReturnPromoCodeOffer()
        {
            // Arrange
            var offer = new Offer
            {
                ID = 1,
                OfferType = OfferType.PromoCode,
                PromoCode = "SAVE20",
                PromoDiscountType = "percentage",
                PromoDiscountValue = 20
            };

            A.CallTo(() => _offersRepository.GetAll())
                .Returns(new List<Offer> { offer });

            // Act
            var result = (await _service.GetAllOffers()).ToList();

            // Assert
            Assert.Single(result);

            Assert.Equal("SAVE20", result[0].PromoCode);
            Assert.Equal("percentage", result[0].PromoDiscountType);
            Assert.Equal(20, result[0].PromoDiscountValue);
        }
        
        [Fact]
        public async Task CreateOffer_FixedDiscount_ShouldCalculateNewPrice()
        {
            // Arrange
            var item = new Laptop
            {
                ID = 1,
                Price = 1000,
                IsDiscounted = false
            };

            var data = new OfferDTO
            {
                OfferType = OfferType.FixedDiscount,
                FixedDiscountValue = 200,
                ItemID = 1,

                ClientFile = A.Fake<IFormFile>()
            };

            A.CallTo(() => _itemsService.GetItem(1))
                .Returns(item);

            A.CallTo(() => _offersRepository.Add(A<Offer>._))
                .Returns(new Offer { ID = 1 });

            // Act
            var result = await _service.CreateOffer(data);

            // Assert
            Assert.True(result.Success);

            Assert.True(item.IsDiscounted);
            Assert.Equal(800, item.NewPrice);

            A.CallTo(() => _offersRepository.Add(A<Offer>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappened();
        }
        
        [Fact]
        public async Task CreateOffer_PercentDiscount_ShouldCalculateNewPrice()
        {
            // Arrange
            var item = new Laptop
            {
                ID = 1,
                Price = 1000
            };

            var data = new OfferDTO
            {
                OfferType = OfferType.PercentDiscount,
                PercentDiscount = 20,
                ItemID = 1,

                ClientFile = A.Fake<IFormFile>()
            };

            A.CallTo(() => _itemsService.GetItem(1))
                .Returns(item);

            A.CallTo(() => _offersRepository.Add(A<Offer>._))
                .Returns(new Offer { ID = 1 });

            // Act
            var result = await _service.CreateOffer(data);

            // Assert
            Assert.True(result.Success);

            // 1000 * (1 - 20 / 100) = 800
            Assert.Equal(800, item.NewPrice);

            Assert.True(item.IsDiscounted);
        }
        
        [Fact]
        public async Task CreateOffer_BOGO_ShouldMarkBuyAndGetItems()
        {
            // Arrange
            var buyItem = new Laptop
            {
                ID = 1
            };

            var getItem = new HeadPhone
            {
                ID = 2
            };

            var data = new OfferDTO
            {
                OfferType = OfferType.BuyOneGetOne,
                ItemOneID = 1,
                ItemTwoID = 2,

                ClientFile = A.Fake<IFormFile>()
            };

            A.CallTo(() => _itemsService.GetItem(1))
                .Returns(buyItem);

            A.CallTo(() => _itemsService.GetItem(2))
                .Returns(getItem);

            A.CallTo(() => _offersRepository.Add(A<Offer>._))
                .Returns(new Offer { ID = 1 });

            // Act
            var result = await _service.CreateOffer(data);

            // Assert
            Assert.True(result.Success);

            Assert.True(buyItem.IsDiscounted);
            Assert.True(buyItem.IsBOGOBuy);

            Assert.True(getItem.IsDiscounted);
            Assert.True(getItem.IsBOGOGet);
        }
        
        [Fact]
        public async Task IsPromoCodeExist_WhenCodeExists_ShouldReturnOffer()
        {
            // Arrange
            var offer = new Offer
            {
                ID = 1,
                OfferType = OfferType.PromoCode,
                PromoCode = "SAVE20"
            };

            A.CallTo(() => _offersRepository.GetAll())
                .Returns(new List<Offer> { offer });

            // Act
            var result = await _service.IsPromoCodeExist("SAVE20");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SAVE20", result.PromoCode);
        }

        [Fact]
        public async Task IsPromoCodeExist_WhenCodeDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            A.CallTo(() => _offersRepository.GetAll())
                .Returns(new List<Offer>());

            // Act
            var result = await _service.IsPromoCodeExist("INVALID");

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task GetBOGOGetItem_WhenItemIsBOGOBuy_ShouldReturnGetItem()
        {
            // Arrange
            var buyItem = new Laptop
            {
                ID = 1,
                IsBOGOBuy = true
            };

            var getItem = new HeadPhone
            {
                ID = 2
            };

            var offer = new Offer
            {
                OfferType = OfferType.BuyOneGetOne,
                ItemOneID = 1,
                ItemTwoID = 2
            };

            A.CallTo(() => _itemsService.GetItem(1))
                .Returns(buyItem);

            A.CallTo(() => _offersRepository.GetAll())
                .Returns(new List<Offer> { offer });

            A.CallTo(() => _itemsService.GetItem(2))
                .Returns(getItem);

            // Act
            var result = await _service.GetBOGOGetItem(buyItem);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.ID);
        }

        [Fact]
        public async Task GetBOGOGetItem_WhenItemIsNotBOGOBuy_ShouldReturnNull()
        {
            // Arrange
            var item = new Laptop
            {
                ID = 1,
                IsBOGOBuy = false
            };

            A.CallTo(() => _itemsService.GetItem(1))
                .Returns(item);

            // Act
            var result = await _service.GetBOGOGetItem(item);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task DeleteOffer_FixedDiscount_ShouldRemoveDiscountFromItem()
        {
            // Arrange
            var item = new Laptop
            {
                ID = 1,
                Price = 1000,
                IsDiscounted = true,
                NewPrice = 800
            };

            var offer = new Offer
            {
                ID = 1,
                OfferType = OfferType.FixedDiscount,
                ItemID = 1
            };

            A.CallTo(() => _offersRepository.GetById(1))
                .Returns(offer);

            A.CallTo(() => _itemsService.GetItem(1))
                .Returns(item);

            A.CallTo(() => _offersRepository.Delete(offer))
                .Returns(offer);

            // Act
            var result = await _service.DeleteOffer(1);

            // Assert
            Assert.True(result.Success);

            Assert.False(item.IsDiscounted);
            Assert.Null(item.NewPrice);

            A.CallTo(() => _offersRepository.Delete(offer))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappened();
        }

        [Fact]
        public async Task DeleteOffer_BOGO_ShouldRemoveBOGOFlags()
        {
            // Arrange
            var buyItem = new Laptop
            {
                ID = 1,
                IsDiscounted = true,
                IsBOGOBuy = true
            };

            var getItem = new HeadPhone
            {
                ID = 2,
                IsDiscounted = true,
                IsBOGOGet = true
            };

            var offer = new Offer
            {
                ID = 1,
                OfferType = OfferType.BuyOneGetOne,
                ItemOneID = 1,
                ItemTwoID = 2
            };

            A.CallTo(() => _offersRepository.GetById(1))
                .Returns(offer);

            A.CallTo(() => _itemsService.GetItem(1))
                .Returns(buyItem);

            A.CallTo(() => _itemsService.GetItem(2))
                .Returns(getItem);

            A.CallTo(() => _offersRepository.Delete(offer))
                .Returns(offer);

            // Act
            var result = await _service.DeleteOffer(1);

            // Assert
            Assert.True(result.Success);

            Assert.False(buyItem.IsDiscounted);
            Assert.False(buyItem.IsBOGOBuy);

            Assert.False(getItem.IsDiscounted);
            Assert.False(getItem.IsBOGOGet);
        }

        [Fact]
        public async Task DeleteOffer_WhenOfferDoesNotExist_ShouldThrow()
        {
            // Arrange
            A.CallTo(() => _offersRepository.GetById(1))
                .Returns((Offer)null);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.DeleteOffer(1));
        }
    }
}
