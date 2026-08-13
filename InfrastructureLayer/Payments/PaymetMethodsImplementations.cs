using ApplicationLayer.DTOs.Payments;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using InfrastructureLayer.Helpers;
using Stripe.Checkout;

namespace InfrastructureLayer.Payments
{
    public class PaymetMethodsImplementations : IPaymentMethodsImplementations
    {
        private readonly ICartService _cart;
        private readonly IOrdersService _orders;
        private readonly IUsersService _users;
        private readonly PaypalClient _paypalClient;

        public PaymetMethodsImplementations(ICartService cart, PaypalClient paypalClient, IOrdersService orders, IUsersService users)
        {
            _cart = cart;
            _paypalClient = paypalClient;
            _orders = orders;
            _users = users;
        }

        public async Task<PaypalCheckoutDTO> PaypalCheckout()
        {
            var userID = _users.GetUserId();

            var clientID = _paypalClient.PayPalClientID;

            var cart = await _cart.GetUserCart();

            var order = await _orders.GetUserCurrentOrder(userID);
            var orderID = order.ID;
            var totalPrice = cart.Carts.Sum(od => od.Price * od.Quantity);
           
            if (order.PromoCodeDiscountType is not null)
            {
                cart.OldPrice = totalPrice;

                if (order.PromoCodeDiscountType == "fixed")
                {
                    cart.TotalPrice = totalPrice - order.PromoCodeDiscountValue ?? 0;
                }
                else
                {
                    cart.TotalPrice = totalPrice * (1 - (order.PromoCodeDiscountValue ?? 0) / 100.0);
                }
            }
                
            return new PaypalCheckoutDTO()
            {
                ClientID = clientID,
                UserCart = cart
            };
        }

        public async Task<StripeCheckoutDTO> StripeCheckout()
        {
            var domain = "https://localhost:44352/";

            var options = new SessionCreateOptions()
            {
                SuccessUrl = domain + $"Payments/SucceedOrder?method=Stripe",
                CancelUrl = domain + $"Payments/FailedOrder?method=Stripe",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };

            var items = (await _cart.GetUserCart()).Carts;

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

            return new StripeCheckoutDTO()
            {
                SessionID = session.Id,
                SessionURL = session.Url
            };
        }

        public async Task<CreatePaypalOrderDTO> CreatePaypalOrder(int totalPrice, CancellationToken cancellationToken)
        {
            try
            {
                var price = totalPrice.ToString();
                var currency = "USD";

                var reference = _paypalClient.GetRandomInvoiceNumber();

                var response = await _paypalClient.CreateOrder(price, currency, reference);

                return new CreatePaypalOrderDTO { Succeed = true, ResponseID = response.id };
            }
            catch (Exception e)
            {
                return new CreatePaypalOrderDTO() { Succeed = false, Error = e.Message };
            }
        }

        public async Task<CapturePaypalOrderDTO> CapturePaypalOrder(string orderId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderId);

                var reference = response.purchase_units[0].reference_id;

                return new CapturePaypalOrderDTO() { Succeed = true, Response = response };
            }
            catch (Exception e)
            {
                return new CapturePaypalOrderDTO() { Succeed = false, Error = e.Message };
            }
        }
    }
}