using ApplicationLayer.DTOs;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IElectronicsService
    {
        Task<IndexDTO> GetAllElectronics();
        Task<ItemsDTO> GetBrandsElectronics(string? orderIndex, int? page, string name, bool? Des);
        Task<ItemsDTO> GetElectronicsWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? Des);
    }
}
