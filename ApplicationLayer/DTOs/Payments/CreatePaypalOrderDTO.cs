namespace ApplicationLayer.DTOs.Payments
{
    public class CreatePaypalOrderDTO
    {
        public bool Succeed { get; set; }
        public string? ResponseID { get; set; }
        public string? Error { get; set; }
    }
}
