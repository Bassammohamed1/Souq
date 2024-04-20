using Microsoft.AspNetCore.Identity;

namespace InfrastructureLayer.Data.Seeds
{
    public static class Users
    {
        public static async Task CreateUser(UserManager<IdentityUser> userManager)
        {
            var user = new IdentityUser() { UserName = "User@gmail.com", Email = "User@gmail.com", PhoneNumber = "123456" };
            await userManager.CreateAsync(user, "Ba$$am3324");
            await userManager.AddToRoleAsync(user, "User");
        }
        public static async Task CreateAdmin(UserManager<IdentityUser> userManager)
        {
            var user = new IdentityUser() { UserName = "Admin@gmail.com", Email = "Admin@gmail.com", PhoneNumber = "123456" };
            await userManager.CreateAsync(user, "Ba$$am3324");
            await userManager.AddToRolesAsync(user, new List<string>() { "Admin", "User" });
        }
    }
}