using Microsoft.AspNetCore.Http;
using Souq.Models.Cart_Orders;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public abstract class Item
    {
        public int ID { get; set; }
        [Required, MaxLength(250)]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required, DisplayName("Date")]
        public DateTime AddedOn { get; set; } = DateTime.Now;
        [Required, Range(0, 5)]
        public double Rate { get; set; } = 0;
        [Required]
        public bool IsDiscounted { get; set; } = false;
        public bool IsBOGOBuy { get; set; } = false;
        public bool IsBOGOGet { get; set; } = false;
        public double? NewPrice { get; set; }
        [Required, NotMapped, DisplayName("Image")]
        public IFormFile clientFile { get; set; }
        public byte[]? dbImage { get; set; }
        [NotMapped]
        public string? imageSrc
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
        [Required, DisplayName("Category")]
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
        public List<CartDetails>? CartDetails { get; set; }
    }
}
