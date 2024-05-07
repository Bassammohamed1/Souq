using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Models
{
    public class Department
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(150)]
        public string Description { get; set; }
        public List<Item>? Items { get; set; }
    }
}
