using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ApplicationLayer.Services
{
    public class OffersService : IOffersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public OffersService(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<AppUser> userManager, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<Offer> GetOffer(int id)
        {
            return await _unitOfWork.Offers.GetById(id);
        }

        public async Task<IEnumerable<OfferDTO>> GetAllOffers()
        {
            var offersVM = new List<OfferDTO>();

            var offers = await _unitOfWork.Offers.GetAll();

            foreach (var offer in offers)
            {
                if (offer.OfferType == OfferType.BuyOneGetOne)
                {
                    var offerVM = new OfferDTO()
                    {
                        ID = offer.ID,
                        OfferType = offer.OfferType,
                        ItemOneID = offer.ItemOneID,
                        ItemTwoID = offer.ItemTwoID,
                        ImageSrc = offer.ImageSrc,
                    };

                    offersVM.Add(offerVM);
                }
                else if (offer.OfferType == OfferType.FixedDiscount || offer.OfferType == OfferType.PercentDiscount)
                {
                    var offerItem = await _servicesInstanceProvider.GetItemsServiceInstance().GetItem(offer.ItemID ?? 0);

                    var offerVM = new OfferDTO()
                    {
                        ID = offer.ID,
                        OfferType = offer.OfferType,
                        DepartmentName = offer.DepartmentName,
                        CategoryName = offer.CategoryName,
                        ItemID = offer.ItemID,
                        FixedDiscountValue = offer.FixedDiscountValue,
                        PercentDiscount = offer.PercentDiscount,
                        ImageSrc = offer.ImageSrc,
                    };

                    offersVM.Add(offerVM);
                }
                else
                {
                    var offerVM = new OfferDTO()
                    {
                        ID = offer.ID,
                        OfferType = offer.OfferType,
                        PromoCode = offer.PromoCode,
                        PromoDiscountType = offer.PromoDiscountType,
                        PromoDiscountValue = offer.PromoDiscountValue,
                        ImageSrc = offer.ImageSrc,
                    };

                    offersVM.Add(offerVM);
                }
            }

            return offersVM.Any() ? offersVM : Enumerable.Empty<OfferDTO>();
        }

        public async Task<OfferDTO> GetOfferWithRelatedData()
        {
            var departments = await _servicesInstanceProvider.GetDepartmentsServiceInstance().GetDepartments();

            var categories = await _servicesInstanceProvider.GetDepartmentsServiceInstance().GetAllDepartmentsCategories(departments);

            var items = await _servicesInstanceProvider.GetItemsServiceInstance().GetItems(1, int.MaxValue);

            return new OfferDTO()
            {
                Departments = departments.OrderBy(d => d.Name).ToList(),
                Categories = categories.OrderBy(c => c.Name).ToList(),
                Items = items.OrderBy(i => i.Name).ToList()
            };
        }

        public async Task<OfferDTO> GetOfferWithRelatedData(int id)
        {
            var departments = await _servicesInstanceProvider.GetDepartmentsServiceInstance().GetDepartments();

            var categories = await _servicesInstanceProvider.GetDepartmentsServiceInstance().GetAllDepartmentsCategories(departments);

            var items = await _servicesInstanceProvider.GetItemsServiceInstance().GetItems(1, int.MaxValue);

            var offer = await _unitOfWork.Offers.GetById(id);

            return new OfferDTO()
            {
                ID = offer.ID,
                Departments = departments.OrderBy(d => d.Name).ToList(),
                Categories = categories.OrderBy(c => c.Name).ToList(),
                Items = items.OrderBy(i => i.Name).ToList(),
                OfferType = offer.OfferType,
                DepartmentName = offer.DepartmentName,
                CategoryName = offer.CategoryName,
                ItemID = offer.ItemID,
                FixedDiscountValue = offer.FixedDiscountValue,
                PercentDiscount = offer.PercentDiscount,
                ItemOneID = offer.ItemOneID,
                ItemTwoID = offer.ItemTwoID,
                PromoCode = offer.PromoCode,
                PromoDiscountType = offer.PromoDiscountType,
                PromoDiscountValue = offer.PromoDiscountValue
            };
        }

        public async Task<Result> CreateOffer(OfferDTO data)
        {
            if (data.OfferType == OfferType.FixedDiscount || data.OfferType == OfferType.PercentDiscount)
            {
                var discountedItems = new List<Item>();

                if (!string.IsNullOrEmpty(data.DepartmentName))
                {
                    var department = await _unitOfWork.Departments.GetByName(data.DepartmentName);

                    var departmentItems = await _servicesInstanceProvider.GetDepartmentsServiceInstance().GetDepartmentItems(department);

                    discountedItems.AddRange(departmentItems);
                }

                if (!string.IsNullOrEmpty(data.CategoryName))
                {
                    var category = await _unitOfWork.Categories.GetByName(data.CategoryName);

                    var brandItems = await _servicesInstanceProvider.GetCategoriesServiceInstance().GetCategoryItems(category);

                    discountedItems.AddRange(brandItems);
                }

                if (data.ItemID is not null)
                {
                    var item = await _servicesInstanceProvider.GetItemsServiceInstance().GetItem(data.ItemID ?? 0);

                    discountedItems.Add(item);
                }

                foreach (var item in discountedItems)
                {
                    item.IsDiscounted = true;

                    switch (data.OfferType)
                    {
                        case OfferType.FixedDiscount:
                            item.NewPrice = item.Price - data.FixedDiscountValue;

                            break;

                        case OfferType.PercentDiscount:
                            item.NewPrice = item.Price * (1 - data.PercentDiscount / 100);

                            break;
                    }
                }
            }
            else if (data.OfferType == OfferType.BuyOneGetOne)
            {
                var buyItem = await _servicesInstanceProvider.GetItemsServiceInstance().GetItem(data.ItemOneID ?? 0);
                var getItem = await _servicesInstanceProvider.GetItemsServiceInstance().GetItem(data.ItemTwoID ?? 0);

                buyItem.IsDiscounted = true;
                buyItem.IsBOGOBuy = true;
                getItem.IsDiscounted = true;
                getItem.IsBOGOGet = true;
            }

            var stream = new MemoryStream();
            await data.ClientFile.CopyToAsync(stream);

            var offer = new Offer()
            {
                DepartmentName = data.DepartmentName,
                CategoryName = data.CategoryName,
                ItemID = data.ItemID,
                OfferType = data.OfferType,
                FixedDiscountValue = data.FixedDiscountValue,
                PercentDiscount = data.PercentDiscount,
                ItemOneID = data.ItemOneID,
                ItemTwoID = data.ItemTwoID,
                PromoCode = data.PromoCode,
                PromoDiscountType = data.PromoDiscountType,
                PromoDiscountValue = data.PromoDiscountValue,
                dbImage = stream.ToArray()
            };

            var result = await _unitOfWork.Offers.Add(offer);

            foreach (var user in _userManager.Users)
            {
                await _emailSender.SendEmailAsync(user.Email, "Check our new offer!!", @$"
                 <div style=""font-family: Amiri, serif; max-width: 600px; margin: auto; padding: 20px; 
                     border: 1px solid #e0e0e0; border-radius: 10px; background-color: #fdfdfd;"">
    
                   <h2 style=""color: #e67e22;"">🔥 Hot New Offer Just for You!</h2>
  
                   <p style=""font-size: 16px; color: #555;"">
                    Hello {user.UserName}, we've just launched an exciting new offer you won't want to miss!
                   </p>

                <div style='margin: 20px 0; text-align: center;'>
                      <img src='{offer.ImageSrc}' alt='Offer Banner' 
                      style='width: 100%; max-height: 300px; border-radius: 8px; object-fit: cover;' />
                </div>

                 <p style=""font-size: 15px; color: #666;"">
                   <strong>{offer.OfferType}</strong>
                 </p>

                 <div style=""text-align: center; margin: 30px 0;"">
                 <a href=""https://yourdomain.com"" 
                    style=""padding: 12px 25px; background-color: #e67e22; color: #fff; text-decoration: none; border-radius: 5px;"">
                   View Offer
                 </a>
                 </div>

                 <p style=""font-size: 14px; color: #999;"">
                  This offer is available for a limited time, so act fast!
                 </p>

                <p style=""font-size: 14px; color: #555; margin-top: 30px;"">
                 Stay tuned for more exclusive deals!<br />
                 <strong>Souq.eg</strong>
                 </p>
             </div>");
            }

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true } :
                new Result() { Success = false, Error = "An error occured while creating offer." };
        }

        public async Task<Result> UpdateOffer(OfferDTO data)
        {
            await this.DeleteOffer(data.ID);

            if (data.OfferType == OfferType.FixedDiscount || data.OfferType == OfferType.PercentDiscount)
            {
                var discountedItems = new List<Item>();

                if (!string.IsNullOrEmpty(data.DepartmentName))
                {
                    var department = await _unitOfWork.Departments.GetByName(data.DepartmentName);

                    var departmentItems = await _servicesInstanceProvider.GetDepartmentsServiceInstance().GetDepartmentItems(department);

                    discountedItems.AddRange(departmentItems);
                }

                if (!string.IsNullOrEmpty(data.CategoryName))
                {
                    var category = await _unitOfWork.Categories.GetByName(data.CategoryName);

                    var brandItems = await _servicesInstanceProvider.GetCategoriesServiceInstance().GetCategoryItems(category);

                    discountedItems.AddRange(brandItems);
                }

                if (data.ItemID is not null)
                {
                    var item = await _servicesInstanceProvider.GetItemsServiceInstance().GetItem(data.ItemID ?? 0);

                    discountedItems.Add(item);
                }

                foreach (var item in discountedItems)
                {
                    item.IsDiscounted = true;

                    switch (data.OfferType)
                    {
                        case OfferType.FixedDiscount:
                            item.NewPrice = item.Price - data.FixedDiscountValue;

                            break;

                        case OfferType.PercentDiscount:
                            item.NewPrice = item.Price * (data.PercentDiscount / 100);

                            break;
                    }
                }
            }
            else if (data.OfferType == OfferType.BuyOneGetOne)
            {
                var buyItem = await _servicesInstanceProvider.GetItemsServiceInstance().GetItem(data.ItemOneID ?? 0);
                var getItem = await _servicesInstanceProvider.GetItemsServiceInstance().GetItem(data.ItemTwoID ?? 0);

                buyItem.IsDiscounted = true;
                buyItem.IsBOGOBuy = true;
                getItem.IsDiscounted = true;
                getItem.IsBOGOGet = true;
            }

            var stream = new MemoryStream();
            await data.ClientFile.CopyToAsync(stream);

            var offer = new Offer()
            {
                DepartmentName = data.DepartmentName,
                CategoryName = data.CategoryName,
                ItemID = data.ItemID,
                OfferType = data.OfferType,
                FixedDiscountValue = data.FixedDiscountValue,
                PercentDiscount = data.PercentDiscount,
                ItemOneID = data.ItemOneID,
                ItemTwoID = data.ItemTwoID,
                PromoCode = data.PromoCode,
                PromoDiscountType = data.PromoDiscountType,
                PromoDiscountValue = data.PromoDiscountValue,
                dbImage = stream.ToArray()
            };

            var result = await _unitOfWork.Offers.Add(offer);

            await _unitOfWork.Commit();

            return result is not null ? new Result() { Success = true } :
                new Result() { Success = false, Error = "An error occured while updating. offer." };
        }

        public async Task<Result> DeleteOffer(int ID)
        {
            var offer = await _unitOfWork.Offers.GetById(ID);

            if (offer is not null)
            {
                if (offer.OfferType == OfferType.FixedDiscount || offer.OfferType == OfferType.PercentDiscount)
                {
                    var discountedItems = new List<Item>();

                    if (!string.IsNullOrEmpty(offer.DepartmentName))
                    {
                        var department = await _unitOfWork.Departments.GetByName(offer.DepartmentName);

                        var departmentItems = await _servicesInstanceProvider.GetDepartmentsServiceInstance().GetDepartmentItems(department);

                        discountedItems.AddRange(departmentItems);
                    }

                    if (!string.IsNullOrEmpty(offer.CategoryName))
                    {
                        var category = await _unitOfWork.Categories.GetByName(offer.CategoryName);

                        var brandItems = await _servicesInstanceProvider.GetCategoriesServiceInstance().GetCategoryItems(category);

                        discountedItems.AddRange(brandItems);
                    }

                    if (offer.ItemID is not null)
                    {
                        var item = await _servicesInstanceProvider.GetItemsServiceInstance().GetItem(offer.ItemID ?? 0);

                        discountedItems.Add(item);
                    }

                    foreach (var item in discountedItems)
                    {
                        item.IsDiscounted = false;
                        item.NewPrice = null;
                    }
                }
                else if (offer.OfferType == OfferType.BuyOneGetOne)
                {
                    var buyItem = await _servicesInstanceProvider.GetItemsServiceInstance().GetItem(offer.ItemOneID ?? 0);
                    var getItem = await _servicesInstanceProvider.GetItemsServiceInstance().GetItem(offer.ItemTwoID ?? 0);

                    buyItem.IsDiscounted = false;
                    buyItem.IsBOGOBuy = false;
                    getItem.IsDiscounted = false;
                    getItem.IsBOGOGet = false;
                }

                var result = _unitOfWork.Offers.Delete(offer);

                await _unitOfWork.Commit();

                return result is not null ? new Result() { Success = true } :
                    new Result() { Success = false, Error = "An error occured while deleting. offer." };
            }

            throw new InvalidOperationException();
        }

        public async Task<Item> GetBOGOGetItem(Item item)
        {
            var buyItem = await _servicesInstanceProvider.GetItemsServiceInstance().GetItem(item.ID);

            if (buyItem.IsBOGOBuy)
            {
                var BOGOOffer = (await _unitOfWork.Offers.GetAll())
                    .FirstOrDefault(o => o.OfferType == OfferType.BuyOneGetOne && o.ItemOneID == buyItem.ID);

                var getItem = await _servicesInstanceProvider.GetItemsServiceInstance().GetItem(BOGOOffer.ItemTwoID ?? 0);

                return getItem;
            }

            return null;
        }

        public IQueryable<Offer> GetOffers(string? department, string? category, int? itemID)
        {
            var offers = new List<Offer>();

            if (department is not null)
            {
                var departmentOffers = _unitOfWork.Offers.GetOffersWithDepartmentName(department);

                offers.AddRange(departmentOffers);
            }

            if (category is not null)
            {
                var categoryOffers = _unitOfWork.Offers.GetOffersWithCategoryName(category);

                offers.AddRange(categoryOffers);
            }

            if (itemID is not null)
            {
                var itemOffers = _unitOfWork.Offers.GetOffersWithItemID(itemID ?? 0);

                offers.AddRange(itemOffers);
            }

            return offers.Any() ? offers.Distinct().AsQueryable() : Enumerable.Empty<Offer>().AsQueryable();
        }

        public async Task<Offer> IsPromoCodeExist(string promoCode)
        {
            var promoCodeOffer = (await _unitOfWork.Offers.GetAll())
                .FirstOrDefault(o => o.OfferType == OfferType.PromoCode && o.PromoCode == promoCode);

            return promoCodeOffer;
        }
    }
}