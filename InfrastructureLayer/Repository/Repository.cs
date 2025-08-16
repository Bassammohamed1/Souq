using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using X.PagedList;
using X.PagedList.Extensions;

namespace InfrastructureLayer.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public Task<IPagedList<T>> GetAll(int pageNumber, int pageSize)
        {
            var result = _context.Set<T>().AsNoTracking()
               .OrderBy(i => EF.Property<string>(i, "Name"))
               .ToPagedList(pageNumber, pageSize);

            return Task.FromResult(result.Any() ? result : Enumerable.Empty<T>().ToPagedList());
        }

        public async Task<IEnumerable<T>> GetAllWithoutPagination()
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();

            var navCategory = _context.Model
                .FindEntityType(typeof(T))?
                .FindNavigation("Category");

            if (navCategory != null)
            {
                query = query.AsNoTracking().AsSplitQuery().Include("Category");
            }

            var result = await query.AsNoTracking().ToListAsync();

            return result.Any() ? result : Enumerable.Empty<T>();
        }

        public async Task<T> GetById(int id)
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();

            var navCategory = _context.Model
                .FindEntityType(typeof(T))?
                .FindNavigation("Category");

            if (navCategory != null)
            {
                query = query.AsNoTracking().AsSplitQuery().Include("Category");
            }

            return await query.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, "ID") == id);
        }

        public async Task Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public Task Update(T entity)
        {
            return Task.FromResult(_context.Set<T>().Update(entity));
        }

        public Task Delete(T entity)
        {
            return Task.FromResult(_context.Set<T>().Remove(entity));
        }

        public Task<IQueryable<Category>> GetItemCategories()
        {
            var entityType = typeof(T);
            var propertyInfo = entityType.GetProperty("CategoryId", BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo == null || propertyInfo.PropertyType != typeof(int))
            {
                throw new InvalidOperationException($"{entityType.Name} does not have an int property named 'CategoryId'.");
            }

            var parameter = Expression.Parameter(entityType, "x");
            var propertyAccess = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var dbSet = _context.Set<T>().AsQueryable();

            var selectMethod = typeof(Queryable).GetMethods()
                .First(m =>
                m.Name == "Select" &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
                .MakeGenericMethod(entityType, typeof(int));


            var selectedQuery = selectMethod.Invoke(null, new object[] { dbSet, lambda });

            var distinctMethod = typeof(Queryable).GetMethods()
                .Where(m => m.Name == "Distinct" && m.GetParameters().Length == 1)
                .Single()
                .MakeGenericMethod(typeof(int));

            var distinctQuery = distinctMethod.Invoke(null, new object[] { selectedQuery });

            var toListMethod = typeof(Enumerable).GetMethod("ToList")
                .MakeGenericMethod(typeof(int));

            var result = toListMethod.Invoke(null, new object[] { distinctQuery });

            var categoryIds = (List<int>)result;

            var categories = _context.Categories.AsNoTracking().Where(c => categoryIds.Contains(c.ID));

            return Task.FromResult(categories.Any() ? categories : Enumerable.Empty<Category>().AsQueryable());
        }

        public async Task<IQueryable<Category>> GetItemsCategories(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var department = await _context.Departments.AsNoTracking().SingleOrDefaultAsync(d => d.Name == key);

                var itemsCategoriesId = _context.CategoryDepartments.AsNoTracking().Where(d => d.DepartmentId == department.ID).Select(c => c.CategoryId);

                var categories = _context.Categories.AsNoTracking().Where(c => itemsCategoriesId.Contains(c.ID));

                return categories.Any() ? categories : Enumerable.Empty<Category>().AsQueryable();
            }
            throw new ArgumentException();
        }

        public Task<IQueryable<T>> GetLatestItems(int pageNumber, int pageSize, string orderKey, bool desOrder)
        {
            if (!string.IsNullOrEmpty(orderKey))
            {
                IQueryable<T> data;

                if (desOrder)
                {
                    data = _context.Set<T>().AsNoTracking().AsSplitQuery()
                       .OrderByDescending(i => EF.Property<object>(i, "AddedOn"))
                       .Include("Category")
                       .OrderByDescending(i => EF.Property<object>(i, orderKey))
                       .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }
                else
                {
                    data = _context.Set<T>().AsNoTracking().AsSplitQuery()
                       .OrderByDescending(i => EF.Property<object>(i, "AddedOn"))
                       .Include("Category")
                       .OrderBy(i => EF.Property<object>(i, orderKey))
                       .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }

                return Task.FromResult(data.Any() ? data : Enumerable.Empty<T>().AsQueryable());
            }
            else
                throw new ArgumentException();
        }

        public Task<IQueryable<T>> GetDiscountedItems(int pageNumber, int pageSize, string orderKey, bool desOrder)
        {
            if (!string.IsNullOrEmpty(orderKey))
            {
                IQueryable<T> data;

                if (desOrder)
                {
                    data = _context.Set<T>().AsNoTracking().AsSplitQuery()
                      .Where(i => EF.Property<bool>(i, "IsDiscounted")).Include("Category")
                      .OrderByDescending(i => EF.Property<object>(i, orderKey))
                      .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }
                else
                {
                    data = _context.Set<T>().AsNoTracking().AsSplitQuery()
                      .Where(i => EF.Property<bool>(i, "IsDiscounted")).Include("Category")
                      .OrderBy(i => EF.Property<object>(i, orderKey))
                      .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }
                return Task.FromResult(data.Any() ? data : Enumerable.Empty<T>().AsQueryable());
            }
            else
                throw new ArgumentException();
        }

        public Task<IQueryable<T>> GetTopRatedItems(int pageNumber, int pageSize, string orderKey, bool desOrder)
        {
            if (!string.IsNullOrEmpty(orderKey))
            {
                IQueryable<T> data;

                if (desOrder)
                {
                    data = _context.Set<T>().AsNoTracking().AsSplitQuery()
                        .Where(i => EF.Property<double>(i, "Rate") >= 3.5).Include("Category")
                        .OrderByDescending(i => EF.Property<object>(i, orderKey))
                        .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }
                else
                {
                    data = _context.Set<T>().AsNoTracking().Where(i => EF.Property<double>(i, "Rate") >= 3.5).Include("Category")
                        .OrderBy(i => EF.Property<object>(i, orderKey))
                        .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }
                return Task.FromResult(data.Any() ? data : Enumerable.Empty<T>().AsQueryable());
            }
            else
                throw new ArgumentException();
        }

        public Task<IQueryable<T>> GetItemsFilteredByPrice(int price1, int price2, int pageNumber, int pageSize, string orderKey, bool desOrder)
        {
            if (desOrder)
            {
                if (price2 == 0)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var items = _context.Set<T>().AsNoTracking().AsSplitQuery()
                            .Where(mp => EF.Property<double>(mp, "Price") <= price1)
                            .Include("Category")
                            .OrderByDescending(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return Task.FromResult(items.Any() ? items : Enumerable.Empty<T>().AsQueryable());
                    }
                    else
                        throw new ArgumentException();
                }
                else if (price1 >= 1000 & price2 >= 1000)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var items = _context.Set<T>().AsNoTracking().AsSplitQuery()
                            .Where(mp => EF.Property<double>(mp, "Price") >= price1 && EF.Property<double>(mp, "Price") <= price2)
                            .Include("Category")
                            .OrderByDescending(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return Task.FromResult(items.Any() ? items : Enumerable.Empty<T>().AsQueryable());
                    }
                    else
                        throw new ArgumentException();
                }
                else if (price2 == 1)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var items = _context.Set<T>().AsNoTracking().AsSplitQuery()
                            .Where(mp => EF.Property<double>(mp, "Price") >= price1)
                            .Include("Category")
                            .OrderByDescending(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return Task.FromResult(items.Any() ? items : Enumerable.Empty<T>().AsQueryable());
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
                        var items = _context.Set<T>().AsNoTracking().AsSplitQuery()
                            .Where(mp => EF.Property<double>(mp, "Price") <= price1)
                            .Include("Category")
                            .OrderBy(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return Task.FromResult(items.Any() ? items : Enumerable.Empty<T>().AsQueryable());
                    }
                    else
                        throw new ArgumentException();
                }
                else if (price1 >= 1000 & price2 >= 1000)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var items = _context.Set<T>().AsNoTracking().AsSplitQuery()
                            .Where(mp => EF.Property<double>(mp, "Price") >= price1 && EF.Property<double>(mp, "Price") <= price2)
                            .Include("Category")
                            .OrderBy(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return Task.FromResult(items.Any() ? items : Enumerable.Empty<T>().AsQueryable());
                    }
                    else
                        throw new ArgumentException();
                }
                else if (price2 == 1)
                {
                    if (!string.IsNullOrEmpty(orderKey))
                    {
                        var items = _context.Set<T>().AsNoTracking().AsSplitQuery()
                            .Where(mp => EF.Property<double>(mp, "Price") >= price1)
                            .Include("Category")
                            .OrderBy(m => EF.Property<object>(m, orderKey))
                            .Skip((pageNumber - 1) * pageSize).Take(pageSize);

                        return Task.FromResult(items.Any() ? items : Enumerable.Empty<T>().AsQueryable());
                    }
                    else
                        throw new ArgumentException();
                }
                else
                    throw new ArgumentException("An error occurred.");
            }
        }

        public Task<IQueryable<T>> GetCategoryItems(string name, int pageNumber, int pageSize, string orderKey, bool desOrder)
        {
            if (!string.IsNullOrEmpty(orderKey))
            {
                var query = _context.Set<T>().AsNoTracking().AsQueryable();
                IQueryable<T> data;

                var navCategory = _context.Model.FindEntityType(typeof(T))?.FindNavigation("Category");

                if (navCategory != null)
                {
                    query = query.AsNoTracking().AsSplitQuery().Include("Category");
                }

                if (typeof(T).GetProperty("Category") != null)
                {
                    query = query.AsNoTracking().Where(m => EF.Property<string>(EF.Property<object>(m, "Category"), "Name") == name);
                }

                if (desOrder)
                {
                    data = query.AsNoTracking().OrderByDescending(m => EF.Property<object>(m, orderKey))
                       .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }
                else
                {
                    data = query.AsNoTracking().OrderBy(m => EF.Property<object>(m, orderKey))
                       .Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }
                return Task.FromResult(data.Any() ? data.AsQueryable() : Enumerable.Empty<T>().AsQueryable());
            }
            else
                throw new NotImplementedException();
        }

        public async Task<int> TotalItems(string Key, int? price1 = null, int? price2 = null, string? categoryName = null)
        {
            if (Key == "Latest")
            {
                var max = await _context.Set<T>().AsNoTracking().CountAsync();

                var itemsList = await GetLatestItems(1, max, "ID", false);
                var result = itemsList.AsNoTracking().Count();

                return result;
            }
            else if (Key == "Rated")
            {
                var max = await _context.Set<T>().AsNoTracking().CountAsync();

                var itemsList = await GetTopRatedItems(1, max, "ID", false);
                var result = itemsList.AsNoTracking().Count();

                return result;
            }
            else if (Key == "Discounted")
            {
                var max = await _context.Set<T>().AsNoTracking().CountAsync();

                var itemsList = await GetDiscountedItems(1, max, "ID", false);
                var result = itemsList.AsNoTracking().Count();

                return result;
            }
            else if (Key == "Price")
            {
                var max = await _context.Set<T>().AsNoTracking().CountAsync();

                var itemsList = await GetItemsFilteredByPrice(price1 ?? 0, price2 ?? 0, 1, max, "ID", false);
                var result = itemsList.AsNoTracking().Count();

                return result;
            }
            else if (Key == "Brands")
            {
                var max = await _context.Set<T>().AsNoTracking().CountAsync();

                var itemsList = await GetCategoryItems(categoryName ?? "", 1, max, "ID", false);
                var result = itemsList.AsNoTracking().Count();

                return result;
            }
            else
                throw new ArgumentException();
        }

        public async Task<IQueryable<Comment>> GetItemComments(int id, string type, string key)
        {
            var item = await _context.Set<T>().FindAsync(id);

            if (item is not null)
            {
                if (key == "Default")
                {
                    var comments = _context.Comments.AsNoTracking().AsSplitQuery()
                        .Include(u => u.User).Where(c => c.ItemId == id && c.ItemType == type)
                        .OrderBy(c => c.CommentTime).Take(2);

                    return comments.Any() ? comments : Enumerable.Empty<Comment>().AsQueryable();
                }
                else if (key == "All")
                {
                    var comments = _context.Comments.AsNoTracking().AsSplitQuery()
                        .Include(u => u.User).Where(c => c.ItemId == id && c.ItemType == type)
                        .OrderBy(c => c.CommentTime);

                    return comments.Any() ? comments : Enumerable.Empty<Comment>().AsQueryable();
                }
            }
            return Enumerable.Empty<Comment>().AsQueryable();
        }

        public async Task<IQueryable<Rate>> GetItemRates(int id, string type)
        {
            var item = await _context.Set<T>().FindAsync(id);

            if (item is not null)
            {
                var rates = _context.Rates.AsNoTracking().Where(r => r.ItemId == id && r.ItemType == type);

                return rates.Any() ? rates : Enumerable.Empty<Rate>().AsQueryable();
            }
            return Enumerable.Empty<Rate>().AsQueryable();
        }

        public async Task<bool> SetRate(Rate data)
        {
            var item = _context.Set<T>().Find(data.ItemId);

            if (item is not null)
            {
                var entityType = typeof(T);
                var ratePropInfo = entityType.GetProperty("Rate", BindingFlags.Public | BindingFlags.Instance);

                if (ratePropInfo == null || ratePropInfo.PropertyType != typeof(double))
                    throw new InvalidOperationException(
                        $"{entityType.Name} does not have a public double Rate property.");

                var rates = this.GetItemRates(data.ItemId, data.ItemType).Result;

                if (rates.Any())
                {
                    var avgRate = await rates.AsNoTracking().AverageAsync(r => r.Value);

                    ratePropInfo.SetValue(item, avgRate);

                    _context.Update(item);

                    await _context.SaveChangesAsync();

                    return true;
                }
            }
            return false;
        }

        public async Task<int[]> GetItemRateDetails(int id, string type)
        {
            var item = await _context.Set<T>().FindAsync(id);

            if (item is not null)
            {
                int[] valuesCount = new int[5];

                var firstValue = await _context.Rates.AsNoTracking().Where(r => r.ItemId == id && r.ItemType == type && r.Value <= 1).CountAsync();
                var secondValue = await _context.Rates.AsNoTracking().Where(r => r.ItemId == id && r.ItemType == type && r.Value > 1 && r.Value <= 2).CountAsync();
                var thirdValue = await _context.Rates.AsNoTracking().Where(r => r.ItemId == id && r.ItemType == type && r.Value > 2 && r.Value <= 3).CountAsync();
                var fourthValue = await _context.Rates.AsNoTracking().Where(r => r.ItemId == id && r.ItemType == type && r.Value > 3 && r.Value <= 4).CountAsync();
                var fifthValue = await _context.Rates.AsNoTracking().Where(r => r.ItemId == id && r.ItemType == type && r.Value > 4 && r.Value <= 5).CountAsync();

                valuesCount[0] = firstValue;
                valuesCount[1] = secondValue;
                valuesCount[2] = thirdValue;
                valuesCount[3] = fourthValue;
                valuesCount[4] = fifthValue;

                return valuesCount;
            }
            return Array.Empty<int>();
        }
    }
}