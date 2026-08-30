using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ApplicationLayer.Services
{
    public class ItemsService : IItemsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ItemsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Item> GetItem(int ID)
        {
            return await _unitOfWork.Items.FindItemByID(ID);
        }

        public async Task<IEnumerable<Item>> GetItems(int pageNumber, int pageSize)
        {
            return await _unitOfWork.Items.GetAllItems(pageNumber, pageSize);
        }

        public async Task<IEnumerable<Item>> GetFilteredItems(List<string> selectedFilters, int pageNumber, int pageSize)
        {
            var items = new List<Item>();

            foreach (var filter in selectedFilters)
            {
                switch (filter)
                {
                    case "Appliances":
                        var appliances = await GetDepartmentItems(filter);
                        items.AddRange(appliances);
                        break;

                    case "Electronics":
                        var electronics = await GetDepartmentItems(filter);
                        items.AddRange(electronics);
                        break;

                    case "Mobile Phones":
                        var mobilePhones = await GetDepartmentItems(filter);
                        items.AddRange(mobilePhones);
                        break;

                    case "Video Games":
                        var videoGames = await GetDepartmentItems(filter);
                        items.AddRange(videoGames);
                        break;

                    case "Air Conditioners":
                        var airConditioners = await _unitOfWork.AirConditioners.GetAll();
                        items.AddRange(airConditioners);
                        break;

                    case "Cookers":
                        var cookers = await _unitOfWork.Cookers.GetAll();
                        items.AddRange(cookers);
                        break;

                    case "Fridges":
                        var fridges = await _unitOfWork.Fridges.GetAll();
                        items.AddRange(fridges);
                        break;

                    case "Washing Machines":
                        var washingMachines = await _unitOfWork.WashingMachines.GetAll();
                        items.AddRange(washingMachines);
                        break;

                    case "Laptops":
                        var laptops = await _unitOfWork.Laptops.GetAll();
                        items.AddRange(laptops);
                        break;

                    case "TVs":
                        var tvs = await _unitOfWork.TVs.GetAll();
                        items.AddRange(tvs);
                        break;

                    case "Headphones":
                        var headphones = await _unitOfWork.HeadPhones.GetAll();
                        items.AddRange(headphones);
                        break;

                    default:
                        break;
                }
            }

            return items.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public IEnumerable<Category> GetItemCategories<T>() where T : Item
        {
            var categoryIds = _unitOfWork.Categories.GetCategoryIDsFromExpression<T>();

            var categories = _unitOfWork.Categories.GetAllByIDs(categoryIds);

            return categories.Any() ? categories : Enumerable.Empty<Category>();
        }

        public async Task<IEnumerable<Category>> GetItemsCategories(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var department = await _unitOfWork.Departments.GetByName(key);

                var itemsCategoriesId = _unitOfWork.CategoryDepartments.GetAllCategoryDepartmentsWithDepartmentID(department.ID)
                    .Select(c => c.CategoryId);

                return _unitOfWork.Categories.GetAllByIDs(itemsCategoriesId);
            }

            throw new ArgumentException();
        }

        public async Task<ProductsDTO> GetAllItemsWithSort(int? page, string? orderIndex, bool? des)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var allItems = await this.GetItems(1, int.MaxValue);

            var totalPages = (int)Math.Ceiling(allItems.Count() / (double)pageSize);

            return new ProductsDTO()
            {
                Items = this.SortItems(allItems, orderIndex ?? "ID", des ?? false)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize),
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                OrderIndex = orderIndex ?? "ID",
                Des = des ?? false
            };
        }

        public async Task<ProductsDTO> GetAllItemsWithSortAndFilter(ProductsDTO? data, int? page, string? filters, string? orderIndex, bool? des)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            data.SelectedFilters = filters?.Split(',').ToList() ?? new List<string>();

            var filteredItems = await this.GetFilteredItems(data.SelectedFilters, 1, int.MaxValue);

            var totalPages = (int)Math.Ceiling(filteredItems.Count() / (double)pageSize);

            return new ProductsDTO()
            {
                Items = _unitOfWork.Items.SortItems(filteredItems, orderIndex ?? "ID", des ?? false)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize),
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                SelectedFilters = data.SelectedFilters,
                OrderIndex = orderIndex ?? "ID",
                Des = des ?? false
            };
        }

        public async Task<ProductsDTO> GetAllItemsWithFilter(ProductsDTO data, string? orderIndex, bool? des)
        {
            var allItems = await this.GetFilteredItems(data.SelectedFilters, 1, int.MaxValue);

            var totalPages = (int)Math.Ceiling(allItems.Count() / (double)10);

            var items = await this.GetFilteredItems(data.SelectedFilters, 1, 10);

            return new ProductsDTO()
            {
                Items = items,
                CurrentPage = 1,
                TotalPages = totalPages,
                SelectedFilters = data.SelectedFilters,
                OrderIndex = orderIndex ?? "ID",
                Des = des ?? false
            };
        }

        public IQueryable<T> GetLatestItems<T>(int pageNumber, int pageSize, string orderKey, bool desOrder) where T : Item
        {
            return desOrder ? _unitOfWork.Items.GetLatestItemsDesOrder<T>(pageNumber, pageSize, orderKey) :
                 _unitOfWork.Items.GetLatestItemsAesOrder<T>(pageNumber, pageSize, orderKey);
        }

        public IQueryable<T> GetDiscountedItems<T>(int pageNumber, int pageSize, string orderKey, bool desOrder) where T : Item
        {
            if (!string.IsNullOrEmpty(orderKey))
            {
                IQueryable<T> data;

                var items = _unitOfWork.Items.ItemDbSet<T>();

                if (desOrder)
                {
                    data = items.Where(i => EF.Property<bool>(i, "IsDiscounted")).Include("Category")
                      .OrderByDescending(i => EF.Property<object>(i, orderKey))
                      .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }
                else
                {
                    data = items.Where(i => EF.Property<bool>(i, "IsDiscounted")).Include("Category")
                      .OrderBy(i => EF.Property<object>(i, orderKey))
                      .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }

                return data.Any() ? data : Enumerable.Empty<T>().AsQueryable();
            }

            else
                throw new ArgumentException();
        }

        public IQueryable<T> GetTopRatedItems<T>(int pageNumber, int pageSize, string orderKey, bool desOrder) where T : Item
        {
            if (!string.IsNullOrEmpty(orderKey))
            {
                IQueryable<T> data;

                var items = _unitOfWork.Items.ItemDbSet<T>();

                if (desOrder)
                {
                    data = items.Where(i => EF.Property<double>(i, "Rate") >= 3.5).Include("Category")
                        .OrderByDescending(i => EF.Property<object>(i, orderKey))
                        .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }
                else
                {
                    data = items.Where(i => EF.Property<double>(i, "Rate") >= 3.5).Include("Category")
                        .OrderBy(i => EF.Property<object>(i, orderKey))
                        .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }
                return data.Any() ? data : Enumerable.Empty<T>().AsQueryable();
            }
            else
                throw new ArgumentException();
        }

        public IQueryable<T> GetItemsFilteredByPrice<T>(int price1, int price2, int pageNumber, int pageSize, string orderKey, bool desOrder) where T : Item
        {
            var dbset = _unitOfWork.Items.ItemDbSet<T>();

            if (desOrder)
            {
                if (price2 == 0)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var items = dbset.Where(mp => EF.Property<double>(mp, "Price") <= price1)
                            .Include("Category")
                            .OrderByDescending(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return items.Any() ? items : Enumerable.Empty<T>().AsQueryable();
                    }
                    else
                        throw new ArgumentException();
                }
                else if (price1 >= 1000 & price2 >= 1000)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var items = dbset.Where(mp => EF.Property<double>(mp, "Price") >= price1 && EF.Property<double>(mp, "Price") <= price2)
                            .Include("Category")
                            .OrderByDescending(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return items.Any() ? items : Enumerable.Empty<T>().AsQueryable();
                    }
                    else
                        throw new ArgumentException();
                }
                else if (price2 == 1)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var items = dbset.Where(mp => EF.Property<double>(mp, "Price") >= price1)
                            .Include("Category")
                            .OrderByDescending(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return items.Any() ? items : Enumerable.Empty<T>().AsQueryable();
                    }
                    else
                        throw new ArgumentException();
                }
                else
                    throw new ArgumentException("An error occurred.");
            }
            else
            {
                if (price2 == 0)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var items = dbset.Where(mp => EF.Property<double>(mp, "Price") <= price1)
                            .Include("Category")
                            .OrderBy(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return items.Any() ? items : Enumerable.Empty<T>().AsQueryable();
                    }
                    else
                        throw new ArgumentException();
                }
                else if (price1 >= 1000 & price2 >= 1000)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var items = dbset.Where(mp => EF.Property<double>(mp, "Price") >= price1 && EF.Property<double>(mp, "Price") <= price2)
                            .Include("Category")
                            .OrderBy(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return items.Any() ? items : Enumerable.Empty<T>().AsQueryable();
                    }
                    else
                        throw new ArgumentException();
                }
                else if (price2 == 1)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var items = dbset.Where(mp => EF.Property<double>(mp, "Price") >= price1)
                            .Include("Category")
                            .OrderBy(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return items.Any() ? items : Enumerable.Empty<T>().AsQueryable();
                    }
                    else
                        throw new ArgumentException();
                }
                else
                    throw new ArgumentException("An error occurred.");
            }
        }

        public IQueryable<T> GetCategoryItems<T>(string name, int pageNumber, int pageSize, string orderKey, bool desOrder) where T : Item
        {
            if (!string.IsNullOrEmpty(orderKey))
            {
                var items = _unitOfWork.Items.ItemDbSet<T>();

                IQueryable<T> data;

                var navCategory = _unitOfWork.Items.FindNavigation<T>("Category");

                if (navCategory != null)
                {
                    items = items.AsNoTracking().Include("Category");
                }

                if (typeof(T).GetProperty("Category") != null)
                {
                    items = items.AsNoTracking()
                        .Where(m => EF.Property<string>(EF.Property<object>(m, "Category"), "Name") == name);
                }

                if (desOrder)
                {
                    data = items.AsNoTracking().OrderByDescending(m => EF.Property<object>(m, orderKey))
                       .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }
                else
                {
                    data = items.AsNoTracking().OrderBy(m => EF.Property<object>(m, orderKey))
                       .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }
                return data.Any() ? data.AsQueryable() : Enumerable.Empty<T>().AsQueryable();
            }
            else
                throw new NotImplementedException();
        }

        public async Task<int> TotalItems<T>(string Key, int? price1 = null, int? price2 = null, string? categoryName = null) where T : Item
        {
            var items = _unitOfWork.Items.ItemDbSet<T>();

            if (Key == "Latest")
            {
                var max = await items.CountAsync();

                var itemsList = _unitOfWork.Items.GetLatestItemsAesOrder<T>(1, max, "ID");
                var result = itemsList.AsNoTracking().Count();

                return result;
            }
            else if (Key == "Rated")
            {
                var max = await items.AsNoTracking().CountAsync();

                var itemsList = GetTopRatedItems<T>(1, max, "ID", false);
                var result = itemsList.AsNoTracking().Count();

                return result;
            }
            else if (Key == "Discounted")
            {
                var max = await items.AsNoTracking().CountAsync();

                var itemsList = GetDiscountedItems<T>(1, max, "ID", false);
                var result = itemsList.AsNoTracking().Count();

                return result;
            }
            else if (Key == "Price")
            {
                var max = await items.AsNoTracking().CountAsync();

                var itemsList = GetItemsFilteredByPrice<T>(price1 ?? 0, price2 ?? 0, 1, max, "ID", false);
                var result = itemsList.AsNoTracking().Count();

                return result;
            }
            else if (Key == "Brands")
            {
                var max = await items.AsNoTracking().CountAsync();

                var itemsList = GetCategoryItems<T>(categoryName ?? "", 1, max, "ID", false);
                var result = itemsList.AsNoTracking().Count();

                return result;
            }
            else
                throw new ArgumentException();
        }

        public async Task<IEnumerable<Comment>> GetItemComments(int id, string type, string key)
        {
            var item = await _unitOfWork.Items.GetById(id);

            if (item is not null)
            {
                if (key == "Default")
                {
                    var comments = (await _unitOfWork.Comments.GetAllComments())
                        .Where(c => c.ItemId == id && c.ItemType == type)
                        .OrderBy(c => c.CommentTime).Take(2);

                    return comments.Any() ? comments : Enumerable.Empty<Comment>();
                }
                else if (key == "All")
                {
                    var comments = (await _unitOfWork.Comments.GetAllComments())
                        .Where(c => c.ItemId == id && c.ItemType == type)
                        .OrderBy(c => c.CommentTime);

                    return comments.Any() ? comments : Enumerable.Empty<Comment>();
                }
            }
            return Enumerable.Empty<Comment>().AsQueryable();
        }

        public async Task<IEnumerable<Rate>> GetItemRates(int id, string type)
        {
            var item = await _unitOfWork.Items.GetById(id);

            if (item is not null)
            {
                var rates = (await _unitOfWork.Rates.GetAll())
                    .Where(r => r.ItemId == id && r.ItemType == type);

                return rates.Any() ? rates : Enumerable.Empty<Rate>().AsQueryable();
            }
            return Enumerable.Empty<Rate>().AsQueryable();
        }

        public async Task<bool> SetRate(Rate data)
        {
            var item = await _unitOfWork.Items.GetById(data.ItemId);

            if (item is not null)
            {
                var entityType = typeof(Item);
                var ratePropInfo = entityType.GetProperty("Rate", BindingFlags.Public | BindingFlags.Instance);

                if (ratePropInfo == null || ratePropInfo.PropertyType != typeof(double))
                    throw new InvalidOperationException(
                        $"{entityType.Name} does not have a public double Rate property.");

                var rates = await this.GetItemRates(data.ItemId, data.ItemType);

                if (rates.Any())
                {
                    var avgRate = rates.Average(r => r.Value);

                    ratePropInfo.SetValue(item, avgRate);

                    _unitOfWork.Items.Update(item);

                    await _unitOfWork.Commit();

                    return true;
                }
            }
            return false;
        }

        public async Task<int[]> GetItemRateDetails<T>(int id, string type) where T : Item
        {
            var items = _unitOfWork.Items.ItemDbSet<T>();

            var item = await items.FirstOrDefaultAsync(x => x.ID == id);

            if (item is not null)
            {
                int[] valuesCount = new int[5];

                var firstValue = (await _unitOfWork.Rates.GetAll())
                    .Where(r => r.ItemId == id && r.ItemType == type && r.Value <= 1).Count();

                var secondValue = (await _unitOfWork.Rates.GetAll())
                    .Where(r => r.ItemId == id && r.ItemType == type && r.Value > 1 && r.Value <= 2).Count();

                var thirdValue = (await _unitOfWork.Rates.GetAll())
                    .Where(r => r.ItemId == id && r.ItemType == type && r.Value > 2 && r.Value <= 3).Count();

                var fourthValue = (await _unitOfWork.Rates.GetAll())
                    .Where(r => r.ItemId == id && r.ItemType == type && r.Value > 3 && r.Value <= 4).Count();

                var fifthValue = (await _unitOfWork.Rates.GetAll())
                    .Where(r => r.ItemId == id && r.ItemType == type && r.Value > 4 && r.Value <= 5).Count();

                valuesCount[0] = firstValue;
                valuesCount[1] = secondValue;
                valuesCount[2] = thirdValue;
                valuesCount[3] = fourthValue;
                valuesCount[4] = fifthValue;

                return valuesCount;
            }
            return Array.Empty<int>();
        }

        public IEnumerable<Item> SortItems(IEnumerable<Item> items, string key, bool des)
        {
            return _unitOfWork.Items.SortItems(items, key, des);
        }

        private async Task<IEnumerable<Item>> GetDepartmentItems(string name)
        {
            var items = new List<Item>();

            switch (name)
            {
                case "Appliances":
                    var airConditioners = await _unitOfWork.AirConditioners.GetAll();
                    items.AddRange(airConditioners);

                    var cookers = await _unitOfWork.Cookers.GetAll();
                    items.AddRange(cookers);

                    var fridges = await _unitOfWork.Fridges.GetAll();
                    items.AddRange(fridges);

                    var washingMachines = await _unitOfWork.WashingMachines.GetAll();
                    items.AddRange(washingMachines);

                    break;

                case "Electronics":
                    var laptops = await _unitOfWork.Laptops.GetAll();
                    items.AddRange(laptops);

                    var headphones = await _unitOfWork.HeadPhones.GetAll();
                    items.AddRange(headphones);

                    var tvs = await _unitOfWork.TVs.GetAll();
                    items.AddRange(tvs);

                    break;

                case "Mobile Phones":
                    var mobilePhones = await _unitOfWork.MobilePhones.GetAll();
                    items.AddRange(mobilePhones);
                    break;

                case "Video Games":
                    var videoGames = await _unitOfWork.VideoGames.GetAll();
                    items.AddRange(videoGames);
                    break;

                default:
                    break;
            }

            return items;
        }
    }
}