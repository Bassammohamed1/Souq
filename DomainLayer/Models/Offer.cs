using DomainLayer.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Offer
    {
        public int ID { get; set; }
        public OfferType OfferType { get; set; }
        public string? DepartmentName { get; set; }
        public string? CategoryName { get; set; }
        public int? ItemID { get; set; }
        public double? FixedDiscountValue { get; set; }
        public double? PercentDiscount { get; set; }
        public int? ItemOneID { get; set; }
        public int? ItemTwoID { get; set; }
        public string? PromoCode { get; set; }
        public string? PromoDiscountType { get; set; }
        public double? PromoDiscountValue { get; set; }
        [NotMapped]
        public IFormFile ClientFile { get; set; }
        public byte[]? dbImage { get; set; }
        [NotMapped]
        public string? ImageSrc
        {
            get
            {
                if (dbImage != null)
                {
                    string base64String = Convert.ToBase64String(dbImage, 0, dbImage.Length);
                    return "data:image/jpg;base64," + base64String;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
