using DomainLayer.Models;

namespace PresentationLayer.ViewModels
{
    public class CookerViewModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public double? Rate { get; set; }
        public double? Price { get; set; }
        public double? NewPrice { get; set; }
        public bool IsDiscounted { get; set; }
        public string? DiscountValue { get; set; }
        public bool IsBOGOBuy { get; set; }
        public bool IsBOGOGet { get; set; }
        public string? imageSrc { get; set; }
        public string? ItemDimensions { get; set; }
        public string? DrawerType { get; set; }
        public string? Material { get; set; }
        public string? FinishType { get; set; }
        public string? ModelName { get; set; }
        public string? FormFactor { get; set; }
        public string? ControlsType { get; set; }
        public string? SpecialFeatures { get; set; }
        public string? Color { get; set; }
        public string? ItemWeight { get; set; }
        public int? NumberOfHeatingElements { get; set; }
        public int? TotalQuantity { get; set; }
        public bool isLiked { get; set; } = false;
        public string? CategoryName { get; set; }
        public string? ControllerName { get; set; }
        public List<CookerViewModel>? RelatedCookers { get; set; }
        public List<CookerViewModel>? SimilarPriceCookers { get; set; }
        public IQueryable<Comment>? Comments { get; set; }
        public IQueryable<Offer>? Offers { get; set; }
        public Item? BOGOGet { get; set; }
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
