using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Souq.Models.Cart_Orders;
using Stripe.Checkout;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "User")]
    public class PaymentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaypalClient _paypalClient;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;

        public PaymentsController(PaypalClient paypalClient, IUnitOfWork unitOfWork, IEmailSender emailSender, IUserService userService, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _paypalClient = paypalClient;
            _emailSender = emailSender;
            _userService = userService;
            _userManager = userManager;
        }

        public async Task<IActionResult> PaymentMethods()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var order = _unitOfWork.Orders.GetUserCurrentOrder();
            if (order == null)
            {
                var items = _unitOfWork.Carts.GetUserCart().Result.Carts
                    .Select(i => new OrderDetails
                    {
                        ItemID = i.ItemId,
                        ItemType = i.ItemType,
                        Price = i.Price,
                        Quantity = i.Quantity,
                    }).ToList();

                var userOrder = await _unitOfWork.Orders.CreateOrder(items);

                return userOrder is not null ? View() : BadRequest();
            }

            return View();
        }

        [HttpPost]
        public IActionResult PaymentMethods(string paymentMethod)
        {
            switch (paymentMethod)
            {
                case "paypal":
                    return RedirectToAction("PaypalCheckout");
                case "stripe":
                    return RedirectToAction("StripeCheckout");
                case "cod":
                    return RedirectToAction("CODCheckout");
            }

            throw new ArgumentException();
        }

        public async Task<IActionResult> CODCheckout()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var userID = await _userService.GetUserId();
            var user = await _userManager.FindByIdAsync(userID);

            var order = await _unitOfWork.Orders.GetUserCurrentOrder();
            var orderID = order.ID;
            var totalPrice = order.OrderDetails.Sum(od => od.Price * od.Quantity);

            if (order.PromoCodeDiscountType == "fixed")
            {
                order.TotalPrice = totalPrice - order.PromoCodeDiscountValue ?? 0;
            }
            else
            {
                order.TotalPrice = totalPrice * (1 - (order.PromoCodeDiscountValue ?? 0) / 100.0);
            }

            await _unitOfWork.Orders.SetOrderPaymentMethodAndStatus(orderID, "COD", 3);
            await _unitOfWork.Carts.EmptyCart();
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

            return View("COD", order);
        }

        public async Task<IActionResult> StripeCheckout()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var domain = "https://localhost:44352/";

            var options = new SessionCreateOptions()
            {
                SuccessUrl = domain + $"Payments/SucceedOrder?method=Stripe",
                CancelUrl = domain + $"Payments/FailedOrder?method=Stripe",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };

            var items = _unitOfWork.Carts.GetUserCart().Result.Carts;

            foreach (var item in items)
            {
                var sessionListItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.Price * item.Quantity * 100),
                        Currency = "EGP",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = item.Name,
                        }
                    },
                    Quantity = item.Quantity,
                };
                options.LineItems.Add(sessionListItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            TempData["Session"] = session.Id;

            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> PaypalCheckout(int orderID)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            ViewBag.ClientId = _paypalClient.PayPalClientID;

            var items = await _unitOfWork.Carts.GetUserCart();

            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> PaypalOrder(int totalPrice, CancellationToken cancellationToken)
        {
            try
            {
                var price = totalPrice.ToString();
                var currency = "USD";

                var reference = _paypalClient.GetRandomInvoiceNumber();

                var response = await _paypalClient.CreateOrder(price, currency, reference);

                return Ok(new { response.id });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public async Task<IActionResult> PaypalCapture(string orderId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderId);

                var reference = response.purchase_units[0].reference_id;

                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public async Task<IActionResult> SucceedOrder(string method)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var userID = await _userService.GetUserId();
            var user = await _userManager.FindByIdAsync(userID);

            var order = await _unitOfWork.Orders.GetUserCurrentOrder();
            var orderID = order.ID;
            var totalPrice = order.OrderDetails.Sum(od => od.Price * od.Quantity);

            if (order.PromoCodeDiscountType == "fixed")
            {
                order.TotalPrice = totalPrice - order.PromoCodeDiscountValue ?? 0;
            }
            else
            {
                order.TotalPrice = totalPrice * (1 - (order.PromoCodeDiscountValue ?? 0) / 100.0);
            }

            if (method == "Paypal")
            {
                await _unitOfWork.Orders.SetOrderPaymentMethodAndStatus(orderID, method, 1);
                await _unitOfWork.Carts.EmptyCart();
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

                return View();
            }

            var service = new SessionService();
            Session session = service.Get(TempData["Session"].ToString());

            if (session.PaymentStatus == "paid")
            {
                var transaction = session.PaymentIntentId.ToString();

                await _unitOfWork.Orders.SetOrderPaymentMethodAndStatus(orderID, "Stripe", 1);
                await _unitOfWork.Carts.EmptyCart();
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

                return View();
            }

            return RedirectToAction("FailedOrder", "Stripe");
        }

        public async Task<IActionResult> FailedOrder(string method)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var orderID = _unitOfWork.Orders.GetUserCurrentOrder().Result.ID;

            await _unitOfWork.Orders.SetOrderPaymentMethodAndStatus(orderID, method, 2);
            await _unitOfWork.Commit();

            return View();
        }
    }
}