using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Services
{
    public interface IOrderLogService : IService<OrderLog>
    {
        Task OrderCreatedLog(int orderId, string UserId);
        Task OrderOnDoorPaymentPendingLog(int orderId, string UserId);
        Task OrderPaiedLog(int orderId, string UserId);
        Task OrderCompletedLog(int orderId, string UserId);
        Task OrderUpdatedLog(Order order, string UserId, string? notes = null);
        Task ItemCreatedLog(int itemId, Order order, string UserId);
        Task ItemAcceptedLog(int itemId, Order order, string UserId);
        Task ItemShippedLog(int itemId, Order order, string UserId);
        Task ItemUpdatedOrEditedLog(OrderItem item, string UserId, string notes);
        Task ItemRejectedLog(int itemId, Order order, string UserId);
        Task ItemCancelledLog(int itemId, Order order, string UserId);
        Task ItemReturningLog(int itemId, Order order, string UserId);
    }
    public class OrderLogService : Service<OrderLog>, IOrderLogService
    {
        private readonly UserManager<AppUser> _userManager;
        public OrderLogService(IRepository<OrderLog> repo, UserManager<AppUser> userManager) : base(repo)
        {
            _userManager = userManager;
        }
        //orders
        public async Task OrderCreatedLog(int orderId, string UserId)
        {
            var u = await _userManager.FindByIdAsync(UserId);

            Insert(new OrderLog
            {
                OrderId = orderId,
                OrderStatus = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = u.Id,
                Username = u.DisplayName = ", " + u.Email,
                UserType = u.UserType,
            });
        }
        public async Task OrderOnDoorPaymentPendingLog(int orderId, string UserId)
        {
            var u = await _userManager.FindByIdAsync(UserId);

            Insert(new OrderLog
            {
                OrderId = orderId,
                OrderStatus = OrderStatus.OnDoorPaymentPending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = u.Id,
                Username = u.DisplayName = ", " + u.Email,
                UserType = u.UserType,
            });
        }
        public async Task OrderPaiedLog(int orderId, string UserId)
        {
            var u = await _userManager.FindByIdAsync(UserId);

            Insert(new OrderLog
            {
                OrderId = orderId,
                OrderStatus = OrderStatus.Paid,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = u.Id,
                Username = u.DisplayName = ", " + u.Email,
                UserType = u.UserType,
            });
        }
        public async Task OrderCompletedLog(int orderId, string UserId)
        {
            var u = await _userManager.FindByIdAsync(UserId);

            Insert(new OrderLog
            {
                OrderId = orderId,
                OrderStatus = OrderStatus.Completed,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = u.Id,
                Username = u.DisplayName = ", " + u.Email,
                UserType = u.UserType,
            });
        }
        public async Task OrderUpdatedLog(Order order, string UserId, string notes = null)
        {
            var u = await _userManager.FindByIdAsync(UserId);

            Insert(new OrderLog
            {
                OrderId = order.Id,
                OrderStatus = order.OrderStatus,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = u.Id,
                Username = u.DisplayName = ", " + u.Email,
                UserType = u.UserType,
                Notes = notes
            });
        }

        //items
        public async Task ItemCreatedLog(int itemId, Order order, string UserId)
        {
            var u = await _userManager.FindByIdAsync(UserId);

            Insert(new OrderLog
            {
                OrderItemId = itemId,
                OrderId = order.Id,
                OrderStatus = order.OrderStatus,
                OrderItemState = OrderItemState.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = u.Id,
                Username = u.DisplayName = ", " + u.Email,
                UserType = u.UserType,
            });
        }
        public async Task ItemAcceptedLog(int itemId, Order order, string UserId)
        {
            var u = await _userManager.FindByIdAsync(UserId);

            Insert(new OrderLog
            {
                OrderItemId = itemId,
                OrderId = order.Id,
                OrderStatus = order.OrderStatus,
                OrderItemState = OrderItemState.Accepted,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = u.Id,
                Username = u.DisplayName = ", " + u.Email,
                UserType = u.UserType,
            });
        }
        public async Task ItemShippedLog(int itemId, Order order, string UserId)
        {
            var u = await _userManager.FindByIdAsync(UserId);

            Insert(new OrderLog
            {
                OrderItemId = itemId,
                OrderId = order.Id,
                OrderStatus = order.OrderStatus,
                OrderItemState = OrderItemState.Shipped,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = u.Id,
                Username = u.DisplayName = ", " + u.Email,
                UserType = u.UserType,
            });
        }
        public async Task ItemCancelledLog(int itemId, Order order, string UserId)
        {
            var u = await _userManager.FindByIdAsync(UserId);

            Insert(new OrderLog
            {
                OrderItemId = itemId,
                OrderItemState = OrderItemState.Cancelled,
                OrderId = order.Id,
                OrderStatus = order.OrderStatus,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = u.Id,
                Username = u.DisplayName = ", " + u.Email,
                UserType = u.UserType,
                Notes = "",
            });
        }
        public async Task ItemUpdatedOrEditedLog(OrderItem item, string UserId, string notes)
        {
            var u = await _userManager.FindByIdAsync(UserId);

            Insert(new OrderLog
            {
                OrderItemId = item.Id,
                OrderItemState = item.OrderItemState,
                OrderId = item.OrderId,
                OrderStatus = item.Order.OrderStatus,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = u.Id,
                Username = u.DisplayName = ", " + u.Email,
                UserType = u.UserType,
                Notes = notes
            });
        }
        public async Task ItemRejectedLog(int itemId, Order order, string UserId)
        {
            var u = await _userManager.FindByIdAsync(UserId);

            Insert(new OrderLog
            {
                OrderItemId = itemId,
                OrderId = order.Id,
                OrderStatus = order.OrderStatus,
                OrderItemState = OrderItemState.Rejected,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = u.Id,
                Username = u.DisplayName = ", " + u.Email,
                UserType = u.UserType,
            });
        }
        public async Task ItemReturningLog(int itemId, Order order, string UserId)
        {
            var u = await _userManager.FindByIdAsync(UserId);

            Insert(new OrderLog
            {
                OrderItemId = itemId,
                OrderId = order.Id,
                OrderStatus = order.OrderStatus,
                OrderItemState = OrderItemState.Returning,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = u.Id,
                Username = u.DisplayName = ", " + u.Email,
                UserType = u.UserType,
            });
        }
    }
}
