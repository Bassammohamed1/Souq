using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface ICookersService
    {
        Task<Cooker> GetCooker(int id);
        IEnumerable<Cooker> GetCookers(int pageNumber, int pageSize);
        Task<Result> Add(Cooker cooker);
        Task<Result> Update(Cooker cooker);
        Task<Result> Delete(Cooker cooker);
        ItemDTO<CookerDTO> GetCookersWithRelatedOnes();
        Task<ItemsDTO> GetBrandsCookers(string? orderIndex, int? page, string name, bool? des);
        Task<ItemsDTO> GetDiscountedCookers(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetTopRatedCookers(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetLatestCookers(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetCookersWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des);
        Task<CookerDTO> GetCookerDetails(int id);
        Task<CookerDTO> GetCookerAllComments(int id);
        Task<IEnumerable<Category>> GetSpecificCategoriesForSelectList();
    }
}
