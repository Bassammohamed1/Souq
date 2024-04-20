using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface IComputerAccessoriesRepository : IRepository<ComputerAccessory>
    {
        IEnumerable<ComputerAccessory> GetByPrice();
        IEnumerable<ComputerAccessory> GetByDate();
    }
}