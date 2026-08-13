using ApplicationLayer.DTOs;
using Souq.Models.Cart_Orders;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IOrdersService
    {
        Task<IEnumerable<Order>> AllOrders();
        Task<OrdersDTO> AllOrders(int? page);
        OrdersDTO UserOrders(int? page, string userID);
        Task<Order> GetUserCurrentOrder(string userID);
        Task<Order> GetUserCurrentOrderOrCreateIt(string userID);
        Task<Order> CreateOrder(string userID, IEnumerable<OrderDetails> data);
        Task SetOrderPaymentMethodAndStatus(int orderID, string paymentMethod, int status);
    }
}
