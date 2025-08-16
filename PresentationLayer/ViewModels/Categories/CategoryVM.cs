using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PresentationLayer.ViewModels.Categories
{
    public class CategoryVM
    {
        public int Id { get; set; }
        [Required, MaxLength(25)]
        public string Name { get; set; }
        [Required, NotMapped, DisplayName("Image")]
        public IFormFile clientFile { get; set; }
        [Required, DisplayName("Department")]
        public List<int> DepartmentsIds { get; set; }
    }
}
