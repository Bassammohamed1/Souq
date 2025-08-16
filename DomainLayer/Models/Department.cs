using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Models
{
    public class Department
    {
        public int ID { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        public List<CategoryDepartments>? CategoryDepartments { get; set; }
    }
}
