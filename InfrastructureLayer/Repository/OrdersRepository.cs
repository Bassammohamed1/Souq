using DomainLayer.Enums;
using DomainLayer.Interfaces;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using Souq.Models.Cart_Orders;

namespace InfrastructureLayer.Repository
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;

        public OrdersRepository(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<IEnumerable<Order>> AllOrders(int pageNumber, int pageSize)
        {
            var orders = await _context.Orders.AsNoTracking()
                .Include(o => o.User).Include(o => o.OrderDetails)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .ToListAsync();

            return orders.Any() ? orders : Enumerable.Empty<Order>();
        }

        public async Task<IQueryable<Order>> UserOrders(int pageNumber, int pageSize, string userID)
        {
            var userOrders = _context.Orders.AsNoTracking()
                .Include(o => o.User).Include(o => o.OrderDetails).Where(o => o.UserID == userID)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return userOrders.Any() ? userOrders : Enumerable.Empty<Order>().AsQueryable();
        }

        public async Task<Order> CreateOrder(List<OrderDetails> data)
        {
            var userID = await _userService.GetUserId();

            if (!string.IsNullOrEmpty(userID))
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    var previousUserOrder = await _context.Orders.AsNoTracking()
                        .Where(o => o.UserID == userID && o.Status == OrderStatus.Pending)
                        .FirstOrDefaultAsync();

                    if (previousUserOrder is not null)
                    {
                        _context.Orders.Remove(previousUserOrder);
                        await _context.SaveChangesAsync();
                    }

                    var userOrder = new Order()
                    {
                        UserID = userID,
                        CreatedAt = DateTime.UtcNow,
                        Status = OrderStatus.Pending
                    };
                    await _context.Orders.AddAsync(userOrder);
                    await _context.SaveChangesAsync();

                    if (data is not null)
                    {
                        foreach (var order in data)
                        {
                            order.OrderID = userOrder.ID;
                            await _context.OrderDetails.AddRangeAsync(order);
                            await _context.SaveChangesAsync();
                        }
                    }

                    await transaction.CommitAsync();
                    return userOrder;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                }
            }
            throw new InvalidOperationException();
        }

        public async Task SetOrderPaymentMethodAndStatus(int orderID, string paymentMethod, int status)
        {
            var userID = await _userService.GetUserId();

            var userOrder = await _context.Orders
                   .Where(o => o.UserID == userID && o.Status == OrderStatus.Pending)
                   .FirstOrDefaultAsync();

            if (userOrder is not null)
            {
                userOrder.PaymentMethod = paymentMethod;
                userOrder.Status = (OrderStatus)status;
            }
            else
                throw new InvalidOperationException();
        }

        public async Task<Order> GetUserCurrentOrder()
        {
            var userID = await _userService.GetUserId();

            var userOrder = await _context.Orders.AsSplitQuery()
                .Include(o => o.OrderDetails)
                .Where(o => o.UserID == userID && o.Status == OrderStatus.Pending)
                .FirstOrDefaultAsync();

            return userOrder is not null ? userOrder : null;
        }
    }
}
