using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IVideoGamesService
    {
        Task<VideoGame> GetVideoGame(int id);
        IEnumerable<VideoGame> GetVideoGames(int pageNumber, int pageSize);
        Task<Result> Add(VideoGame videoGame);
        Task<Result> Update(VideoGame videoGame);
        Task<Result> Delete(VideoGame videoGame);
        ItemDTO<VideoGameDTO> GetVideoGamesWithRelatedOnes();
        Task<ItemsDTO> GetBrandsVideoGames(string? orderIndex, int? page, string name, bool? des);
        Task<ItemsDTO> GetDiscountedVideoGames(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetTopRatedVideoGames(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetLatestVideoGames(string? orderIndex, int? page, bool? des);
        Task<ItemsDTO> GetVideoGamesWithPriceFilter(string? orderIndex, int? page, int price1, int price2, bool? des);
        Task<VideoGameDTO> GetVideoGameDetails(int id);
        Task<VideoGameDTO> GetVideoGameAllComments(int id);
        Task<IEnumerable<Category>> GetSpecificCategoriesForSelectList();
    }
}
