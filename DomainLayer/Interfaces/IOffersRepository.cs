using DomainLayer.DTOs;
using DomainLayer.Models;

namespace DomainLayer.Interfaces
{
    public interface IOffersRepository
    {
        Task<IEnumerable<OfferDTO>> GetAllOffers();
        Task<Offer> FindOfferByID(int id);
        Task CreateOffer(OfferDTO offer);
        Task UpdateOffer(OfferDTO offer);
        Task DeleteOffer(int ID);
        Task<Offer> IsPromoCodeExist(string promoCode);
        Task<IQueryable<Offer>> GetOffers(string? department, string? category, int? itemID);
        Task<Item> GetBOGOGetItem(Item item);
    }
}
