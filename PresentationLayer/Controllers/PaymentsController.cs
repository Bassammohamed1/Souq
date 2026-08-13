using ApplicationLayer.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Souq.Models.Cart_Orders;
using X.PagedList.Extensions;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "User")]
    public class PaymentsController : Controller
    {
        private readonly IDepartmentsService _departments;
        private readonly IPaymentsService _payments;
        private readonly IPaymentMethodsImplementations _paymentMethods;

        public PaymentsController(IDepartmentsService departments, IPaymentsService payments, IPaymentMethodsImplementations paymentMethods)
        {
            _departments = departments;
            _payments = payments;
            _paymentMethods = paymentMethods;
        }

        public async Task<IActionResult> PaymentMethods()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var order = await _payments.GetUserCurrentOrderOrCreateIt();

            return order is not null ? View() : BadRequest();
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
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var order = await _payments.CODCheckout();

            return View("COD", order);
        }

        public async Task<IActionResult> StripeCheckout()
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _paymentMethods.StripeCheckout();

            TempData["Session"] = result.SessionID;

            Response.Headers.Add("Location", result.SessionURL);

            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> PaypalCheckout(int orderID)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _paymentMethods.PaypalCheckout();

            ViewBag.ClientId = result.ClientID;

            var cartVM = new CartViewModel
            {
                Carts = result.UserCart.Carts
                .Select(c => new RepositoryCartVM
                {
                    ItemId = c.ItemId,
                    ItemType = c.ItemType,
                    Name = c.Name,
                    Price = c.Price,
                    Quantity = c.Quantity,
                    imageSrc = c.imageSrc
                }).ToPagedList(),
                TotalPrice = result.UserCart.TotalPrice,
                OldPrice = result.UserCart.OldPrice
            };

            return View(cartVM);
        }

        [HttpPost]
        public async Task<IActionResult> PaypalOrder(int totalPrice, CancellationToken cancellationToken)
        {
            var result = await _paymentMethods.CreatePaypalOrder(totalPrice, cancellationToken);

            return result.Succeed ? Ok(new { id = result.ResponseID }) :
                BadRequest(result.Error);
        }

        public async Task<IActionResult> PaypalCapture(string orderId, CancellationToken cancellationToken)
        {
            var result = await _paymentMethods.CapturePaypalOrder(orderId, cancellationToken);

            return result.Succeed ? Ok(result.Response) :
                BadRequest(result.Error);
        }

        public async Task<IActionResult> SucceedOrder(string method)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            var result = await _payments.SucceedOrder(method, TempData["Session"]?.ToString());

            return result ? View() : RedirectToAction("FailedOrder", "Stripe");
        }

        public async Task<IActionResult> FailedOrder(string method)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            await _payments.FaildOrder(method);

            return View();
        }
    }
}