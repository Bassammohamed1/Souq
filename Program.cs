using Microsoft.EntityFrameworkCore;
using Souq.Models;
using Souq.Repository;
using Souq.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Souq.Data.IdentitySeeds;
using Souq.Data.Cart;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped(c => ShoppingCart.GetShoppingCart(c));
builder.Services.AddSession();
builder.Services.AddMemoryCache();

var app = builder.Build();

var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var usermanager = services.GetRequiredService<UserManager<IdentityUser>>();
var rolemanager = services.GetRequiredService<RoleManager<IdentityRole>>();

await Roles.AddRoles(rolemanager);

await Users.CreateAdmin(usermanager);
await Users.CreateUser(usermanager);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints => endpoints.MapRazorPages());

app.Run();
