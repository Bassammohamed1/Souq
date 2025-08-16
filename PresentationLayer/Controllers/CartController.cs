using DomainLayer.Interfaces;
using InfrastructureLayer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaypalClient _paypalClient;

        public CartController(IUnitOfWork unitOfWork, PaypalClient paypalClient)
        {
            _unitOfWork = unitOfWork;
            _paypalClient = paypalClient;
        }

        public async Task<IActionResult> AddItemToCart(int itemId, string itemType, int? qty, int redirect = 0)
        {
            var cartCount = await _unitOfWork.Carts.Add(itemId, itemType, qty);

            if (redirect == 0)
                return Ok(cartCount);

            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> RemoveItemFromCart(int itemId, string itemType, int redirect = 0)
        {
            var cartCount = await _unitOfWork.Carts.Remove(itemId, itemType);

            if (redirect == 0)
                return Ok(cartCount);

            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> GetUserCart(int? page)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            int pageNumber = page ?? 1;
            int pageSize = 10;

            var cart = await _unitOfWork.Carts.GetUserCart();
            cart.Carts = cart.Carts.ToPagedList(pageNumber, pageSize);

            return View("ShoppingCart", cart);
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            int totalItems = await _unitOfWork.Carts.TotalItemsInCart();
            return Ok(totalItems);
        }

        [HttpPost]
        public async Task<IActionResult> ApplyPromoCode(string promoCode)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            if (string.IsNullOrWhiteSpace(promoCode))
            {
                ViewBag.PromoMessage = "Please enter a promo code.";
            }
            else
            {
                var promoCodeOffer = await _unitOfWork.Offers.IsPromoCodeExist(promoCode);

                if (promoCodeOffer is not null)
                {
                    var userCart = await _unitOfWork.Carts.GetUserCart();

                    var updatedCart = await _unitOfWork.Carts.ApplyPromoCode(userCart, promoCodeOffer);
                    updatedCart.Carts = updatedCart.Carts.ToPagedList(1, 10);

                    var promoSign = promoCodeOffer.PromoDiscountType == "fixed" ? "💲" : "%";
                    ViewBag.PromoMessage = $"Promo code applied: {promoCodeOffer.PromoDiscountValue}{promoSign} discount!";

                    return View("ShoppingCart", updatedCart);
                }
                else
                {
                    ViewBag.PromoMessage = "Invalid promo code.";
                }
            }

            var cart = await _unitOfWork.Carts.GetUserCart();
            cart.Carts = cart.Carts.ToPagedList(1, 10);

            return View("ShoppingCart", cart);
        }
    }
}