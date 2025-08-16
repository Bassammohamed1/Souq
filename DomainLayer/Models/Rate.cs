using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Rate
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser? User { get; set; }
        public int ItemId { get; set; }
        public string ItemType { get; set; }
        public string[] StarStates
        {
            get
            {
                var states = new string[5];
                var remaining = Value;
                for (int i = 0; i < 5; i++)
                {
                    if (remaining >= 1)
                    {
                        states[i] = "full";
                        remaining -= 1;
                    }
                    else if (remaining >= 0.5)
                    {
                        states[i] = "half";
                        remaining = 0;
                    }
                    else
                    {
                        states[i] = "empty";
                    }
                }
                return states;
            }
        }
    }
}
