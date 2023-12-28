using Microsoft.AspNetCore.Identity;

namespace Souq.Data.IdentitySeeds
{
    public static class Roles
    {
        public static async Task AddRoles(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
    }
}
