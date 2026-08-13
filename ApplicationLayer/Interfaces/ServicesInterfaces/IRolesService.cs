using ApplicationLayer.Helpers;
using Microsoft.AspNetCore.Identity;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IRolesService
    {
        IEnumerable<IdentityRole> AllRoles();
        Task<Result> CreateRole(string roleName);
    }
}
