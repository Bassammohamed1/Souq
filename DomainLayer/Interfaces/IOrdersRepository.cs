using DomainLayer.Models.Cart;

namespace DomainLayer.Interfaces
{
    public interface IOrdersRepository
    {
        Task<IEnumerable<Order>> UserOrders();
    }
}
