using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface ITvsService
    {
        Task<TV> GetTv(int id);
        IEnumerable<TV> GetTvs(int pageNumber, int pageSize);
        Task<Result> Add(TV tv);
        Task<Result> Update(TV tv);
        Task<Result> Delete(TV tv);
        ItemDTO<TvDTO> GetTvsWithRelatedOnes();
        Task<ItemsDTO> GetBrandsTvs(string? orderIndex, int? page, string name, bool? des);
        Task<ItemsDTO> GetDiscountedTvs(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetTopRatedTvs(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetLatestTvs(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetTvsWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des);
        Task<TvDTO> GetTvDetails(int id);
        Task<TvDTO> GetTvAllComments(int id);
        Task<IEnumerable<Category>> GetSpecificCategoriesForSelectList();
    }
}
