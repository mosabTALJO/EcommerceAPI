using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public interface IOrderService : IService<Order>
    {
        /// <summary>
        /// Get the Order with its details (Customer, Gift, OrderItems, ProductVariant, Product, Brand, Logs).
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order with its details</returns>
        Task<Order> GetOrderDetails(int id);
        void CreateOrder(Order o);
        void UpdateOrder(Order o, bool UpdateCreatedAt = false);
        void DeleteOrder(Order o);
        void ShipItem(OrderItem orderItem, string ShippingCompany, string ShippingCode);
        void Accept(OrderItem orderItem);
        void InStock(OrderItem orderItem);
        void Cancel(OrderItem oi);
        void Return(OrderItem oi, string reason);
        void DeleteOrderItem(OrderItem oi);
        IQueryable<OrderItem> ItemsQueryable();
    }

    public class OrderService : Service<Order>, IOrderService
    {
        ITrackableRepository<OrderItem> _orderItemRepo;
        ITrackableRepository<Product> _productRepo;
        //ITrackableRepository<ProductType> _productTypeRepo;
        ITrackableRepository<ProductVariant> _productVariantRepo;
        public OrderService(ITrackableRepository<Order> repo, ITrackableRepository<OrderItem> orderItemRepo,
            /*ITrackableRepository<ProductType> productTypeRepo*/ITrackableRepository<ProductVariant> productVariantRepo, ITrackableRepository<Product> productRepo) : base(repo)
        {
            _orderItemRepo = orderItemRepo;
            _productRepo = productRepo;
            _productVariantRepo = productVariantRepo;
            //_productTypeRepo = productTypeRepo;
        }
        public void CreateOrder(Order o)
        {
            Insert(o);
            foreach (var oi in o.OrderItems ?? new List<OrderItem>())
            {
                if (oi.Qty > 0)
                {
                    oi.Order = o;
                    _orderItemRepo.Insert(oi);
                }
            }
        }
        public void UpdateOrder(Order o, bool UpdateCreatedAt = false)
        {
            var dbItems = _orderItemRepo.Queryable().AsNoTracking().Where(x => x.OrderId == o.Id).ToList();
            foreach (var oi in o.OrderItems ?? new List<OrderItem>())
            {
                oi.Order = o;
                oi.OrderId = o.Id;
                if (oi.Id == 0)
                    _orderItemRepo.Insert(oi);
                else if (oi.Qty > 0)
                {
                    dbItems.Remove(dbItems.FirstOrDefault(x => x.Id == oi.Id));
                    _orderItemRepo.Update(oi);
                }
            }
            foreach (var item in dbItems)
            {
                item.CompleteRemove = true;
                _orderItemRepo.Delete(item);
            }

            if (UpdateCreatedAt)
            {
                o.CreatedAt = DateTime.Now;
                o.UpdateCreatedAt = true;
            }

            Update(o);
        }
        public void DeleteOrder(Order o)
        {
            _orderItemRepo.Queryable().Where(x => x.OrderId == o.Id).ToList().ForEach(pt => _orderItemRepo.Delete(pt));
            Delete(o);
        }
        public void ShipItem(OrderItem OrderItem, string ShippingCompany, string ShippingCode)
        {
            OrderItem.ShippingCode = ShippingCode;
            OrderItem.ShippingCompany = ShippingCompany;
            OrderItem.ShippingDate = DateTime.UtcNow;
            OrderItem.OrderItemState = OrderItemState.Shipped;

            _orderItemRepo.Update(OrderItem);
        }
        public void Accept(OrderItem orderItem)
        {
            var previousState = orderItem.OrderItemState;
            orderItem.AcceptedDate = DateTime.UtcNow;
            orderItem.OrderItemState = OrderItemState.Accepted;
            //var p = _productRepo.Queryable().Include(x => x.ProductTypes).IgnoreQueryFilters().FirstOrDefault(x => x.Id == orderItem.ProductId);
            var pv = _productVariantRepo.Queryable().Include(x => x.Product).IgnoreQueryFilters().FirstOrDefault(x => x.Id == orderItem.ProductVariantId)
                ?? throw new Exception("Product Not Found");

            if (previousState == OrderItemState.Pending)
                pv.StockLevel -= orderItem.Qty;
            _productVariantRepo.Update(pv);
            _orderItemRepo.Update(orderItem);
        }
        public async void Cancel(OrderItem OrderItem)
        {
            OrderItem.CancelDate = DateTime.UtcNow;
            OrderItem.OrderItemState = OrderItemState.Cancelled;
            _orderItemRepo.Update(OrderItem);

            //UpdateOrder(await Repository.FindAsync(OrderItem.OrderId));
        }
        public void Return(OrderItem OrderItem, string reason)
        {
            OrderItem.ReturningDate = DateTime.UtcNow;
            OrderItem.ReturnReason = reason;
            OrderItem.OrderItemState = OrderItemState.Returning;
            _orderItemRepo.Update(OrderItem);
        }
        public async Task<Order> GetOrderDetails(int id)
        {
            var o = await Repository.Queryable().IgnoreQueryFilters().AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Gift)
                .Include(x => x.OrderItems).ThenInclude(x => x.ProductVariant).ThenInclude(x => x.Product).ThenInclude(x => x.Brand)
                .Include(x => x.OrderItems).ThenInclude(x => x.Logs)
                .Where(x => !x.DeletedAt.HasValue).AsSplitQuery().FirstOrDefaultAsync(x => x.Id == id);

            return o;
        }
        public void DeleteOrderItem(OrderItem oi)
        {
            var dbOrderItem = _orderItemRepo.Queryable().FirstOrDefault(x => x.Id == oi.Id);
            if (dbOrderItem is null)
                return;

            dbOrderItem.CompleteRemove = true;
            _orderItemRepo.Delete(dbOrderItem);
        }
        public void InStock(OrderItem orderItem)
        {
            orderItem.InStockDate = DateTime.UtcNow;
            orderItem.OrderItemState = OrderItemState.InStock;
            _orderItemRepo.Update(orderItem);
        }
        public IQueryable<OrderItem> ItemsQueryable() => _orderItemRepo.Queryable();
    }
}