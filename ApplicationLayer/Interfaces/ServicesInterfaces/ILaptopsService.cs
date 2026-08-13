using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface ILaptopsService
    {
        Task<Laptop> GetLaptop(int id);
        IEnumerable<Laptop> GetLaptops(int pageNumber, int pageSize);
        Task<Result> Add(Laptop laptop);
        Task<Result> Update(Laptop laptop);
        Task<Result> Delete(Laptop laptop);
        ItemDTO<LaptopDTO> GetLaptopsWithRelatedOnes();
        Task<ItemsDTO> GetBrandsLaptops(string? orderIndex, int? page, string name, bool? des);
        Task<ItemsDTO> GetDiscountedLaptops(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetTopRatedLaptops(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetLatestLaptops(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetLaptopsWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des);
        Task<LaptopDTO> GetLaptopDetails(int id);
        Task<LaptopDTO> GetLaptopAllComments(int id);
        Task<IEnumerable<Category>> GetSpecificCategoriesForSelectList();
    }
}
