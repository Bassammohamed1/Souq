using DomainLayer.Enums;
using DomainLayer.Interfaces;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using Souq.Models.Cart_Orders;

namespace InfrastructureLayer.Repository
{
    public class OrdersRepository : Repository<Order>, IOrdersRepository
    {
        private readonly AppDbContext _context;

        public OrdersRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _context.Orders.AsNoTracking()
                .Include(o => o.User).Include(o => o.OrderDetails)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrders(int pageNumber, int pageSize)
        {
            return await _context.Orders.AsNoTracking()
                .Include(o => o.User).Include(o => o.OrderDetails)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .ToListAsync();
        }

        public IQueryable<Order> GetUserOrders(int pageNumber, int pageSize, string userID)
        {
            return _context.Orders.AsNoTracking()
               .Include(o => o.User).Include(o => o.OrderDetails)
               .Where(o => o.UserID == userID)
               .Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public async Task<Order> GetUserPendingOrderWithDetails(string userID)
        {
            return await _context.Orders
              .Include(o => o.OrderDetails)
              .Where(o => o.UserID == userID && o.Status == OrderStatus.Pending)
              .FirstOrDefaultAsync();
        }

        public async Task<Order> GetUserPendingOrder(string userID)
        {
            return await _context.Orders
                .Where(o => o.UserID == userID && o.Status == OrderStatus.Pending)
                .FirstOrDefaultAsync();
        }

        public async Task<OrderDetails> AddOrderDetails(OrderDetails orderDetails)
        {
            await _context.OrderDetails.AddAsync(orderDetails);

            return orderDetails;
        }
    }
}