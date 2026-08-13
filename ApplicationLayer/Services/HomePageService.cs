using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Enums;
using DomainLayer.Models;

namespace ApplicationLayer.Services
{
    public class HomePageService : IHomePageService
    {
        private readonly IItemsService _items;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public HomePageService(IItemsService items, IServicesInstanceProvider servicesInstanceProvider)
        {
            _items = items;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<HomePageDTO> GetHomePageRelatedData()
        {
            var departments = await _servicesInstanceProvider.GetDepartmentsServiceInstance().GetDepartments();

            var items = await _items.GetItems(1, int.MaxValue);
            var latestItems = items.OrderByDescending(i => i.AddedOn).Take(8).OrderBy(i => Guid.NewGuid());
            var featuredItems = items.OrderByDescending(i => i.Rate).Take(8).OrderBy(i => Guid.NewGuid());

            var offers = (await _servicesInstanceProvider.GetOffersServiceInstance().GetAllOffers())
                .Where(o => o.OfferType != OfferType.PromoCode);

            return new HomePageDTO()
            {
                Departments = departments ?? Enumerable.Empty<Department>(),
                Latest = latestItems ?? Enumerable.Empty<Item>(),
                Featured = featuredItems ?? Enumerable.Empty<Item>(),
                Offers = offers ?? Enumerable.Empty<OfferDTO>()
            };
        }

        public async Task<string> GetItemType(int ID)
        {
            return (await _items.GetItem(ID))?.GetType().Name;
        }

        public async Task<ItemsDTO> GetAllItems(string categoryName, string? orderIndex, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 9;

            var allItems = (await _items.GetItems(1, int.MaxValue))
                .Where(i => i.Category?.Name == categoryName);

            allItems = _items.SortItems(allItems, orderIndex ?? "ID", false);

            var totalPages = (int)Math.Ceiling(allItems.Count() / (double)pageSize);

            return new ItemsDTO()
            {
                Items = allItems.Skip((pageNumber - 1) * pageSize).Take(pageSize),
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex,
                Brand = categoryName
            };
        }

        public async Task<FilterDTO> GetFilteredItems(string key, int? page, string? orderIndex)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var filterDTO = new FilterDTO()
            {
                SearchPhrase = key,
                CurrentPage = pageNumber,
                OrderIndex = orderIndex ?? "ID"
            };

            var adjusted = key.Split(' ');
            var items = await _items.GetItems(1, int.MaxValue);

            foreach (var word in adjusted)
            {
                filterDTO.MatchedItems = items.Where(i => i.Name.ToLower().Contains(word.ToLower()));
                items = filterDTO.MatchedItems;
            }

            if (filterDTO.MatchedItems.Any())
            {
                var totalPages = (int)Math.Ceiling(filterDTO.MatchedItems.Count() / (double)pageSize);
                filterDTO.TotalPages = totalPages;

                filterDTO.MatchedItems = filterDTO.MatchedItems
                    .OrderBy(i => i.GetType().GetProperty(orderIndex ?? "ID").GetValue(i, null))
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                return filterDTO;
            }

            if (!filterDTO.MatchedItems.Any())
            {
                var departments = await _servicesInstanceProvider.GetDepartmentsServiceInstance().GetDepartments();

                foreach (var word in adjusted)
                {
                    foreach (var department in departments)
                    {
                        if (department.Name.ToLower().Contains(word.ToLower()))
                        {
                            var matched = await _servicesInstanceProvider.GetDepartmentsServiceInstance().GetDepartmentItems(department);

                            var totalPages = (int)Math.Ceiling(matched.Count() / (double)pageSize);
                            filterDTO.TotalPages = totalPages;

                            filterDTO.MatchedItems = matched
                                .OrderBy(i => i.GetType().GetProperty(orderIndex ?? "ID").GetValue(i, null))
                                .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                            return filterDTO;
                        }
                    }
                }
            }

            if (!filterDTO.MatchedItems.Any())
            {
                var categories = await _servicesInstanceProvider.GetCategoriesServiceInstance().GetCategories();

                foreach (var word in adjusted)
                {
                    foreach (var category in categories)
                    {
                        if (category.Name.ToLower().Contains(word.ToLower()))
                        {
                            var matched = await _servicesInstanceProvider.GetCategoriesServiceInstance().GetCategoryItems(category);

                            var totalPages = (int)Math.Ceiling(matched.Count() / (double)pageSize);
                            filterDTO.TotalPages = totalPages;

                            filterDTO.MatchedItems = matched
                                .OrderBy(i => i.GetType().GetProperty(orderIndex ?? "ID").GetValue(i, null))
                                .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                            return filterDTO;
                        }
                    }
                }
            }

            return filterDTO;
        }

        public async Task<HomePageOfferDetailsDTO> GetHomePageOfferDetails(int id)
        {
            var offer = await _servicesInstanceProvider.GetOffersServiceInstance().GetOffer(id);

            if (offer is not null)
            {
                if (offer.OfferType == OfferType.BuyOneGetOne)
                {
                    var itemType = (await _items.GetItem(offer.ItemOneID ?? 0))?.GetType().Name;

                    if (itemType is not null)

                        return new HomePageOfferDetailsDTO() { ActionName = "Details", ItemType = $"{itemType}s", ItemOneID = offer.ItemOneID };
                }
                else if (offer.OfferType == OfferType.FixedDiscount || offer.OfferType == OfferType.PercentDiscount)
                {
                    if (offer.DepartmentName is not null)
                    {
                        var nameAfterSplit = offer.DepartmentName.Split(' ');
                        string controllerName = nameAfterSplit[0];

                        for (int i = 1; i < nameAfterSplit.Length; i++)
                        {
                            controllerName += nameAfterSplit[i];
                        }

                        return new HomePageOfferDetailsDTO() { ActionName = "Index", ControllerName = controllerName };
                    }
                    else if (offer.CategoryName is not null)
                        return new HomePageOfferDetailsDTO() { ActionName = "Items", CategoryName = offer.CategoryName };
                    else
                    {
                        var itemType = (await _items.GetItem(offer.ItemID ?? 0))?.GetType().Name;

                        if (itemType is not null)
                            return new HomePageOfferDetailsDTO() { ActionName = "Details", ItemType = $"{itemType}s", ItemID = offer.ItemID };
                    }
                }
            }
            return new HomePageOfferDetailsDTO() { ActionName = "Index" };
        }
    }
}
