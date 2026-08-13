using Souq.Models.Cart_Orders;

namespace DomainLayer.Interfaces
{
    public interface ICartRepository : IRepository<ShoppingCart>
    {
        Task<ShoppingCart> GetUserShoppingCart(string userID);
        IQueryable<CartDetails> GetUserCartDetails(string userID);
        Task<CartDetails> GetUserCartDetails(int shoppingCartID, int itemID, string itemType);
        Task<IEnumerable<CartDetails>> GetAllCartDetails();
        Task<CartDetails> AddCartDetails(CartDetails cart);
        CartDetails RemoveCartDetails(CartDetails cart);
        IQueryable<CartDetails> RemoveCartDetails(int shoppingCartID);
    }
}