using Souq.Models.Cart_Orders;

namespace DomainLayer.Interfaces
{
    public interface IOrdersRepository
    {
        Task<IEnumerable<Order>> AllOrders(int pageNumber, int pageSize);
        Task<IQueryable<Order>> UserOrders(int pageNumber, int pageSize, string userID);
        Task<Order> GetUserCurrentOrder();
        Task<Order> CreateOrder(List<OrderDetails> data);
        Task SetOrderPaymentMethodAndStatus(int orderID, string paymentMethod, int status);
    }
}
