using Microsoft.EntityFrameworkCore;
using Souq.Data.ViewModels;
using Souq.Models;
using Souq.Repository.Interfaces;

namespace Souq.Repository
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly AppDbContext _context;
        public OrdersRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersByUserIdAndRoleAsync(string userId, string userRole)
        {
            var orders = await _context.Orders.Include(n => n.OrderItems).ThenInclude(n => n.Item).Include(n => n.User).ToListAsync();

            if (userRole != "Admin")
            {
                orders = orders.Where(n => n.UserId == userId).ToList();
            }

            return orders;
        }

        public async Task StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress)
        {
            var order = new Order()
            {
                UserId = userId,
                Email = userEmailAddress
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                var orderItem = new OrderItem()
                {
                    Amount = item.Amount,
                    itemId = item.Item.Id,
                    OrderId = order.Id,
                    Price = item.Item.Price
                };
                await _context.OrderItems.AddAsync(orderItem);
            }
            await _context.SaveChangesAsync();
        }
    }
}
