using ApplicationLayer.DTOs.Payments;

namespace ApplicationLayer.Interfaces.ServicesInterfaces
{
    public interface IPaymentMethodsImplementations
    {
        Task<StripeCheckoutDTO> StripeCheckout();
        Task<PaypalCheckoutDTO> PaypalCheckout();
        Task<CreatePaypalOrderDTO> CreatePaypalOrder(int totalPrice, CancellationToken cancellationToken);
        Task<CapturePaypalOrderDTO> CapturePaypalOrder(string orderId, CancellationToken cancellationToken);
    }
}
