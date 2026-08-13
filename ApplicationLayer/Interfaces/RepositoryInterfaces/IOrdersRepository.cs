using Souq.Models.Cart_Orders;

namespace DomainLayer.Interfaces
{
    public interface IOrdersRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetAllOrders();
        Task<IEnumerable<Order>> GetAllOrders(int pageNumber, int pageSize);
        IQueryable<Order> GetUserOrders(int pageNumber, int pageSize, string userID);
        Task<Order> GetUserPendingOrder(string userID);
        Task<Order> GetUserPendingOrderWithDetails(string userID);
        Task<OrderDetails> AddOrderDetails(OrderDetails orderDetails);
    }
}