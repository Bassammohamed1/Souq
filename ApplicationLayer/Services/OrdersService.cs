using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Enums;
using DomainLayer.Interfaces;
using Souq.Models.Cart_Orders;

namespace ApplicationLayer.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsersService _userService;
        private readonly IServicesInstanceProvider _servicesInstanceProvider;

        public OrdersService(IUnitOfWork unitOfWork, IUsersService userService, IServicesInstanceProvider servicesInstanceProvider)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _servicesInstanceProvider = servicesInstanceProvider;
        }

        public async Task<IEnumerable<Order>> AllOrders()
        {
            return await _unitOfWork.Orders.GetAllOrders();
        }

        public async Task<OrdersDTO> AllOrders(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var allOrders = await _unitOfWork.Orders.GetAllOrders();

            var totalPages = (int)Math.Ceiling(allOrders.Count() / (double)pageSize);

            var orders = (await _unitOfWork.Orders.GetAllOrders(pageNumber, pageSize)).ToList()
                   .Select(o =>
                   {
                       var totalPrice = o.OrderDetails.Sum(od => od.Price * od.Quantity);

                       if (o.PromoCodeDiscountType == "fixed")
                       {
                           o.TotalPrice = totalPrice - o.PromoCodeDiscountValue ?? 0;
                       }
                       else
                       {
                           o.TotalPrice = totalPrice * (1 - (o.PromoCodeDiscountValue ?? 0) / 100.0);
                       }

                       return new OrderDTO()
                       {
                           Id = o.ID,
                           UserName = o.User.UserName,
                           TotalPrice = o.TotalPrice,
                           CreatedAt = o.CreatedAt.ToString("g"),
                           PaymentMethod = o.PaymentMethod,
                           Status = o.Status
                       };
                   }).ToList();

            return new OrdersDTO()
            {
                Orders = orders,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            };
        }

        public OrdersDTO UserOrders(int? page, string userID)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var allOrders = _unitOfWork.Orders.GetUserOrders(1, int.MaxValue, userID);

            var totalPages = (int)Math.Ceiling(allOrders.Count() / (double)pageSize);

            var userOrders = _unitOfWork.Orders.GetUserOrders(pageNumber, pageSize, userID).ToList()
             .Select(o =>
             {
                 var totalPrice = o.OrderDetails.Sum(od => od.Price * od.Quantity);

                 if (o.PromoCodeDiscountType == "fixed")
                 {
                     o.TotalPrice = totalPrice - o.PromoCodeDiscountValue ?? 0;
                 }
                 else
                 {
                     o.TotalPrice = totalPrice * (1 - (o.PromoCodeDiscountValue ?? 0) / 100.0);
                 }

                 return new OrderDTO()
                 {
                     Id = o.ID,
                     UserName = o.User.UserName,
                     TotalPrice = o.TotalPrice,
                     CreatedAt = o.CreatedAt.ToString("g"),
                     PaymentMethod = o.PaymentMethod,
                     Status = o.Status
                 };
             }).ToList();

            return new OrdersDTO()
            {
                Orders = userOrders,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            };
        }

        public async Task<Order> CreateOrder(string userID, IEnumerable<OrderDetails> data)
        {
            if (!string.IsNullOrEmpty(userID))
            {
                var previousUserOrder = await _unitOfWork.Orders.GetUserPendingOrder(userID);

                if (previousUserOrder is not null)
                {
                    _unitOfWork.Orders.Delete(previousUserOrder);
                    await _unitOfWork.Commit();
                }

                var userOrder = new Order()
                {
                    UserID = userID,
                    CreatedAt = DateTime.UtcNow,
                    Status = OrderStatus.Pending
                };
                await _unitOfWork.Orders.Add(userOrder);
                await _unitOfWork.Commit();

                if (data is not null)
                {
                    foreach (var order in data)
                    {
                        order.OrderID = userOrder.ID;
                        await _unitOfWork.Orders.AddOrderDetails(order);
                        await _unitOfWork.Commit();
                    }
                }

                return userOrder;
            }
            throw new InvalidOperationException();
        }

        public async Task SetOrderPaymentMethodAndStatus(int orderID, string paymentMethod, int status)
        {
            var userID = _userService.GetUserId();

            var userOrder = await _unitOfWork.Orders.GetUserPendingOrder(userID);

            if (userOrder is not null)
            {
                userOrder.PaymentMethod = paymentMethod;
                userOrder.Status = (OrderStatus)status;
            }
            else
                throw new InvalidOperationException();
        }

        public async Task<Order> GetUserCurrentOrder(string userID)
        {
            var userOrder = await _unitOfWork.Orders.GetUserPendingOrderWithDetails(userID);

            return userOrder is not null ? userOrder : null;
        }

        public async Task<Order> GetUserCurrentOrderOrCreateIt(string userID)
        {
            var order = await _unitOfWork.Orders.GetUserPendingOrderWithDetails(userID);

            if (order == null)
            {
                var userCartItems = await _servicesInstanceProvider.GetCartServiceInstance().GetCartItems();

                var items = userCartItems
                    .Select(i => new OrderDetails
                    {
                        ItemID = i.ItemId,
                        ItemType = i.ItemType,
                        Price = i.Price,
                        Quantity = i.Quantity,
                    });

                var userOrder = await this.CreateOrder(userID, items);

                return userOrder;
            }
            return order;
        }
    }
}