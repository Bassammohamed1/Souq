using DomainLayer.Models;
using DomainLayer.Models.Chat;
using DomainLayer.Models.Wishing_List;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Souq.Models.Cart_Orders;


namespace InfrastructureLayer.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryDepartments> CategoryDepartments { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<MobilePhone> MobilePhones { get; set; }
        public DbSet<TV> TVs { get; set; }
        public DbSet<HeadPhone> HeadPhones { get; set; }
        public DbSet<Laptop> Laptops { get; set; }
        public DbSet<WashingMachine> WashingMachines { get; set; }
        public DbSet<Cooker> Cookers { get; set; }
        public DbSet<AirConditioner> AirConditioners { get; set; }
        public DbSet<Fridge> Fridges { get; set; }
        public DbSet<VideoGame> VideoGames { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<WishingList> WishingLists { get; set; }
        public DbSet<WishingListDetails> WishingListsDetails { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().UseTpcMappingStrategy();

            modelBuilder.Entity<CategoryDepartments>().HasKey(k => new { k.DepartmentId, k.CategoryId });
            modelBuilder.Entity<CategoryDepartments>().HasOne(d => d.Department).WithMany(x => x.CategoryDepartments).HasForeignKey(x => x.DepartmentId);
            modelBuilder.Entity<CategoryDepartments>().HasOne(d => d.Category).WithMany(x => x.CategoryDepartments).HasForeignKey(x => x.CategoryId);

            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(k => new { k.LoginProvider, k.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(p => new { p.UserId, p.Name, p.LoginProvider });


        }
    }
}