using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.DTOs
{
    public class CategoryDTO
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
