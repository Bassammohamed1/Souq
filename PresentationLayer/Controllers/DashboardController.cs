using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> userManager;

        public DashboardController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            double totalRevenue = 0;

            var usersCount = await userManager.Users.CountAsync();

            var orders = await _unitOfWork.Orders.AllOrders(1, int.MaxValue);

            foreach (var order in orders)
            {
                var totalPrice = order.OrderDetails.Sum(od => od.Price * od.Quantity);

                if (order.PromoCodeDiscountType == "fixed")
                {
                    order.TotalPrice = totalPrice - order.PromoCodeDiscountValue ?? 0;
                }
                else
                {
                    order.TotalPrice = totalPrice * (1 - (order.PromoCodeDiscountValue ?? 0) / 100.0);
                }

                totalRevenue += order.TotalPrice;
            }

            var ordersCount = orders.Count();

            var productsCount = _unitOfWork.Items.GetAll(1, int.MaxValue).Result.Count();

            var dashboardVM = new DashboardViewModel()
            {
                ProductsCount = productsCount,
                UsersCount = usersCount,
                TotalRevenue = totalRevenue,
                OrdersCount = ordersCount,
            };

            return View(dashboardVM);
        }

        public IActionResult Analytics()
        {
            return View();
        }
    }
}
