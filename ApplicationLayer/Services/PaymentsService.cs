using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Souq.Models.Cart_Orders;
using Stripe.Checkout;

namespace ApplicationLayer.Services
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _users;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public PaymentsService(IUnitOfWork unitOfWork, IUsersService users, IEmailSender emailSender, UserManager<AppUser> userManager, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _users = users;
            _emailSender = emailSender;
            _userManager = userManager;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<Order> GetUserCurrentOrderOrCreateIt()
        {
            var userID = _users.GetUserId();

            return await _servicesInstanceProvider.GetOrdersServiceInstance().GetUserCurrentOrderOrCreateIt(userID);
        }

        public async Task<Order> CODCheckout()
        {
            var userID = _users.GetUserId();
            var user = await _userManager.FindByIdAsync(userID);

            var order = await _servicesInstanceProvider.GetOrdersServiceInstance().GetUserCurrentOrder(userID);
            var orderID = order.ID;
            var totalPrice = order.OrderDetails.Sum(od => od.Price * od.Quantity);
            order.TotalPrice = totalPrice;

            if (order.PromoCodeDiscountType == "fixed")
            {
                order.TotalPrice = totalPrice - order.PromoCodeDiscountValue ?? 0;
            }
            else
            {
                order.TotalPrice = totalPrice * (1 - (order.PromoCodeDiscountValue ?? 0) / 100.0);
            }

            await _servicesInstanceProvider.GetOrdersServiceInstance().SetOrderPaymentMethodAndStatus(orderID, "COD", 3);
            await _servicesInstanceProvider.GetCartServiceInstance().EmptyCart();
            await _unitOfWork.Commit();

            await _emailSender.SendEmailAsync(user.Email, "Order Confirmation",
                   $"<div style=\"font-family: Amiri, serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;\">\r\n  " +
                   $"<h2 style=\"color: #27ae60;\">✅ Thank you for your order!</h2>\r\n " +
                   $" <p style=\"font-size: 16px; color: #555;\">\r\n    Hi {user.UserName}, we’ve received your order <strong>#{orderID}</strong> and it's now being processed.\r\n  </p>\r\n\r\n " +
                   $" <p style=\"font-size: 15px; color: #666;\">\r\n    Here’s a quick summary of your purchase:\r\n  </p>\r\n\r\n  <ul style=\"font-size: 14px; color: #444;\">\r\n  " +
                   $" <li><strong>Order Total:</strong> {order.TotalPrice}</li>\r\n " +
                   $" <li><strong>Payment Method:</strong> {order.PaymentMethod}</li>\r\n   " +
                   $" <li><strong>Date:</strong> {order.CreatedAt}</li>\r\n  </ul>\r\n\r\n  " +
                   $"<div style=\"text-align: center; margin: 30px 0;\">\r\n    <a href=\"https://yourdomain.com/Orders/Details/@Model.OrderID\" style=\"padding: 12px 25px; background-color: #28a745; color: #fff; text-decoration: none; border-radius: 5px;\">\r\n      View Order Details\r\n    </a>\r\n  </div>\r\n\r\n " +
                   $" <p style=\"font-size: 14px; color: #999;\">\r\n    Need help? Just reply to this email and our support team will assist you.\r\n  </p>\r\n\r\n " +
                   $" <p style=\"font-size: 14px; color: #555; margin-top: 30px;\">\r\n    Thanks again for choosing <strong>Souq</strong>!<br />\r\n  " +
                   $"  <strong>Souq.eg</strong>\r\n  </p>\r\n</div>\r\n");

            return order;
        }

        public async Task<bool> SucceedOrder(string method, string? sessionID)
        {
            var userID = _users.GetUserId();
            var user = await _userManager.FindByIdAsync(userID);

            var order = await _servicesInstanceProvider.GetOrdersServiceInstance().GetUserCurrentOrder(userID);
            var orderID = order.ID;
            var totalPrice = order.OrderDetails.Sum(od => od.Price * od.Quantity);
            order.TotalPrice = totalPrice;

            if (order.PromoCodeDiscountType is not null)
            {
                if (order.PromoCodeDiscountType == "fixed")
                {
                    order.TotalPrice = totalPrice - order.PromoCodeDiscountValue ?? 0;
                }
                else
                {
                    order.TotalPrice = totalPrice * (1 - (order.PromoCodeDiscountValue ?? 0) / 100.0);
                }
            }

            if (method == "Paypal")
            {
                await _servicesInstanceProvider.GetOrdersServiceInstance().SetOrderPaymentMethodAndStatus(orderID, method, 1);
                await _servicesInstanceProvider.GetCartServiceInstance().EmptyCart();
                await _unitOfWork.Commit();

                await _emailSender.SendEmailAsync(user.Email, "Order Confirmation",
                    $"<div style=\"font-family: Amiri, serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;\">\r\n  " +
                    $"<h2 style=\"color: #27ae60;\">✅ Thank you for your order!</h2>\r\n " +
                    $" <p style=\"font-size: 16px; color: #555;\">\r\n    Hi {user.UserName}, we’ve received your order <strong>#{orderID}</strong> and it's now being processed.\r\n  </p>\r\n\r\n " +
                    $" <p style=\"font-size: 15px; color: #666;\">\r\n    Here’s a quick summary of your purchase:\r\n  </p>\r\n\r\n  <ul style=\"font-size: 14px; color: #444;\">\r\n  " +
                    $" <li><strong>Order Total:</strong> {order.TotalPrice}</li>\r\n " +
                    $" <li><strong>Payment Method:</strong> {order.PaymentMethod}</li>\r\n   " +
                    $" <li><strong>Date:</strong> {order.CreatedAt}</li>\r\n  </ul>\r\n\r\n  " +
                    $"<div style=\"text-align: center; margin: 30px 0;\">\r\n    <a href=\"https://yourdomain.com/Orders/Details/@Model.OrderID\" style=\"padding: 12px 25px; background-color: #28a745; color: #fff; text-decoration: none; border-radius: 5px;\">\r\n      View Order Details\r\n    </a>\r\n  </div>\r\n\r\n " +
                    $" <p style=\"font-size: 14px; color: #999;\">\r\n    Need help? Just reply to this email and our support team will assist you.\r\n  </p>\r\n\r\n " +
                    $" <p style=\"font-size: 14px; color: #555; margin-top: 30px;\">\r\n    Thanks again for choosing <strong>Souq</strong>!<br />\r\n  " +
                    $"  <strong>Souq.eg</strong>\r\n  </p>\r\n</div>\r\n");

                return true;
            }

            var service = new SessionService();
            Session session = service.Get(sessionID);

            if (session.PaymentStatus == "paid")
            {
                var transaction = session.PaymentIntentId.ToString();

                await _servicesInstanceProvider.GetOrdersServiceInstance().SetOrderPaymentMethodAndStatus(orderID, "Stripe", 1);
                await _servicesInstanceProvider.GetCartServiceInstance().EmptyCart();
                await _unitOfWork.Commit();

                await _emailSender.SendEmailAsync(user.Email, "Order Confirmation",
                   $"<div style=\"font-family: Amiri, serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;\">\r\n  " +
                   $"<h2 style=\"color: #27ae60;\">✅ Thank you for your order!</h2>\r\n " +
                   $" <p style=\"font-size: 16px; color: #555;\">\r\n    Hi {user.UserName}, we’ve received your order <strong>#{orderID}</strong> and it's now being processed.\r\n  </p>\r\n\r\n " +
                   $" <p style=\"font-size: 15px; color: #666;\">\r\n    Here’s a quick summary of your purchase:\r\n  </p>\r\n\r\n  <ul style=\"font-size: 14px; color: #444;\">\r\n  " +
                   $" <li><strong>Order Total:</strong> {order.TotalPrice}</li>\r\n " +
                   $" <li><strong>Payment Method:</strong> {order.PaymentMethod}</li>\r\n   " +
                   $" <li><strong>Date:</strong> {order.CreatedAt}</li>\r\n  </ul>\r\n\r\n  " +
                   $"<div style=\"text-align: center; margin: 30px 0;\">\r\n    <a href=\"https://yourdomain.com/Orders/Details/@Model.OrderID\" style=\"padding: 12px 25px; background-color: #28a745; color: #fff; text-decoration: none; border-radius: 5px;\">\r\n      View Order Details\r\n    </a>\r\n  </div>\r\n\r\n " +
                   $" <p style=\"font-size: 14px; color: #999;\">\r\n    Need help? Just reply to this email and our support team will assist you.\r\n  </p>\r\n\r\n " +
                   $" <p style=\"font-size: 14px; color: #555; margin-top: 30px;\">\r\n    Thanks again for choosing <strong>Souq</strong>!<br />\r\n  " +
                   $"  <strong>Souq.eg</strong>\r\n  </p>\r\n</div>\r\n");

                return true;
            }

            return false;
        }

        public async Task FaildOrder(string method)
        {
            var userID = _users.GetUserId();
            var orderID = (await _servicesInstanceProvider.GetOrdersServiceInstance().GetUserCurrentOrder(userID)).ID;

            await _servicesInstanceProvider.GetOrdersServiceInstance().SetOrderPaymentMethodAndStatus(orderID, method, 2);
            await _unitOfWork.Commit();
        }
    }
}
