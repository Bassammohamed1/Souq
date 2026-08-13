using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class MobilePhonesService : IMobilePhonesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public MobilePhonesService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<MobilePhone> GetMobilePhone(int id)
        {
            return await _unitOfWork.MobilePhones.GetById(id);
        }

        public IEnumerable<MobilePhone> GetMobilePhones(int pageNumber, int pageSize)
        {
            return _unitOfWork.MobilePhones.GetAll(pageNumber, pageSize);
        }

        public async Task<Result> Add(MobilePhone mobilePhone)
        {
            var stream = new MemoryStream();
            await mobilePhone.clientFile.CopyToAsync(stream);
            mobilePhone.dbImage = stream.ToArray();

            var result = await _unitOfWork.MobilePhones.Add(mobilePhone);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while adding." };
        }

        public async Task<Result> Update(MobilePhone mobilePhone)
        {
            var stream = new MemoryStream();
            await mobilePhone.clientFile.CopyToAsync(stream);
            mobilePhone.dbImage = stream.ToArray();

            var result = _unitOfWork.MobilePhones.Update(mobilePhone);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while updating." };
        }

        public async Task<Result> Delete(MobilePhone mobilePhone)
        {
            var result = _unitOfWork.MobilePhones.Delete(mobilePhone);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true }
                    : new Result() { Success = false, Error = "An error occured while deleting." };
        }

        public ItemDTO<MobilePhoneDTO> GetMobilePhonesWithRelatedOnes()
        {
            var mobilePhonesCategories = _servicesInstanceProvider.GetItemsServiceInstance().GetItemCategories<MobilePhone>();

            var discountedMobilePhones = (_servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<MobilePhone>(1, 10, "ID", false)).ToList().
                Select(p => new MobilePhoneDTO
                {
                    Id = p.ID,
                    Name = p.Name,
                    Rate = p.Rate,
                    Price = p.Price,
                    NewPrice = p.NewPrice ?? 0,
                    IsDiscounted = p.IsDiscounted,
                    imageSrc = p.imageSrc,
                    RAM = p.RAM,
                    OperatingSystem = p.OperatingSystem,
                    CPUModel = p.CPUModel,
                    MemoryStorageCapacity = p.MemoryStorageCapacity,
                    Color = p.Color,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), p.ID, "MobilePhones").Result,
                    CategoryName = p.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(p.ID, "MobilePhones").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var topRatedMobilePhones = (_servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<MobilePhone>(1, 10, "ID", false)).ToList().
                Select(p => new MobilePhoneDTO
                {
                    Id = p.ID,
                    Name = p.Name,
                    Rate = p.Rate,
                    Price = p.Price,
                    NewPrice = p.NewPrice ?? 0,
                    IsDiscounted = p.IsDiscounted,
                    imageSrc = p.imageSrc,
                    RAM = p.RAM,
                    OperatingSystem = p.OperatingSystem,
                    CPUModel = p.CPUModel,
                    MemoryStorageCapacity = p.MemoryStorageCapacity,
                    Color = p.Color,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), p.ID, "MobilePhones").Result,
                    CategoryName = p.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(p.ID, "MobilePhones").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            var latestMobilePhones = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<MobilePhone>(1, 10, "ID", false).ToList().
                Select(p => new MobilePhoneDTO
                {
                    Id = p.ID,
                    Name = p.Name,
                    Rate = p.Rate,
                    Price = p.Price,
                    NewPrice = p.NewPrice ?? 0,
                    IsDiscounted = p.IsDiscounted,
                    imageSrc = p.imageSrc,
                    RAM = p.RAM,
                    OperatingSystem = p.OperatingSystem,
                    CPUModel = p.CPUModel,
                    MemoryStorageCapacity = p.MemoryStorageCapacity,
                    Color = p.Color,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), p.ID, "MobilePhones").Result,
                    CategoryName = p.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(p.ID, "MobilePhones").Result.Count()
                }).OrderBy(a => Guid.NewGuid());

            return new ItemDTO<MobilePhoneDTO>()
            {
                ItemCategories = mobilePhonesCategories,
                DiscountedItems = discountedMobilePhones,
                latestItems = latestMobilePhones,
                TopRatedItems = topRatedMobilePhones,
            };
        }

        public async Task<ItemsDTO> GetBrandsMobilePhones(string? orderIndex, int? page, string name, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<MobilePhone>("Brands", null, null, name) / (double)pageSize);

            var mobilePhones = (_servicesInstanceProvider.GetItemsServiceInstance().GetCategoryItems<MobilePhone>(name, pageNumber, pageSize, orderIndex ?? "ID", des ?? false)).ToList().
                 Select(p => new MobilePhoneDTO
                 {
                     Id = p.ID,
                     Name = p.Name,
                     Rate = p.Rate,
                     Price = p.Price,
                     NewPrice = p.NewPrice ?? 0,
                     IsDiscounted = p.IsDiscounted,
                     imageSrc = p.imageSrc,
                     RAM = p.RAM,
                     OperatingSystem = p.OperatingSystem,
                     CPUModel = p.CPUModel,
                     MemoryStorageCapacity = p.MemoryStorageCapacity,
                     Color = p.Color,
                     isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), p.ID, "MobilePhones").Result,
                     CategoryName = p.Category.Name,
                     RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(p.ID, "MobilePhones").Result.Count()
                 });

            return new ItemsDTO
            {
                Items = mobilePhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Brands",
                Brand = name
            };
        }

        public async Task<ItemsDTO> GetDiscountedMobilePhones(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<MobilePhone>("Discounted") / (double)pageSize);

            var discountedMobilePhones = _servicesInstanceProvider.GetItemsServiceInstance().GetDiscountedItems<MobilePhone>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(p => new MobilePhoneDTO
                {
                    Id = p.ID,
                    Name = p.Name,
                    Rate = p.Rate,
                    Price = p.Price,
                    NewPrice = p.NewPrice ?? 0,
                    IsDiscounted = p.IsDiscounted,
                    imageSrc = p.imageSrc,
                    RAM = p.RAM,
                    OperatingSystem = p.OperatingSystem,
                    CPUModel = p.CPUModel,
                    MemoryStorageCapacity = p.MemoryStorageCapacity,
                    Color = p.Color,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), p.ID, "MobilePhones").Result,
                    CategoryName = p.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(p.ID, "MobilePhones").Result.Count()
                });

            return new ItemsDTO
            {
                Items = discountedMobilePhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Discounted",
            };
        }

        public async Task<ItemsDTO> GetTopRatedMobilePhones(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<MobilePhone>("Rated") / (double)pageSize);


            var ratedMobilePhones = _servicesInstanceProvider.GetItemsServiceInstance().GetTopRatedItems<MobilePhone>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(p => new MobilePhoneDTO
                {
                    Id = p.ID,
                    Name = p.Name,
                    Rate = p.Rate,
                    Price = p.Price,
                    NewPrice = p.NewPrice ?? 0,
                    IsDiscounted = p.IsDiscounted,
                    imageSrc = p.imageSrc,
                    RAM = p.RAM,
                    OperatingSystem = p.OperatingSystem,
                    CPUModel = p.CPUModel,
                    MemoryStorageCapacity = p.MemoryStorageCapacity,
                    Color = p.Color,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), p.ID, "MobilePhones").Result,
                    CategoryName = p.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(p.ID, "MobilePhones").Result.Count()
                });

            return new ItemsDTO
            {
                Items = ratedMobilePhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "TopRated",
            };
        }

        public async Task<ItemsDTO> GetLatestMobilePhones(string? orderIndex, int? page, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<MobilePhone>("Latest") / (double)pageSize);

            var latestMobilePhones = _servicesInstanceProvider.GetItemsServiceInstance().GetLatestItems<MobilePhone>(pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(p => new MobilePhoneDTO
                {
                    Id = p.ID,
                    Name = p.Name,
                    Rate = p.Rate,
                    Price = p.Price,
                    NewPrice = p.NewPrice ?? 0,
                    IsDiscounted = p.IsDiscounted,
                    imageSrc = p.imageSrc,
                    RAM = p.RAM,
                    OperatingSystem = p.OperatingSystem,
                    CPUModel = p.CPUModel,
                    MemoryStorageCapacity = p.MemoryStorageCapacity,
                    Color = p.Color,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), p.ID, "MobilePhones").Result,
                    CategoryName = p.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(p.ID, "MobilePhones").Result.Count()
                });

            return new ItemsDTO
            {
                Items = latestMobilePhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "Latest",
            };
        }

        public async Task<ItemsDTO> GetMobilePhonesWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await _servicesInstanceProvider.GetItemsServiceInstance().TotalItems<MobilePhone>("Price", price1, price2, null) / (double)pageSize);

            var priceMobilePhones = _servicesInstanceProvider.GetItemsServiceInstance().GetItemsFilteredByPrice<MobilePhone>(price1, price2, pageNumber, pageSize, orderIndex ?? "ID", des ?? false).ToList().
                Select(p => new MobilePhoneDTO
                {
                    Id = p.ID,
                    Name = p.Name,
                    Rate = p.Rate,
                    Price = p.Price,
                    NewPrice = p.NewPrice ?? 0,
                    IsDiscounted = p.IsDiscounted,
                    imageSrc = p.imageSrc,
                    RAM = p.RAM,
                    OperatingSystem = p.OperatingSystem,
                    CPUModel = p.CPUModel,
                    MemoryStorageCapacity = p.MemoryStorageCapacity,
                    Color = p.Color,
                    isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), p.ID, "MobilePhones").Result,
                    CategoryName = p.Category.Name,
                    RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(p.ID, "MobilePhones").Result.Count()
                });

            return new ItemsDTO
            {
                Items = priceMobilePhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "PriceFilter",
                Price1 = price1,
                Price2 = price2
            };
        }

        public async Task<ItemsDTO> GetMobilePhonesWithStorageFilter(string? orderIndex, int? page, int storage, bool? des)
        {
            bool desOrder = des ?? false;
            int pageSize = 9;
            int pageNumber = page ?? 1;
            var totalPages = (int)Math.Ceiling(await this.TotalFilterStoragePhones(storage) / (double)pageSize);

            var storagePhones = (await this.GetPhonesFilteredByStorage(storage, pageNumber, pageSize, orderIndex ?? "ID", des ?? false)).ToList().
                  Select(p => new MobilePhoneDTO
                  {
                      Id = p.ID,
                      Name = p.Name,
                      Rate = p.Rate,
                      Price = p.Price,
                      NewPrice = p.NewPrice ?? 0,
                      IsDiscounted = p.IsDiscounted,
                      imageSrc = p.imageSrc,
                      RAM = p.RAM,
                      OperatingSystem = p.OperatingSystem,
                      CPUModel = p.CPUModel,
                      MemoryStorageCapacity = p.MemoryStorageCapacity,
                      Color = p.Color,
                      isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), p.ID, "MobilePhones").Result,
                      CategoryName = p.Category.Name,
                      RateCount = _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(p.ID, "MobilePhones").Result.Count()
                  });

            return new ItemsDTO
            {
                Items = storagePhones,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Des = des,
                ActionName = "StorageFilter",
                Storage = storage
            };
        }

        public async Task<MobilePhoneDTO> GetMobilePhoneDetails(int id)
        {
            var mobilePhone = await this.GetMobilePhone(id);

            if (mobilePhone != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "MobilePhones", "Default");

                var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "MobilePhones")).Count();

                var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<MobilePhone>(id, "MobilePhones");

                var totalQuantity = await _servicesInstanceProvider.GetCartServiceInstance().TotalItemQuantityInCart(id, "MobilePhones");

                var similarPriceMobilePhones = (await _unitOfWork.MobilePhones.GetAll())
                    .Where(p => p.Price == mobilePhone.Price || Math.Abs(p.Price - mobilePhone.Price) <= 1000)
                    .Select(p => new MobilePhoneDTO
                    {
                        Id = p.ID,
                        Name = p.Name,
                        Rate = p.Rate,
                        Price = p.Price,
                        NewPrice = p.NewPrice ?? 0,
                        IsDiscounted = p.IsDiscounted,
                        imageSrc = p.imageSrc,
                        RAM = p.RAM,
                        OperatingSystem = p.OperatingSystem,
                        CPUModel = p.CPUModel,
                        MemoryStorageCapacity = p.MemoryStorageCapacity,
                        Color = p.Color,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), p.ID, "MobilePhones").Result,
                        CategoryName = p.Category.Name,
                        RateCount = rateCount
                    });

                var relatedMobilePhones = (await _unitOfWork.MobilePhones.GetAll())
                    .Where(p => p.CategoryId == mobilePhone.CategoryId).Take(10)
                    .Select(p => new MobilePhoneDTO
                    {
                        Id = p.ID,
                        Name = p.Name,
                        Rate = p.Rate,
                        Price = p.Price,
                        NewPrice = p.NewPrice ?? 0,
                        IsDiscounted = p.IsDiscounted,
                        imageSrc = p.imageSrc,
                        RAM = p.RAM,
                        OperatingSystem = p.OperatingSystem,
                        CPUModel = p.CPUModel,
                        MemoryStorageCapacity = p.MemoryStorageCapacity,
                        Color = p.Color,
                        isLiked = _servicesInstanceProvider.GetWishingListServiceInstance().HasUserLiked(_userService.GetUserId(), p.ID, "MobilePhones").Result,
                        CategoryName = p.Category.Name,
                        RateCount = rateCount
                    });

                var offers = _servicesInstanceProvider.GetOffersServiceInstance().GetOffers("Mobile Phones", mobilePhone.Category?.Name, mobilePhone.ID);

                var discountValue = string.Empty;

                if (offers.Any())
                    discountValue = offers.First().OfferType == OfferType.PercentDiscount ?
                              $"{offers.First().PercentDiscount}%" :
                              offers.First().OfferType == OfferType.FixedDiscount ? $"{offers.First().FixedDiscountValue} EGP" : null;

                var BOGOGetItem = await _servicesInstanceProvider.GetOffersServiceInstance().GetBOGOGetItem(mobilePhone);

                return new MobilePhoneDTO
                {
                    Id = mobilePhone.ID,
                    Name = mobilePhone.Name,
                    Rate = mobilePhone.Rate,
                    Price = mobilePhone.Price,
                    NewPrice = mobilePhone.NewPrice ?? 0,
                    IsDiscounted = mobilePhone.IsDiscounted,
                    DiscountValue = discountValue,
                    IsBOGOBuy = mobilePhone.IsBOGOBuy,
                    IsBOGOGet = mobilePhone.IsBOGOGet,
                    imageSrc = mobilePhone.imageSrc,
                    RAM = mobilePhone.RAM,
                    OperatingSystem = mobilePhone.OperatingSystem,
                    CPUModel = mobilePhone.CPUModel,
                    MemoryStorageCapacity = mobilePhone.MemoryStorageCapacity,
                    Color = mobilePhone.Color,
                    CategoryName = mobilePhone.Category.Name,
                    RelatedPhones = relatedMobilePhones,
                    SimilarPricePhones = similarPriceMobilePhones,
                    Comments = comments,
                    Offers = offers,
                    BOGOGet = BOGOGetItem,
                    StarCounts = starCounts,
                    RateCount = rateCount,
                    ControllerName = "MobilePhones",
                    TotalQuantity = totalQuantity
                };
            }

            else
                return null;
        }

        public async Task<MobilePhoneDTO> GetMobilePhoneAllComments(int id)
        {
            var MobilePhone = await this.GetMobilePhone(id);

            var rateCount = (await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRates(id, "MobilePhones")).Count();

            var starCounts = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemRateDetails<MobilePhone>(id, "MobilePhones");

            if (MobilePhone != null)
            {
                var comments = await _servicesInstanceProvider.GetItemsServiceInstance().GetItemComments(id, "MobilePhones", "All");

                if (comments.Any())
                {
                    return new MobilePhoneDTO
                    {
                        Id = MobilePhone.ID,
                        Name = MobilePhone.Name,
                        Rate = MobilePhone.Rate,
                        CategoryName = MobilePhone.Category.Name,
                        Comments = comments,
                        StarCounts = starCounts,
                        RateCount = rateCount
                    };
                }
                else
                    return null;
            }
            else
                return null;
        }

        public async Task<IEnumerable<Category>> GetSpecificCategoriesForSelectList()
        {
            return await _servicesInstanceProvider.GetCategoriesServiceInstance().GetSpecificCategories("Mobile Phones");
        }

        private async Task<IEnumerable<MobilePhone>> GetPhonesFilteredByStorage(int storage, int pageNumber, int pageSize, string orderKey, bool desOrder)
        {
            if (desOrder)
            {
                if (storage == 256)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var phones = (await _unitOfWork.MobilePhones.GetAll())
                            .Where(mp => mp.MemoryStorageCapacity >= storage)
                            .OrderByDescending(m => m.GetType().GetProperty(orderKey)!.GetValue(m))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return phones.Any() ? phones : Enumerable.Empty<MobilePhone>();
                    }
                    else
                        throw new ArgumentException();
                }
                else
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var phones = (await _unitOfWork.MobilePhones.GetAll())
                            .Where(mp => mp.MemoryStorageCapacity == storage)
                            .OrderByDescending(m => m.GetType().GetProperty(orderKey)!.GetValue(m))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return phones.Any() ? phones : Enumerable.Empty<MobilePhone>();
                    }
                    else
                        throw new ArgumentException();
                }
            }
            else
            {
                if (storage == 256)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var phones = (await _unitOfWork.MobilePhones.GetAll())
                            .Where(mp => mp.MemoryStorageCapacity >= storage)
                            .OrderBy(m => m.GetType().GetProperty(orderKey)!.GetValue(m))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return phones.Any() ? phones : Enumerable.Empty<MobilePhone>();
                    }
                    else
                        throw new ArgumentException();
                }
                else
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var phones = (await _unitOfWork.MobilePhones.GetAll())
                            .Where(mp => mp.MemoryStorageCapacity == storage)
                            .OrderBy(m => m.GetType().GetProperty(orderKey)!.GetValue(m))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return phones.Any() ? phones : Enumerable.Empty<MobilePhone>();
                    }
                    else
                        throw new ArgumentException();
                }
            }
        }

        private async Task<int> TotalFilterStoragePhones(int Storage)
        {
            var max = (await _unitOfWork.MobilePhones.GetAll()).Count();

            var phonesCount = await GetPhonesFilteredByStorage(Storage, 1, max, "ID", false);

            var result = phonesCount.Count();

            return result;
        }
    }
}