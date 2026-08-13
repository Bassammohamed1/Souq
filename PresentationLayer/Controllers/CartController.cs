using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Souq.Models.Cart_Orders;
using X.PagedList.Extensions;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly ICartService _carts;
        private readonly IDepartmentsService _departments;

        public CartController(ICartService carts, IDepartmentsService departments)
        {
            _carts = carts;
            _departments = departments;
        }

        public async Task<IActionResult> AddItemToCart(int itemId, string itemType, int? qty, int redirect = 0)
        {
            var cartCount = await _carts.Add(itemId, itemType, qty);

            if (redirect == 0)
                return Ok(cartCount);

            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> RemoveItemFromCart(int itemId, string itemType, int redirect = 0)
        {
            var cartCount = await _carts.Remove(itemId, itemType);

            if (redirect == 0)
                return Ok(cartCount);

            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> GetUserCart(int? page)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            int pageNumber = page ?? 1;
            int pageSize = 10;

            var cart = await _carts.GetUserCart();
            cart.Carts = cart.Carts.ToPagedList(pageNumber, pageSize);

            return View("ShoppingCart", new CartViewModel
            {
                Carts = cart.Carts.Select(c => new RepositoryCartVM
                {
                    imageSrc = c.imageSrc,
                    ItemId = c.ItemId,
                    ItemType = c.ItemType,
                    Name = c.Name,
                    Price = c.Price,
                    Quantity = c.Quantity
                }),
                TotalPrice = cart.TotalPrice
            });
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            int totalItems = await _carts.TotalItemsInCart();

            return Ok(totalItems);
        }

        [HttpPost]
        public async Task<IActionResult> ApplyPromoCode(string promoCode)
        {
            var departments = await _departments.GetDepartments();
            ViewData["Departments"] = departments;

            if (string.IsNullOrWhiteSpace(promoCode))
                ViewBag.PromoMessage = "Please enter a promo code.";
            else
            {
                var result = await _carts.ApplyPromoCode(promoCode);

                if (result.Success)
                {
                    ViewBag.PromoMessage = result.PromoMessage;
                    var cartVM = new CartViewModel
                    {
                        Carts = result.Cart.Carts.Select(c => new RepositoryCartVM
                        {
                            imageSrc = c.imageSrc,
                            ItemId = c.ItemId,
                            ItemType = c.ItemType,
                            Name = c.Name,
                            Price = c.Price,
                            Quantity = c.Quantity
                        }),
                        TotalPrice = result.Cart.TotalPrice,
                        OldPrice = result.OldPrice
                    };

                    return View("ShoppingCart", cartVM);
                }
                else
                    ViewBag.PromoMessage = result.Error;
            }

            var cart = await _carts.GetUserCart();
            cart.Carts = cart.Carts.ToPagedList(1, 10);

            var cartVM2 = new CartViewModel
            {
                Carts = cart.Carts.Select(c => new RepositoryCartVM
                {
                    imageSrc = c.imageSrc,
                    ItemId = c.ItemId,
                    ItemType = c.ItemType,
                    Name = c.Name,
                    Price = c.Price,
                    Quantity = c.Quantity
                }),
                TotalPrice = cart.TotalPrice
            };

            return View("ShoppingCart", cartVM2);
        }
    }
}