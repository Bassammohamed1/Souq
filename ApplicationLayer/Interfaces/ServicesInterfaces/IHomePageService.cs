using ApplicationLayer.DTOs;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IHomePageService
    {
        Task<HomePageDTO> GetHomePageRelatedData();
        Task<string> GetItemType(int ID);
        Task<ItemsDTO> GetAllItems(string categoryName, string? orderIndex, int? page);
        Task<FilterDTO> GetFilteredItems(string key, int? page, string? orderIndex);
        Task<HomePageOfferDetailsDTO> GetHomePageOfferDetails(int id);
    }
}
