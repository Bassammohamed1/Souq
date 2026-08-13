using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IMobilePhonesService
    {
        Task<MobilePhone> GetMobilePhone(int id);
        IEnumerable<MobilePhone> GetMobilePhones(int pageNumber, int pageSize);
        Task<Result> Add(MobilePhone mobilePhone);
        Task<Result> Update(MobilePhone mobilePhone);
        Task<Result> Delete(MobilePhone mobilePhone);
        ItemDTO<MobilePhoneDTO> GetMobilePhonesWithRelatedOnes();
        Task<ItemsDTO> GetBrandsMobilePhones(string? orderIndex, int? page, string name, bool? des);
        Task<ItemsDTO> GetDiscountedMobilePhones(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetTopRatedMobilePhones(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetLatestMobilePhones(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetMobilePhonesWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des);
        Task<ItemsDTO> GetMobilePhonesWithStorageFilter(string? orderIndex, int? page, int storage, bool? des);
        Task<MobilePhoneDTO> GetMobilePhoneDetails(int id);
        Task<MobilePhoneDTO> GetMobilePhoneAllComments(int id);
        Task<IEnumerable<Category>> GetSpecificCategoriesForSelectList();
    }
}