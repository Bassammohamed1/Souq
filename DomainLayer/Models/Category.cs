using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Category
    {
        public int ID { get; set; }
        [Required, MaxLength(30)]
        public string Name { get; set; }
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
        public List<CategoryDepartments>? CategoryDepartments { get; set; }
        public List<MobilePhone>? MobilePhones { get; set; }
        public List<TV>? Tvs { get; set; }
        public List<HeadPhone>? HeadPhones { get; set; }
        public List<Laptop>? Laptops { get; set; }
        public List<WashingMachine>? WashingMachines { get; set; }
        public List<Cooker>? Cookers { get; set; }
        public List<AirConditioner>? AirConditioners { get; set; }
        public List<Fridge>? Fridges { get; set; }
        public List<VideoGame>? VideoGames { get; set; }
    }
}
