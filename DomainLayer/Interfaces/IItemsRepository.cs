using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface IItemsRepository : IRepository<Item>
    {
        List<Item> GetAllItems();
    }
}
