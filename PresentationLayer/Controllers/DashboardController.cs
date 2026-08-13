using ApplicationLayer.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboard;

        public DashboardController(IDashboardService dashboard)
        {
            _dashboard = dashboard;
        }

        public async Task<IActionResult> Dashboard()
        {
            var result = await _dashboard.GetDashboardRelatedData();

            var dashboardVM = new DashboardViewModel()
            {
                ProductsCount = result.ProductsCount,
                UsersCount = result.UsersCount,
                TotalRevenue = result.TotalRevenue,
                OrdersCount = result.OrdersCount,
            };

            return View(dashboardVM);
        }

        public IActionResult Analytics()
        {
            return View();
        }
    }
}
