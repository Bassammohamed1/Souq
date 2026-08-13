using X.PagedList;

namespace ApplicationLayer.DTOs
{
    public class CartDTO
    {
        public IPagedList<RepositoryCartDTO> Carts { get; set; }
        public double TotalPrice { get; set; }
        public double OldPrice { get; set; }
    }
}