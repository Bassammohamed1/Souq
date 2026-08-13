using ApplicationLayer.DTOs;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IAppliancesService
    {
        Task<IndexDTO> GetAllAppliances();
        Task<ItemsDTO> GetBrandsAppliances(string? orderIndex, int? page, string name, bool? Des);
        Task<ItemsDTO> GetAppliancesWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? Des);
    }
}
