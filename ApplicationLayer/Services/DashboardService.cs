using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;

namespace ApplicationLayer.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> userManager;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public DashboardService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            this.userManager = userManager;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<DashboardDTO> GetDashboardRelatedData()
        {
            double totalRevenue = 0;

            var usersCount = userManager.Users.Count();

            var orders = await _servicesInstanceProvider.GetOrdersServiceInstance().AllOrders();

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

            var productsCount = (await _unitOfWork.Items.GetAll()).Count();

            return new DashboardDTO()
            {
                ProductsCount = productsCount,
                UsersCount = usersCount,
                TotalRevenue = totalRevenue,
                OrdersCount = ordersCount,
            };
        }
    }
}
