using Souq.Data.Enums;

namespace Souq.Models
{
    public class Laptop : BaseModel
    {
        public LaptopsCategory Category { get; set; }
    }
}
