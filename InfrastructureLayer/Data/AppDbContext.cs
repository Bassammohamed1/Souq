using DomainLayer.Models;
using DomainLayer.Models.CartModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace InfrastructureLayer.Data
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
                entity.Property(e => e.ProviderDisplayName).HasMaxLength(256);
            });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(x => x.UserId);
        }
    }
}