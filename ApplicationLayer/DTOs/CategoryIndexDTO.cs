
namespace ApplicationLayer.DTOs
{
    public class CategoryIndexDTO
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public List<string> Departments { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
