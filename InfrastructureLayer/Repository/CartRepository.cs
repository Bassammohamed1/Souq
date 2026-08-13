using DomainLayer.Interfaces;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using Souq.Models.Cart_Orders;

namespace InfrastructureLayer.Repository
{
    public class CartRepository : Repository<ShoppingCart>, ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ShoppingCart> GetUserShoppingCart(string userID)
        {
            return await _context.ShoppingCarts
                .Include(cd => cd.CartDetails)
                .FirstOrDefaultAsync(c => c.UserId == userID);
        }

        public IQueryable<CartDetails> GetUserCartDetails(string userID)
        {
            return _context.CartDetails
               .Where(cd => cd.ShoppingCart.UserId == userID);
        }

        public async Task<CartDetails> GetUserCartDetails(int shoppingCartID, int itemID, string itemType)
        {
            return await _context.CartDetails
               .FirstOrDefaultAsync(cd => cd.ShoppingCartID == shoppingCartID && cd.ItemID == itemID && cd.ItemType == itemType);
        }

        public async Task<IEnumerable<CartDetails>> GetAllCartDetails()
        {
            return await _context.CartDetails.ToListAsync();
        }

        public async Task<CartDetails> AddCartDetails(CartDetails cart)
        {
            await _context.CartDetails.AddAsync(cart);

            return cart;
        }

        public CartDetails RemoveCartDetails(CartDetails cart)
        {
            _context.CartDetails.Remove(cart);

            return cart;
        }

        public IQueryable<CartDetails> RemoveCartDetails(int shoppingCartID)
        {
            var cartDetails = _context.CartDetails
                   .Where(cd => cd.ShoppingCartID == shoppingCartID);

            if (cartDetails.Any())
                _context.CartDetails.RemoveRange(cartDetails);

            return cartDetails;
        }
    }
}