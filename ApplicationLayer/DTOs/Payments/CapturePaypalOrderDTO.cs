
namespace ApplicationLayer.DTOs.Payments
{
    public class CapturePaypalOrderDTO
    {
        public object? Response { get; set; }
        public bool Succeed { get; set; }
        public string? Error { get; set; }
    }
}
