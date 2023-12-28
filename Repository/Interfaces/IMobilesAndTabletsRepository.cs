using Souq.Models;

namespace Souq.Repository.Interfaces
{
    public interface IMobilesAndTabletsRepository : IRepository<MobileAndTablet>
    {
        IEnumerable<MobileAndTablet> GetByPrice();
        IEnumerable<MobileAndTablet> GetByDate();
    }
}
