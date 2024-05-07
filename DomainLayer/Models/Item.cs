using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        [DisplayName("date")]
        public DateTime AddedOn { get; set; }
        public string ItemType { get; set; }
        [NotMapped, DisplayName("Image")]
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
        [DisplayName("Department")]
        public int DepartmentID { get; set; }
        [ForeignKey(nameof(DepartmentID))]
        public Department? Department { get; set; }
    }
}
