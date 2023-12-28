using Souq.Models;

namespace Souq.Repository.Interfaces
{
    public interface IComputerAccessoriesRepository : IRepository<ComputerAccessory>
    {
        IEnumerable<ComputerAccessory> GetByPrice();
        IEnumerable<ComputerAccessory> GetByDate();
    }
}