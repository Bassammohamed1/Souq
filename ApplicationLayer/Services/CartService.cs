using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Souq.Models.Cart_Orders;
using X.PagedList;
using X.PagedList.Extensions;

namespace ApplicationLayer.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public CartService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<int> Add(int itemID, string itemType, int? qty)
        {
            Item? item = itemType switch
            {
                "AirConditioners" => await _unitOfWork.AirConditioners.GetById(itemID),

                "Fridges" => await _unitOfWork.Fridges.GetById(itemID),

                "Cookers" => await _unitOfWork.Cookers.GetById(itemID),

                "WashingMachines" => await _unitOfWork.WashingMachines.GetById(itemID),

                "Laptops" => await _unitOfWork.Laptops.GetById(itemID),

                "HeadPhones" => await _unitOfWork.HeadPhones.GetById(itemID),

                "TVs" => await _unitOfWork.TVs.GetById(itemID),

                "MobilePhones" => await _unitOfWork.MobilePhones.GetById(itemID),

                "VideoGames" => await _unitOfWork.VideoGames.GetById(itemID),

                _ => throw new ArgumentException()
            };

            Item getItem = null;

            if (item.IsBOGOBuy)
            {
                getItem = await _servicesInstanceProvider.GetOffersServiceInstance().GetBOGOGetItem(item);
            }

            var userID = _userService.GetUserId();

            if (string.IsNullOrEmpty(userID))
                throw new InvalidOperationException();

            var userCart = await _unitOfWork.Carts.GetUserShoppingCart(userID);

            if (userCart is not null)
            {
                if (qty == null)
                {
                    var itemCartDetails = new CartDetails()
                    {
                        ItemID = item.ID,
                        ItemType = itemType,
                        Price = item.NewPrice.HasValue ? item.NewPrice ?? 0 : item.Price,
                        Quantity = 1,
                        ShoppingCartID = userCart.Id
                    };

                    item.Amount -= 1;

                    var result = await _unitOfWork.Carts.AddCartDetails(itemCartDetails);

                    if (result is null)
                        return -1;

                    await _unitOfWork.Commit();

                    if (getItem is not null)
                    {
                        var getItemCartDetails = new CartDetails()
                        {
                            ItemID = getItem.ID,
                            ItemType = "-",
                            Price = 0,
                            Quantity = 1,
                            ShoppingCartID = userCart.Id
                        };

                        getItem.Amount -= 1;

                        var result2 = await _unitOfWork.Carts.AddCartDetails(getItemCartDetails);

                        if (result2 is null)
                            return -1;

                        await _unitOfWork.Commit();
                    }
                }
                else
                {
                    var userCartDetails = await _unitOfWork.Carts.GetUserCartDetails(userCart.Id, itemID, itemType);

                    if (userCartDetails is not null)
                    {
                        userCartDetails.Quantity += 1;
                        item.Amount -= 1;

                        await _unitOfWork.Commit();

                        return await TotalItemsInCart();
                    }

                    var cartDetails = new CartDetails()
                    {
                        ItemID = item.ID,
                        ItemType = itemType,
                        Price = item.NewPrice.HasValue ? item.NewPrice ?? 0 : item.Price,
                        Quantity = 1,
                        ShoppingCartID = userCart.Id
                    };

                    item.Amount -= 1;

                    var result = await _unitOfWork.Carts.AddCartDetails(cartDetails);

                    if (result is null)
                        return -1;

                    await _unitOfWork.Commit();
                }
            }
            else
            {
                var userShoppingCart = new ShoppingCart()
                {
                    UserId = userID
                };

                var result = await _unitOfWork.Carts.Add(userShoppingCart);

                if (result is null)
                    return -1;

                var cartDetails = new CartDetails()
                {
                    ItemID = item.ID,
                    ItemType = itemType,
                    Price = item.NewPrice.HasValue ? item.NewPrice ?? 0 : item.Price,
                    Quantity = 1,
                    ShoppingCartID = userShoppingCart.Id
                };

                item.Amount -= 1;

                var result2 = await _unitOfWork.Carts.AddCartDetails(cartDetails);

                if (result2 is null)
                    return -1;

                if (getItem is not null)
                {
                    var getItemCartDetails = new CartDetails()
                    {
                        ItemID = getItem.ID,
                        ItemType = "-",
                        Price = 0,
                        Quantity = 1,
                        ShoppingCartID = userCart.Id
                    };

                    getItem.Amount -= 1;

                    var result3 = await _unitOfWork.Carts.AddCartDetails(getItemCartDetails);

                    if (result3 is null)
                        return -1;
                }

                await _unitOfWork.Commit();
            }

            return await TotalItemsInCart();
        }

        public async Task<int> Remove(int itemID, string itemType)
        {
            Item? item = itemType switch
            {
                "AirConditioners" => await _unitOfWork.AirConditioners.GetById(itemID),

                "Fridges" => await _unitOfWork.Fridges.GetById(itemID),

                "Cookers" => await _unitOfWork.Cookers.GetById(itemID),

                "WashingMachines" => await _unitOfWork.WashingMachines.GetById(itemID),

                "Laptops" => await _unitOfWork.Laptops.GetById(itemID),

                "HeadPhones" => await _unitOfWork.HeadPhones.GetById(itemID),

                "TVs" => await _unitOfWork.TVs.GetById(itemID),

                "MobilePhones" => await _unitOfWork.MobilePhones.GetById(itemID),

                "VideoGames" => await _unitOfWork.VideoGames.GetById(itemID),

                _ => throw new ArgumentException()
            };

            var userID = _userService.GetUserId();

            if (string.IsNullOrEmpty(userID))
                throw new InvalidOperationException();

            var userCart = await _unitOfWork.Carts.GetUserShoppingCart(userID);

            if (userCart is not null)
            {
                var userCartDetails = await _unitOfWork.Carts.GetUserCartDetails(userCart.Id, itemID, itemType);

                if (userCartDetails.Quantity > 1)
                {
                    userCartDetails.Quantity -= 1;
                    item.Amount += 1;

                    await _unitOfWork.Commit();
                }
                else if (userCartDetails.Quantity == 1)
                {
                    Item getItem = null;

                    if (item.IsBOGOBuy)
                    {
                        getItem = await _servicesInstanceProvider.GetOffersServiceInstance().GetBOGOGetItem(item);

                        var getItemCartDetails = (await _unitOfWork.Carts.GetAllCartDetails())
                            .FirstOrDefault(cd => cd.ItemID == getItem.ID && cd.Price == 0 && cd.ShoppingCartID == userCart.Id);

                        getItem.Amount += 1;

                        var result1 = _unitOfWork.Carts.RemoveCartDetails(getItemCartDetails);

                        if (result1 is null)
                            return -1;

                        await _unitOfWork.Commit();
                    }

                    item.Amount += 1;

                    var result2 = _unitOfWork.Carts.RemoveCartDetails(userCartDetails);

                    if (result2 is null)
                        return -1;

                    await _unitOfWork.Commit();
                }

                return await TotalItemsInCart();
            }

            throw new InvalidOperationException();
        }

        public async Task<CartDTO> GetUserCart()
        {
            var userID = _userService.GetUserId();

            var userCart = await _unitOfWork.Carts.GetUserShoppingCart(userID);

            if (userCart is not null)
            {
                var cartItems = userCart?.CartDetails?
                    .Select(cd =>
                    {
                        string? name = null;
                        double? price = null;
                        string? imageSrc = null;

                        switch (cd.ItemType)
                        {
                            case "AirConditioners":
                                var airConditioner = _unitOfWork.AirConditioners.GetById(cd.ItemID).Result;
                                name = airConditioner?.Name;
                                imageSrc = airConditioner?.imageSrc;
                                price = airConditioner.NewPrice.HasValue ? airConditioner?.NewPrice : airConditioner?.Price;
                                break;

                            case "Fridges":
                                var fridge = _unitOfWork.Fridges.GetById(cd.ItemID).Result;
                                name = fridge?.Name;
                                imageSrc = fridge?.imageSrc;
                                price = fridge.NewPrice.HasValue ? fridge?.NewPrice : fridge?.Price;
                                break;

                            case "Cookers":
                                var cooker = _unitOfWork.Cookers.GetById(cd.ItemID).Result;
                                name = cooker?.Name;
                                imageSrc = cooker?.imageSrc;
                                price = cooker.NewPrice.HasValue ? cooker?.NewPrice : cooker?.Price;
                                break;

                            case "WashingMachines":
                                var washingMachine = _unitOfWork.WashingMachines.GetById(cd.ItemID).Result;
                                name = washingMachine?.Name;
                                imageSrc = washingMachine?.imageSrc;
                                price = washingMachine.NewPrice.HasValue ? washingMachine?.NewPrice : washingMachine?.Price;
                                break;

                            case "Laptops":
                                var laptop = _unitOfWork.Laptops.GetById(cd.ItemID).Result;
                                name = laptop?.Name;
                                imageSrc = laptop?.imageSrc;
                                price = laptop.NewPrice.HasValue ? laptop?.NewPrice : laptop?.Price;
                                break;

                            case "TVs":
                                var tv = _unitOfWork.TVs.GetById(cd.ItemID).Result;
                                name = tv?.Name;
                                imageSrc = tv?.imageSrc;
                                price = tv.NewPrice.HasValue ? tv?.NewPrice : tv?.Price;
                                break;

                            case "HeadPhones":
                                var headPhone = _unitOfWork.HeadPhones.GetById(cd.ItemID).Result;
                                name = headPhone?.Name;
                                imageSrc = headPhone?.imageSrc;
                                price = headPhone.NewPrice.HasValue ? headPhone?.NewPrice : headPhone?.Price;
                                break;

                            case "MobilePhones":
                                var mobilePhone = _unitOfWork.MobilePhones.GetById(cd.ItemID).Result;
                                name = mobilePhone?.Name;
                                imageSrc = mobilePhone?.imageSrc;
                                price = mobilePhone.NewPrice.HasValue ? mobilePhone?.NewPrice : mobilePhone?.Price;
                                break;

                            case "VideoGames":
                                var videoGame = _unitOfWork.VideoGames.GetById(cd.ItemID).Result;
                                name = videoGame?.Name;
                                imageSrc = videoGame?.imageSrc;
                                price = videoGame.NewPrice.HasValue ? videoGame?.NewPrice : videoGame?.Price;
                                break;

                            default:
                                var getItem = _servicesInstanceProvider.GetItemsServiceInstance().GetItem(cd.ItemID).Result;
                                name = getItem?.Name;
                                imageSrc = getItem?.imageSrc;
                                price = 0;
                                break;
                        }

                        return new RepositoryCartDTO
                        {
                            ItemId = cd.ItemID,
                            ItemType = cd.ItemType,
                            Name = name,
                            Price = price ?? 0.0,
                            imageSrc = imageSrc,
                            Quantity = cd.Quantity
                        };
                    }).ToList();

                var cart = new CartDTO()
                {
                    Carts = cartItems.ToPagedList() ?? Enumerable.Empty<RepositoryCartDTO>().ToPagedList(),
                    TotalPrice = cartItems.Sum(c => c.Price * c.Quantity)
                };

                return cart;
            }

            var userShoppingCart = new ShoppingCart()
            {
                UserId = userID
            };

            await _unitOfWork.Carts.Add(userShoppingCart);
            await _unitOfWork.Commit();

            return null;
        }

        public async Task<IPagedList<RepositoryCartDTO>> GetCartItems()
        {
            var userCart = await this.GetUserCart();

            return userCart.Carts;
        }

        public async Task<int> TotalItemsInCart()
        {
            var userID = _userService.GetUserId();

            if (string.IsNullOrEmpty(userID))
                throw new InvalidOperationException();

            var userCart = await _unitOfWork.Carts.GetUserShoppingCart(userID);

            if (userCart is not null)
            {
                var totalCartCount = _unitOfWork.Carts.GetUserCartDetails(userID)
                    .Sum(cd => cd.Quantity);

                return totalCartCount;
            }

            return 0;
        }

        public async Task<int> TotalItemQuantityInCart(int itemID, string itemType)
        {
            var userID = _userService.GetUserId();

            var userCart = await _unitOfWork.Carts.GetUserShoppingCart(userID);

            if (userCart is not null)
            {
                var userCartDetails = await _unitOfWork.Carts.GetUserCartDetails(userCart.Id, itemID, itemType);

                return userCartDetails is not null ? userCartDetails.Quantity : 0;
            }

            throw new InvalidOperationException();
        }

        public async Task EmptyCart()
        {
            var userID = _userService.GetUserId();

            var userCart = await _unitOfWork.Carts.GetUserShoppingCart(userID);

            if (userCart is not null)
            {
                _unitOfWork.Carts.RemoveCartDetails(userCart.Id);

                return;
            }

            throw new InvalidOperationException();
        }

        public async Task<ApplyPromoCodeResultDTO> ApplyPromoCode(string promoCode)
        {
            var promoCodeOffer = await _servicesInstanceProvider.GetOffersServiceInstance().IsPromoCodeExist(promoCode);

            if (promoCodeOffer is not null)
            {
                var userCart = await this.GetUserCart();
                var userID = _userService.GetUserId();
                var oldPrice = userCart.TotalPrice;

                var userOrder = await _servicesInstanceProvider.GetOrdersServiceInstance().GetUserCurrentOrder(userID);

                if (userOrder is null)
                {
                    var items = userCart.Carts
                        .Select(i => new OrderDetails
                        {
                            ItemID = i.ItemId,
                            ItemType = i.ItemType,
                            Price = i.Price,
                            Quantity = i.Quantity,
                        }).ToList();

                    var order = await _servicesInstanceProvider.GetOrdersServiceInstance().CreateOrder(userID, items);

                    order.PromoCodeDiscountType = promoCodeOffer.PromoDiscountType;
                    order.PromoCodeDiscountValue = promoCodeOffer.PromoDiscountValue ?? 0;

                    await _unitOfWork.Commit();
                }
                else
                {
                    userOrder.PromoCodeDiscountType = promoCodeOffer.PromoDiscountType;
                    userOrder.PromoCodeDiscountValue = promoCodeOffer.PromoDiscountValue ?? 0;

                    await _unitOfWork.Commit();
                }

                if (promoCodeOffer.PromoDiscountType == "fixed")
                {
                    userCart.TotalPrice -= promoCodeOffer.PromoDiscountValue ?? 0;
                }
                else
                {
                    userCart.TotalPrice *= 1 - (promoCodeOffer.PromoDiscountValue ?? 0) / 100.0;
                }

                userCart.Carts = userCart.Carts.ToPagedList(1, 10);

                var promoSign = promoCodeOffer.PromoDiscountType == "fixed" ? "💲" : "%";

                var promoMessage = $"Promo code applied: {promoCodeOffer.PromoDiscountValue}{promoSign} discount!";

                return new ApplyPromoCodeResultDTO()
                {
                    Success = true,
                    Cart = userCart,
                    PromoMessage = promoMessage,
                    OldPrice = oldPrice
                };
            }
            else
            {
                return new ApplyPromoCodeResultDTO()
                {
                    Success = false,
                    Error = "Invalid promo code."
                };
            }
        }
    }
}