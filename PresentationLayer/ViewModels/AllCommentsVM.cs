using DomainLayer.Models;

namespace PresentationLayer.ViewModels
{
    public class AllCommentsVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Rate { get; set; }
        public string CategoryName { get; set; }
        public IQueryable<Comment>? Comments { get; set; }
        public int[]? StarCounts { get; set; }
        public int? RateCount { get; set; }
        public string[] StarStates
        {
            get
            {
                var states = new string[5];
                var remaining = Rate;
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
        public int[] StarPercentages
        {
            get
            {
                if (RateCount == 0) return new int[5];
                return StarCounts
                    .Select(c => (int)Math.Round((decimal)(100.0 * c / RateCount)))
                    .ToArray();
            }
        }
    }
}
