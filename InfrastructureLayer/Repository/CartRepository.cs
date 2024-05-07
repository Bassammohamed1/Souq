using DomainLayer.Interfaces;
using DomainLayer.Models.Cart;
using InfrastructureLayer.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartRepository(AppDbContext db, IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> AddItem(int itemId, int qty)
        {
            var item = _db.Items.Find(itemId);
            if (item.Amount < qty)
                throw new Exception("No more items");
            string userId = GetUserId();
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges();
                // cart detail section
                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.ItemId == itemId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                    item.Amount -= qty;
                }
                else
                {
                    cartItem = new CartDetail
                    {
                        ItemId = itemId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                    };
                    item.Amount -= qty;
                    _db.CartDetails.Add(cartItem);
                }
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }
        public async Task<int> RemoveItem(int itemId)
        {
            var item = _db.Items.Find(itemId);
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("Invalid cart");
                // cart detail section
                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.ItemId == itemId);
                if (cartItem is null)
                    throw new Exception("Not items in cart");
                else if (cartItem.Quantity == 1)
                {
                    _db.CartDetails.Remove(cartItem);
                    item.Amount += 1;
                }
                else
                {
                    cartItem.Quantity = cartItem.Quantity - 1;
                    item.Amount += cartItem.Quantity;
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }
        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new Exception("Invalid UserId");
            var shoppingCart = await _db.ShoppingCarts
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Item)
                                  .ThenInclude(a => a.Department)
                                  .Where(a => a.UserId == userId).FirstOrDefaultAsync();
            return shoppingCart;

        }
        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }
            var data = await (from cart in _db.ShoppingCarts
                              join cartDetail in _db.CartDetails
                              on cart.Id equals cartDetail.ShoppingCartId
                              where cart.UserId == userId
                              select new { cartDetail.Id }
                        ).ToListAsync();
            return data.Count;
        }
        public async Task<bool> DoCheckout()
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("Invalid cart");
                var cartDetail = _db.CartDetails
                                    .Where(a => a.ShoppingCartId == cart.Id).ToList();
                if (cartDetail.Count == 0)
                    throw new Exception("Cart is empty");
                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    OrderStatusId = 1//pending
                };
                _db.Orders.Add(order);
                _db.SaveChanges();
                foreach (var item in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        ItemId = item.ItemId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    _db.OrderDetails.Add(orderDetail);
                }
                _db.SaveChanges();
                _db.CartDetails.RemoveRange(cartDetail);
                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
        private async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }
    }
}
