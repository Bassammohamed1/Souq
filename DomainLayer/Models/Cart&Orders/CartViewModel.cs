using X.PagedList;

namespace Souq.Models.Cart_Orders
{
    public class CartViewModel
    {
        public IPagedList<RepositoryCartVM> Carts { get; set; }
        public double TotalPrice { get; set; }
    }
}
