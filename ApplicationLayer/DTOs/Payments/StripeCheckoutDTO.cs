namespace ApplicationLayer.DTOs.Payments
{
    public class StripeCheckoutDTO
    {
        public string SessionID { get; set; }
        public string SessionURL { get; set; }
    }
}