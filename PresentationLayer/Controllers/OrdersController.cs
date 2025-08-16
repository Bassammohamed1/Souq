using DomainLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var allOrders = await _unitOfWork.Orders.AllOrders(1, int.MaxValue);

            var totalPages = (int)Math.Ceiling(allOrders.Count() / (double)pageSize);

            var orders = _unitOfWork.Orders.AllOrders(pageNumber, pageSize).Result.ToList()
                   .Select(o =>
                   {
                       var totalPrice = o.OrderDetails.Sum(od => od.Price * od.Quantity);

                       if (o.PromoCodeDiscountType == "fixed")
                       {
                           o.TotalPrice = totalPrice - o.PromoCodeDiscountValue ?? 0;
                       }
                       else
                       {
                           o.TotalPrice = totalPrice * (1 - (o.PromoCodeDiscountValue ?? 0) / 100.0);
                       }

                       return new OrderViewModel()
                       {
                           Id = o.ID,
                           UserName = o.User.UserName,
                           TotalPrice = o.TotalPrice,
                           CreatedAt = o.CreatedAt.ToString("g"),
                           PaymentMethod = o.PaymentMethod,
                           Status = o.Status
                       };
                   }).ToList();

            var ordersVM = new OrdersViewModel()
            {
                Orders = orders,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            };

            return View(ordersVM);
        }

        public async Task<IActionResult> UserOrders(int? page, string userID)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var allOrders = await _unitOfWork.Orders.UserOrders(1, int.MaxValue, userID);

            var totalPages = (int)Math.Ceiling(allOrders.Count() / (double)pageSize);

            var userOrders = _unitOfWork.Orders.UserOrders(pageNumber, pageSize, userID).Result.ToList()
             .Select(o =>
             {
                 var totalPrice = o.OrderDetails.Sum(od => od.Price * od.Quantity);

                 if (o.PromoCodeDiscountType == "fixed")
                 {
                     o.TotalPrice = totalPrice - o.PromoCodeDiscountValue ?? 0;
                 }
                 else
                 {
                     o.TotalPrice = totalPrice * (1 - (o.PromoCodeDiscountValue ?? 0) / 100.0);
                 }

                 return new OrderViewModel()
                 {
                     Id = o.ID,
                     UserName = o.User.UserName,
                     TotalPrice = o.TotalPrice,
                     CreatedAt = o.CreatedAt.ToString("g"),
                     PaymentMethod = o.PaymentMethod,
                     Status = o.Status
                 };
             }).ToList();

            var userOrdersVM = new OrdersViewModel()
            {
                Orders = userOrders,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            };

            return View("Index", userOrdersVM);
        }
    }
}

