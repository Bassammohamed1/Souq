using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IAirConditionersService
    {
        Task<AirConditioner> GetAirConditioner(int id);
        IEnumerable<AirConditioner> GetAirConditioners(int pageNumber, int pageSize);
        Task<Result> Add(AirConditioner airConditioner);
        Task<Result> Update(AirConditioner airConditioner);
        Task<Result> Delete(AirConditioner airConditioner);
        ItemDTO<AirConditionerDTO> GetAirConditionersWithRelatedOnes();
        Task<ItemsDTO> GetBrandsAirConditioners(string? orderIndex, int? page, string name, bool? des);
        Task<ItemsDTO> GetDiscountedAirConditioners(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetTopRatedAirConditioners(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetLatestAirConditioners(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetAirConditionersWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des);
        Task<AirConditionerDTO> GetAirConditionerDetails(int id);
        Task<AirConditionerDTO> GetAirConditionerAllComments(int id);
        Task<IEnumerable<Category>> GetSpecificCategoriesForSelectList();
    }
}