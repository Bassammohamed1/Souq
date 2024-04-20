using DomainLayer.Enums;

namespace DomainLayer.Models
{
    public class Laptop : BaseModel
    {
        public LaptopsCategory Category { get; set; }
    }
}
