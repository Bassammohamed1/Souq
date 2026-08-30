using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface ITVsService
    {
        Task<TV> GetTV(int id);
        IEnumerable<TV> GetTVs(int pageNumber, int pageSize);
        Task<Result> Add(TV tv);
        Task<Result> Update(TV tv);
        Task<Result> Delete(TV tv);
        ItemDTO<TVDTO> GetTVsWithRelatedOnes();
        Task<ItemsDTO> GetBrandsTVs(string? orderIndex, int? page, string name, bool? des);
        Task<ItemsDTO> GetDiscountedTVs(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetTopRatedTVs(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetLatestTVs(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetTVsWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des);
        Task<TVDTO> GetTVDetails(int id);
        Task<TVDTO> GetTVAllComments(int id);
        Task<IEnumerable<Category>> GetSpecificCategoriesForSelectList();
    }
}
