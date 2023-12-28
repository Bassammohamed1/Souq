using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Souq.Data.IdentitySeeds;
using Souq.Data.ViewModels;
using System.Reflection.Emit;

namespace Souq.Models
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<MobileAndTablet> MobilesAndTablets { get; set; }
        public DbSet<ElectricalDevice> ElectricalDevices { get; set; }
        public DbSet<Laptop> Laptops { get; set; }
        public DbSet<ComputerAccessory> ComputerAccessories { get; set; }
        public DbSet<BaseModel> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}