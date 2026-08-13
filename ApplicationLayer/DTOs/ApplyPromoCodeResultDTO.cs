
namespace ApplicationLayer.DTOs
{
    public class ApplyPromoCodeResultDTO
    {
        public bool Success { get; set; }
        public CartDTO Cart { get; set; }
        public string? PromoMessage { get; set; }
        public string? Error { get; set; }
        public double OldPrice { get; set; }
    }
}