using DomainLayer.Interfaces;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using Souq.Models.Cart_Orders;
using X.PagedList.Extensions;

namespace InfrastructureLayer.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly IOrdersRepository orders;
        private readonly IOffersRepository offers;
        private readonly IItemsRepository items;

        public CartRepository(AppDbContext context, IUserService userService, IOrdersRepository orders, IOffersRepository offers, IItemsRepository items)
        {
            _context = context;
            _userService = userService;
            this.orders = orders;
            this.offers = offers;
            this.items = items;
        }

        public async Task<int> Add(int itemID, string itemType, int? qty)
        {
            Item? item = itemType switch
            {
                "AirConditioners" => _context.AirConditioners.Find(itemID),
                "Fridges" => _context.Fridges.Find(itemID),
                "Cookers" => _context.Cookers.Find(itemID),
                "WashingMachines" => _context.WashingMachines.Find(itemID),
                "Laptops" => _context.Laptops.Find(itemID),
                "HeadPhones" => _context.HeadPhones.Find(itemID),
                "TVs" => _context.TVs.Find(itemID),
                "MobilePhones" => _context.MobilePhones.Find(itemID),
                "VideoGames" => _context.VideoGames.Find(itemID),
                _ => throw new ArgumentException()
            };

            Item getItem = null;

            if (item.IsBOGOBuy)
            {
                getItem = await offers.GetBOGOGetItem(item);
            }

            var userID = await _userService.GetUserId();

            if (string.IsNullOrEmpty(userID))
                throw new InvalidOperationException();

            var userCart = await _context.ShoppingCarts.AsNoTracking().Where(sh => sh.UserId == userID).FirstOrDefaultAsync();

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

                    await _context.CartDetails.AddAsync(itemCartDetails);
                    await _context.SaveChangesAsync();

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

                        await _context.CartDetails.AddAsync(getItemCartDetails);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    var userCartDetails = _context.CartDetails
                        .Where(cd => cd.ShoppingCartID == userCart.Id && cd.ItemID == itemID && cd.ItemType == itemType).FirstOrDefault();

                    if (userCartDetails is not null)
                    {
                        userCartDetails.Quantity += 1;
                        item.Amount -= 1;

                        await _context.SaveChangesAsync();

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

                    await _context.CartDetails.AddAsync(cartDetails);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var transaction = _context.Database.BeginTransaction();

                try
                {
                    var userShoppingCart = new ShoppingCart()
                    {
                        UserId = userID
                    };
                    _context.ShoppingCarts.Add(userShoppingCart);
                    await _context.SaveChangesAsync();

                    var cartDetails = new CartDetails()
                    {
                        ItemID = item.ID,
                        ItemType = itemType,
                        Price = item.NewPrice.HasValue ? item.NewPrice ?? 0 : item.Price,
                        Quantity = 1,
                        ShoppingCartID = userShoppingCart.Id
                    };

                    item.Amount -= 1;

                    await _context.CartDetails.AddAsync(cartDetails);
                    await _context.SaveChangesAsync();

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

                        await _context.CartDetails.AddAsync(getItemCartDetails);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                }
            }

            return await TotalItemsInCart();
        }

        public async Task<int> Remove(int itemID, string itemType)
        {
            Item? item = itemType switch
            {
                "AirConditioners" => _context.AirConditioners.Find(itemID),
                "Fridges" => _context.Fridges.Find(itemID),
                "Cookers" => _context.Cookers.Find(itemID),
                "WashingMachines" => _context.WashingMachines.Find(itemID),
                "Laptops" => _context.Laptops.Find(itemID),
                "HeadPhones" => _context.HeadPhones.Find(itemID),
                "TVs" => _context.TVs.Find(itemID),
                "MobilePhones" => _context.MobilePhones.Find(itemID),
                "VideoGames" => _context.VideoGames.Find(itemID),
                _ => throw new ArgumentException()
            };

            var userID = await _userService.GetUserId();

            if (string.IsNullOrEmpty(userID))
                throw new InvalidOperationException();

            var userCart = await _context.ShoppingCarts.AsNoTracking()
                .Where(sh => sh.UserId == userID).FirstOrDefaultAsync();

            if (userCart is not null)
            {
                var userCartDetails = await _context.CartDetails
                       .Where(cd => cd.ShoppingCartID == userCart.Id && cd.ItemID == itemID && cd.ItemType == itemType).FirstOrDefaultAsync();

                if (userCartDetails.Quantity > 1)
                {
                    userCartDetails.Quantity -= 1;
                    item.Amount += 1;

                    await _context.SaveChangesAsync();
                }
                else if (userCartDetails.Quantity == 1)
                {
                    Item getItem = null;

                    if (item.IsBOGOBuy)
                    {
                        getItem = await offers.GetBOGOGetItem(item);

                        var getItemCartDetails = await _context.CartDetails
                            .Where(cd => cd.ItemID == getItem.ID && cd.Price == 0 && cd.ShoppingCartID == userCart.Id).FirstOrDefaultAsync();

                        getItem.Amount += 1;

                        _context.CartDetails.Remove(getItemCartDetails);
                        await _context.SaveChangesAsync();
                    }

                    item.Amount += 1;

                    _context.CartDetails.Remove(userCartDetails);
                    await _context.SaveChangesAsync();
                }

                return await TotalItemsInCart();
            }

            throw new InvalidOperationException();
        }

        public async Task<CartViewModel> GetUserCart()
        {
            var userID = await _userService.GetUserId();

            var userCart = await _context.ShoppingCarts.AsNoTracking().AsSplitQuery()
                .Include(cd => cd.CartDetails)
                .Where(sh => sh.UserId == userID).FirstOrDefaultAsync();

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
                                var airConditioner = _context.AirConditioners.AsNoTracking().FirstOrDefault(a => a.ID == cd.ItemID);
                                name = airConditioner?.Name;
                                imageSrc = airConditioner?.imageSrc;
                                price = airConditioner.NewPrice.HasValue ? airConditioner?.NewPrice : airConditioner?.Price;
                                break;

                            case "Fridges":
                                var fridge = _context.Fridges.AsNoTracking().FirstOrDefault(f => f.ID == cd.ItemID);
                                name = fridge?.Name;
                                imageSrc = fridge?.imageSrc;
                                price = fridge.NewPrice.HasValue ? fridge?.NewPrice : fridge?.Price;
                                break;

                            case "Cookers":
                                var cooker = _context.Cookers.AsNoTracking().FirstOrDefault(c => c.ID == cd.ItemID);
                                name = cooker?.Name;
                                imageSrc = cooker?.imageSrc;
                                price = cooker.NewPrice.HasValue ? cooker?.NewPrice : cooker?.Price;
                                break;

                            case "WashingMachines":
                                var washingMachine = _context.WashingMachines.AsNoTracking().FirstOrDefault(wm => wm.ID == cd.ItemID);
                                name = washingMachine?.Name;
                                imageSrc = washingMachine?.imageSrc;
                                price = washingMachine.NewPrice.HasValue ? washingMachine?.NewPrice : washingMachine?.Price;
                                break;

                            case "Laptops":
                                var laptop = _context.Laptops.AsNoTracking().FirstOrDefault(l => l.ID == cd.ItemID);
                                name = laptop?.Name;
                                imageSrc = laptop?.imageSrc;
                                price = laptop.NewPrice.HasValue ? laptop?.NewPrice : laptop?.Price;
                                break;

                            case "TVs":
                                var tv = _context.TVs.AsNoTracking().FirstOrDefault(t => t.ID == cd.ItemID);
                                name = tv?.Name;
                                imageSrc = tv?.imageSrc;
                                price = tv.NewPrice.HasValue ? tv?.NewPrice : tv?.Price;
                                break;

                            case "HeadPhones":
                                var headPhone = _context.HeadPhones.AsNoTracking().FirstOrDefault(hp => hp.ID == cd.ItemID);
                                name = headPhone?.Name;
                                imageSrc = headPhone?.imageSrc;
                                price = headPhone.NewPrice.HasValue ? headPhone?.NewPrice : headPhone?.Price;
                                break;

                            case "MobilePhones":
                                var mobilePhone = _context.MobilePhones.AsNoTracking().FirstOrDefault(mp => mp.ID == cd.ItemID);
                                name = mobilePhone?.Name;
                                imageSrc = mobilePhone?.imageSrc;
                                price = mobilePhone.NewPrice.HasValue ? mobilePhone?.NewPrice : mobilePhone?.Price;
                                break;

                            case "VideoGames":
                                var videoGame = _context.VideoGames.AsNoTracking().FirstOrDefault(vg => vg.ID == cd.ItemID);
                                name = videoGame?.Name;
                                imageSrc = videoGame?.imageSrc;
                                price = videoGame.NewPrice.HasValue ? videoGame?.NewPrice : videoGame?.Price;
                                break;

                            default:
                                var getItem = items.FindItemByID(cd.ItemID).Result;
                                name = getItem?.Name;
                                imageSrc = getItem?.imageSrc;
                                price = 0;
                                break;
                        }

                        return new RepositoryCartVM
                        {
                            ItemId = cd.ItemID,
                            ItemType = cd.ItemType,
                            Name = name,
                            Price = price ?? 0.0,
                            imageSrc = imageSrc,
                            Quantity = cd.Quantity
                        };
                    });

                var cart = new CartViewModel()
                {
                    Carts = cartItems.ToPagedList() ?? Enumerable.Empty<RepositoryCartVM>().ToPagedList(),
                    TotalPrice = cartItems.Sum(c => c.Price * c.Quantity)
                };

                return cart;
            }

            var userShoppingCart = new ShoppingCart()
            {
                UserId = userID
            };
            await _context.ShoppingCarts.AddAsync(userShoppingCart);
            await _context.SaveChangesAsync();

            return null;
        }

        public async Task<int> TotalItemsInCart()
        {
            var userID = await _userService.GetUserId();

            if (string.IsNullOrEmpty(userID))
                throw new InvalidOperationException();

            var userCart = await _context.ShoppingCarts.AsNoTracking().Where(sh => sh.UserId == userID).FirstOrDefaultAsync();

            if (userCart is not null)
            {
                var totalCartCount = await _context.CartDetails.AsNoTracking().Where(cd => cd.ShoppingCart.UserId == userID).SumAsync(cd => cd.Quantity);

                return totalCartCount;
            }

            return 0;
        }

        public async Task<int> TotalItemQuantityInCart(int itemID, string itemType)
        {
            var userID = await _userService.GetUserId();

            var userCart = await _context.ShoppingCarts.AsNoTracking().AsSplitQuery()
                .Include(cd => cd.CartDetails)
                .Where(sh => sh.UserId == userID).FirstOrDefaultAsync();

            if (userCart is not null)
            {
                var totalQuantity = await _context.CartDetails.AsNoTracking()
                    .Where(cd => cd.ItemID == itemID && cd.ShoppingCart.UserId == userID && cd.ItemType == itemType)
                    .Select(cd => cd.Quantity).FirstOrDefaultAsync();

                return totalQuantity;
            }
            throw new InvalidOperationException();
        }

        public async Task EmptyCart()
        {
            var userID = await _userService.GetUserId();

            var userCart = await _context.ShoppingCarts.AsNoTracking().AsSplitQuery()
                .Include(cd => cd.CartDetails)
                .Where(sh => sh.UserId == userID).FirstOrDefaultAsync();

            if (userCart is not null)
            {
                var cartDetails = await _context.CartDetails.AsNoTracking().Where(cd => cd.ShoppingCartID == userCart.Id).ToListAsync();

                if (cartDetails.Any())
                {
                    _context.RemoveRange(cartDetails);

                    return;
                }

                throw new InvalidOperationException();
            }
            throw new InvalidOperationException();
        }

        public async Task<CartViewModel> ApplyPromoCode(CartViewModel cart, Offer promoCodeOffer)
        {
            var userOrder = await orders.GetUserCurrentOrder();

            if (userOrder is null)
            {
                var items = cart.Carts
                    .Select(i => new OrderDetails
                    {
                        ItemID = i.ItemId,
                        ItemType = i.ItemType,
                        Price = i.Price,
                        Quantity = i.Quantity,
                    }).ToList();

                var order = await orders.CreateOrder(items);

                order.PromoCodeDiscountType = promoCodeOffer.PromoDiscountType;
                order.PromoCodeDiscountValue = promoCodeOffer.PromoDiscountValue ?? 0;

                await _context.SaveChangesAsync();
            }
            else
            {
                userOrder.PromoCodeDiscountType = promoCodeOffer.PromoDiscountType;
                userOrder.PromoCodeDiscountValue = promoCodeOffer.PromoDiscountValue ?? 0;

                await _context.SaveChangesAsync();
            }

            if (promoCodeOffer.PromoDiscountType == "fixed")
            {
                cart.TotalPrice = promoCodeOffer.PromoDiscountValue ?? 0;
            }
            else
            {
                cart.TotalPrice *= 1 - (promoCodeOffer.PromoDiscountValue ?? 0) / 100.0;
            }

            return cart;
        }
    }
}