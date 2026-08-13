using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using DomainLayer.Models;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IUsersService
    {
        string GetUserId();
        Task<AppUser> GetUser(string ID);
        IEnumerable<UserDTO> GetUsers();
        UsersDTO AllUsers(int? page);
        Task<UserRolesDTO> GetAllRolesWithUserSelectedRoles(string userID);
        Task<Result> ManageRoles(UserRolesDTO data);
        Task<Result> Update(SettingsDTO data);
        Task<Result> Delete(AppUser data);
    }
}
