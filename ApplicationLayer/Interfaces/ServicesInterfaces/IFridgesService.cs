using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IFridgesService
    {
        Task<Fridge> GetFridge(int id);
        IEnumerable<Fridge> GetFridges(int pageNumber, int pageSize);
        Task<Result> Add(Fridge fridge);
        Task<Result> Update(Fridge fridge);
        Task<Result> Delete(Fridge fridge);
        ItemDTO<FridgeDTO> GetFridgesWithRelatedOnes();
        Task<ItemsDTO> GetBrandsFridges(string? orderIndex, int? page, string name, bool? des);
        Task<ItemsDTO> GetDiscountedFridges(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetTopRatedFridges(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetLatestFridges(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetFridgesWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des);
        Task<FridgeDTO> GetFridgeDetails(int id);
        Task<FridgeDTO> GetFridgeAllComments(int id);
        Task<IEnumerable<Category>> GetSpecificCategoriesForSelectList();
    }
}
