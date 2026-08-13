using ApplicationLayer.DTOs;
using Souq.Models.Cart_Orders;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IPaymentsService
    {
        Task<Order> GetUserCurrentOrderOrCreateIt();
        Task<Order> CODCheckout();
        Task<bool> SucceedOrder(string method, string? sessionID);
        Task FaildOrder(string method);
    }
}