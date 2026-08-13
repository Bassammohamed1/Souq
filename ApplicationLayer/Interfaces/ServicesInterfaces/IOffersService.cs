using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IOffersService
    {
        Task<Offer> GetOffer(int id);
        Task<IEnumerable<OfferDTO>> GetAllOffers();
        Task<OfferDTO> GetOfferWithRelatedData();
        Task<OfferDTO> GetOfferWithRelatedData(int id);
        Task<Result> CreateOffer(OfferDTO offer);
        Task<Result> UpdateOffer(OfferDTO offer);
        Task<Result> DeleteOffer(int ID);
        Task<Offer> IsPromoCodeExist(string promoCode);
        IQueryable<Offer> GetOffers(string? department, string? category, int? itemID);
        Task<Item> GetBOGOGetItem(Item item);
    }
}
