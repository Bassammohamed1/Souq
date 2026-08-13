using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IHeadPhonesService
    {
        Task<HeadPhone> GetHeadPhone(int id);
        IEnumerable<HeadPhone> GetHeadPhones(int pageNumber, int pageSize);
        Task<Result> Add(HeadPhone headPhone);
        Task<Result> Update(HeadPhone headPhone);
        Task<Result> Delete(HeadPhone headPhone);
        ItemDTO<HeadPhoneDTO> GetHeadPhonesWithRelatedOnes();
        Task<ItemsDTO> GetBrandsHeadPhones(string? orderIndex, int? page, string name, bool? des);
        Task<ItemsDTO> GetDiscountedHeadPhones(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetTopRatedHeadPhones(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetLatestHeadPhones(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetHeadPhonesWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des);
        Task<HeadPhoneDTO> GetHeadPhoneDetails(int id);
        Task<HeadPhoneDTO> GetHeadPhoneAllComments(int id);
        Task<IEnumerable<Category>> GetSpecificCategoriesForSelectList();
    }
}
