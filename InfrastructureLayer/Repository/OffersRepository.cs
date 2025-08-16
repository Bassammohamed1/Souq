using DomainLayer.DTOs;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repository
{
    public class OffersRepository : IOffersRepository
    {
        private readonly AppDbContext _context;
        private readonly IDepartmentsRepository _departments;
        private readonly ICategoriesRepository _categories;
        private readonly IItemsRepository _items;

        public OffersRepository(AppDbContext context, IDepartmentsRepository departments, ICategoriesRepository categories, IItemsRepository items)
        {
            _context = context;
            _departments = departments;
            _categories = categories;
            _items = items;
        }

        public async Task<Offer> FindOfferByID(int id)
        {
            var offer = await _context.Offers.FindAsync(id);

            return offer;
        }

        public async Task CreateOffer(OfferDTO data)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (data.OfferType == OfferType.FixedDiscount || data.OfferType == OfferType.PercentDiscount)
                {
                    var discountedItems = new List<Item>();

                    if (!string.IsNullOrEmpty(data.DepartmentName))
                    {
                        var department = await _context.Departments.Where(d => d.Name == data.DepartmentName).FirstOrDefaultAsync();

                        var departmentItems = await _departments.GetDepartmentItems(department);

                        discountedItems.AddRange(departmentItems);
                    }

                    if (!string.IsNullOrEmpty(data.CategoryName))
                    {
                        var category = await _context.Categories.Where(d => d.Name == data.CategoryName).FirstOrDefaultAsync();

                        var brandItems = await _categories.GetCategoryItems(category);

                        discountedItems.AddRange(brandItems);
                    }

                    if (data.ItemID is not null)
                    {
                        var item = await _items.FindItemByID(data.ItemID ?? 0);

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

                        var s = await _context.SaveChangesAsync();
                    }
                }
                else if (data.OfferType == OfferType.BuyOneGetOne)
                {
                    var buyItem = await _items.FindItemByID(data.ItemOneID ?? 0);
                    var getItem = await _items.FindItemByID(data.ItemTwoID ?? 0);

                    buyItem.IsDiscounted = true;
                    buyItem.IsBOGOBuy = true;
                    getItem.IsDiscounted = true;
                    getItem.IsBOGOGet = true;

                    await _context.SaveChangesAsync();
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

                await _context.Offers.AddAsync(offer);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
            }
        }

        public async Task UpdateOffer(OfferDTO data)
        {
            await this.DeleteOffer(data.ID);

            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (data.OfferType == OfferType.FixedDiscount || data.OfferType == OfferType.PercentDiscount)
                {
                    var discountedItems = new List<Item>();

                    if (!string.IsNullOrEmpty(data.DepartmentName))
                    {
                        var department = await _context.Departments.Where(d => d.Name == data.DepartmentName).FirstOrDefaultAsync();

                        var departmentItems = await _departments.GetDepartmentItems(department);

                        discountedItems.AddRange(departmentItems);
                    }

                    if (!string.IsNullOrEmpty(data.CategoryName))
                    {
                        var category = await _context.Categories.Where(d => d.Name == data.CategoryName).FirstOrDefaultAsync();

                        var brandItems = await _categories.GetCategoryItems(category);

                        discountedItems.AddRange(brandItems);
                    }

                    if (data.ItemID is not null)
                    {
                        var item = await _items.FindItemByID(data.ItemID ?? 0);

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

                        await _context.SaveChangesAsync();
                    }
                }
                else if (data.OfferType == OfferType.BuyOneGetOne)
                {
                    var buyItem = await _items.FindItemByID(data.ItemOneID ?? 0);
                    var getItem = await _items.FindItemByID(data.ItemTwoID ?? 0);

                    buyItem.IsDiscounted = true;
                    buyItem.IsBOGOBuy = true;
                    getItem.IsDiscounted = true;
                    getItem.IsBOGOGet = true;

                    await _context.SaveChangesAsync();
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

                await _context.Offers.AddAsync(offer);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
            }
        }

        public async Task DeleteOffer(int ID)
        {
            var offer = await _context.Offers.FindAsync(ID);

            if (offer is not null)
            {
                var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    if (offer.OfferType == OfferType.FixedDiscount || offer.OfferType == OfferType.PercentDiscount)
                    {
                        var discountedItems = new List<Item>();

                        if (!string.IsNullOrEmpty(offer.DepartmentName))
                        {
                            var department = await _context.Departments
                                .Where(d => d.Name == offer.DepartmentName)
                                .FirstOrDefaultAsync();

                            var departmentItems = await _departments.GetDepartmentItems(department);

                            discountedItems.AddRange(departmentItems);
                        }

                        if (!string.IsNullOrEmpty(offer.CategoryName))
                        {
                            var category = await _context.Categories
                                .Where(d => d.Name == offer.CategoryName)
                                .FirstOrDefaultAsync();

                            var brandItems = await _categories.GetCategoryItems(category);

                            discountedItems.AddRange(brandItems);
                        }

                        if (offer.ItemID is not null)
                        {
                            var item = await _items.FindItemByID(offer.ItemID ?? 0);

                            discountedItems.Add(item);
                        }

                        foreach (var item in discountedItems)
                        {
                            item.IsDiscounted = false;
                            item.NewPrice = null;

                            var result = await _context.SaveChangesAsync();
                        }
                    }
                    else if (offer.OfferType == OfferType.BuyOneGetOne)
                    {
                        var buyItem = await _items.FindItemByID(offer.ItemOneID ?? 0);
                        var getItem = await _items.FindItemByID(offer.ItemTwoID ?? 0);

                        buyItem.IsDiscounted = false;
                        buyItem.IsBOGOBuy = false;
                        getItem.IsDiscounted = false;
                        getItem.IsBOGOGet = false;

                        await _context.SaveChangesAsync();
                    }

                    _context.Offers.Remove(offer);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return;
                }
            }

            throw new InvalidOperationException();
        }

        public async Task<Item> GetBOGOGetItem(Item item)
        {
            var allItems = await _items.GetAll(1, int.MaxValue);

            var buyItem = allItems.FirstOrDefault(i => i.ID == item.ID);

            if (buyItem.IsBOGOBuy)
            {
                var BOGOOffer = await _context.Offers
                    .Where(o => o.OfferType == OfferType.BuyOneGetOne && o.ItemOneID == buyItem.ID).FirstOrDefaultAsync();

                var getItem = allItems.FirstOrDefault(i => i.ID == BOGOOffer.ItemTwoID);

                return getItem;
            }
            return null;
        }

        public Task<IQueryable<Offer>> GetOffers(string? department, string? category, int? itemID)
        {
            var offers = new List<Offer>();

            if (department is not null)
            {
                var departmentOffers = _context.Offers
                    .Where(o => o.DepartmentName == department);

                offers.AddRange(departmentOffers);
            }

            if (category is not null)
            {
                var categoryOffers = _context.Offers
                    .Where(o => o.CategoryName == category);

                offers.AddRange(categoryOffers);
            }

            if (itemID is not null)
            {
                var itemOffers = _context.Offers
                    .Where(o => o.ItemID == itemID || o.ItemOneID == itemID);

                offers.AddRange(itemOffers);
            }

            return Task.FromResult(offers.Any() ? offers.Distinct().AsQueryable() : Enumerable.Empty<Offer>().AsQueryable());
        }

        public async Task<Offer> IsPromoCodeExist(string promoCode)
        {
            var promoCodeOffer = await _context.Offers.AsNoTracking().Where(o => o.OfferType == OfferType.PromoCode && o.PromoCode == promoCode).FirstOrDefaultAsync();

            return promoCodeOffer;
        }

        public async Task<IEnumerable<OfferDTO>> GetAllOffers()
        {
            var offersVM = new List<OfferDTO>();

            var offers = await _context.Offers.ToListAsync();

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
                    var offerItem = await _items.FindItemByID(offer.ItemID ?? 0);

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
    }
}