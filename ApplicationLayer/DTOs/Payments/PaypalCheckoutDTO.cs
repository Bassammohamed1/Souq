namespace ApplicationLayer.DTOs.Payments
{
    public class PaypalCheckoutDTO
    {
        public string ClientID { get; set; }
        public CartDTO UserCart { get; set; }
    }
}
