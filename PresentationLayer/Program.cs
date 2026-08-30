using ApplicationLayer.Interfaces.ServicesInterfaces;
using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using InfrastructureLayer.Helpers;
using InfrastructureLayer.Mailing;
using InfrastructureLayer.Payments;
using InfrastructureLayer.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();

//Configure database Connection
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection")));

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
     .AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAirConditionersService, AirConditionersService>();
builder.Services.AddScoped<IAppliancesService, AppliancesService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IChatsService, ChatsService>();
builder.Services.AddScoped<ICookersService, CookersService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IDepartmentsService, DepartmentsService>();
builder.Services.AddScoped<IElectronicsService, ElectronicsService>();
builder.Services.AddScoped<IFridgesService, FridgesService>();
builder.Services.AddScoped<IHeadPhonesService, HeadPhonesService>();
builder.Services.AddScoped<IHomePageService, HomePageService>();
builder.Services.AddScoped<IItemsService, ItemsService>();
builder.Services.AddScoped<ILaptopsService, LaptopsService>();
builder.Services.AddScoped<IMobilePhonesService, MobilePhonesService>();
builder.Services.AddScoped<IOffersService, OffersService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IPaymentsService, PaymentsService>();
builder.Services.AddScoped<IPaymentMethodsImplementations, PaymetMethodsImplementations>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<ITVsService, TVsService>();
builder.Services.AddScoped<IUserInteractionsService, UserInteractionsService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IVideoGamesService, VideoGamesService>();
builder.Services.AddScoped<IWashingMachinesService, WashingMachinesService>();
builder.Services.AddScoped<IWishingListService, WishingListService>();
builder.Services.AddScoped<IServicesInstanceProvider, ServicesInstanceProvider>();


builder.Services.AddSingleton<PaypalClient>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IEmailSender, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseRouting();

app.UseAuthorization();

app.MapHub<ChatHub>("/chat");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints => endpoints.MapRazorPages());

app.Run();
