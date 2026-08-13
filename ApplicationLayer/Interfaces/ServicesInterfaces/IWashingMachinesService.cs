using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IWashingMachinesService
    {
        Task<WashingMachine> GetWashingMachine(int id);
        IEnumerable<WashingMachine> GetWashingMachines(int pageNumber, int pageSize);
        Task<Result> Add(WashingMachine washingMachine);
        Task<Result> Update(WashingMachine washingMachine);
        Task<Result> Delete(WashingMachine washingMachine);
        ItemDTO<WashingMachineDTO> GetWashingMachinesWithRelatedOnes();
        Task<ItemsDTO> GetBrandsWashingMachines(string? orderIndex, int? page, string name, bool? des);
        Task<ItemsDTO> GetDiscountedWashingMachines(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetTopRatedWashingMachines(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetLatestWashingMachines(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetWashingMachinesWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des);
        Task<WashingMachineDTO> GetWashingMachineDetails(int id);
        Task<WashingMachineDTO> GetWashingMachineAllComments(int id);
        Task<IEnumerable<Category>> GetSpecificCategoriesForSelectList();
    }
}
