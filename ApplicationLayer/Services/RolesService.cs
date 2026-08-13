using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Identity;

namespace ApplicationLayer.Services
{
    public class RolesService : IRolesService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IEnumerable<IdentityRole> AllRoles()
        {
            var roles = _roleManager.Roles.ToList();

            return roles.Any() ? roles : Enumerable.Empty<IdentityRole>();
        }

        public async Task<Result> CreateRole(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return new Result()
                {
                    Success = false,
                    Error = $"Role {roleName} is already exist."
                };
            }
            else
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));

                return result.Succeeded ? new Result() { Success = true } : new Result()
                {
                    Success = false,
                    Error = string.Join('-', result.Errors)
                };
            }
        }
    }
}
