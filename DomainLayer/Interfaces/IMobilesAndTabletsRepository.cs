using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface IMobilesAndTabletsRepository : IRepository<MobileAndTablet>
    {
        IEnumerable<MobileAndTablet> GetByPrice();
        IEnumerable<MobileAndTablet> GetByDate();
    }
}
