using Microsoft.AspNetCore.Identity;

namespace InfrastructureLayer.Data.IdentitySeeds
{
    public static class Roles
    {
        public static async Task CreateAdmin(RoleManager<IdentityRole> roleManager)
        {
            if (await roleManager.FindByNameAsync("Admin") is null)
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            else
                throw new Exception("Role is exists!!");
        }
        public static async Task CreateUser(RoleManager<IdentityRole> roleManager)
        {
            if (await roleManager.FindByNameAsync("User") is null)
                await roleManager.CreateAsync(new IdentityRole("User"));
            else
                throw new Exception("Role is exists!!");
        }
    }
}
