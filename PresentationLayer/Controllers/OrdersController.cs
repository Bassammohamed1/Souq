using ApplicationLayer.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrdersService _orders;

        public OrdersController(IOrdersService orders)
        {
            _orders = orders;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var result = await _orders.AllOrders(page);

            var ordersVM = new OrdersViewModel()
            {
                Orders = result.Orders
                .Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    UserName = o.UserName,
                    TotalPrice = o.TotalPrice,
                    CreatedAt = o.CreatedAt,
                    PaymentMethod = o.PaymentMethod,
                    Status = o.Status
                }),
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages
            };

            return View(ordersVM);
        }

        public IActionResult UserOrders(int? page, string userID)
        {
            var result = _orders.UserOrders(page, userID);

            var userOrdersVM = new OrdersViewModel()
            {
                Orders = result.Orders
                 .Select(o => new OrderViewModel
                 {
                     Id = o.Id,
                     UserName = o.UserName,
                     TotalPrice = o.TotalPrice,
                     CreatedAt = o.CreatedAt,
                     PaymentMethod = o.PaymentMethod,
                     Status = o.Status
                 }),
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages
            };

            return View("Index", userOrdersVM);
        }
    }
}

