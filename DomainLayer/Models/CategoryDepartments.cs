namespace DomainLayer.Models
{
    public class CategoryDepartments
    {
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}
