using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface IOffersRepository : IRepository<Offer>
    {
        IQueryable<Offer> GetOffersWithDepartmentName(string departmentName);
        IQueryable<Offer> GetOffersWithCategoryName(string categoryName);
        IQueryable<Offer> GetOffersWithItemID(int itemID);
    }
}
