using DomainLayer.Enums;
using DomainLayer.Models;

namespace PresentationLayer.ViewModels
{
    public class OfferViewModel
    {
        public int ID { get; set; }
        public string? DepartmentName { get; set; }
        public string? CategoryName { get; set; }
        public int? ItemID { get; set; }
        public OfferType OfferType { get; set; }
        public double? FixedDiscountValue { get; set; }
        public double? PercentDiscount { get; set; }
        public int? ItemOneID { get; set; }
        public int? ItemTwoID { get; set; }
        public string? PromoCode { get; set; }
        public string? PromoDiscountType { get; set; }
        public double? PromoDiscountValue { get; set; }
        public string? ImageSrc { get; set; }
        public IFormFile ClientFile { get; set; }
        public List<Department>? Departments { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Item>? Items { get; set; }
    }
}