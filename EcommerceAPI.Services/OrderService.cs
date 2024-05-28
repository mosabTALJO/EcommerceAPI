using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public interface IOrderService : IService<Order>
    {
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
    }

    public class OrderService : Service<Order>, IOrderService
    {
        ITrackableRepository<OrderItem> _orderItemRepo;
        ITrackableRepository<Product> _productRepo;
        ITrackableRepository<ProductType> _productTypeRepo;
        public OrderService(ITrackableRepository<Order> repo, ITrackableRepository<OrderItem> orderItemRepo,
            ITrackableRepository<ProductType> productTypeRepo, ITrackableRepository<Product> productRepo) : base(repo)
        {
            _orderItemRepo = orderItemRepo;
            _productRepo = productRepo;
            _productTypeRepo = productTypeRepo;
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
            var p = _productRepo.Queryable().Include(x => x.ProductTypes).IgnoreQueryFilters().FirstOrDefault(x => x.Id == orderItem.ProductId);
            if (p is null)
                throw new Exception("Product Not Found");
            if (orderItem.ProductTypeId.HasValue)
            {
                var pt = p.ProductTypes.FirstOrDefault(x => x.Id == orderItem.ProductTypeId);
                if (pt is null)
                    throw new Exception("Product Type Not Found");
                if (previousState == OrderItemState.Pending)
                    pt.StockLevel -= orderItem.Qty;
                _productTypeRepo.Update(pt);

                if (p.ProductTypes.All(x => x.StockLevel <= 0))
                {
                    p.IsActive = false;
                    _productRepo.Update(p);
                }
            }

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
                .Include(x => x.OrderItems).ThenInclude(x => x.Product).ThenInclude(x => x.Brand)
                .Include(x => x.OrderItems).ThenInclude(x => x.ProductType)
                .Include(x => x.OrderItems).ThenInclude(x => x.Logs)
                .Where(x => !x.DeletedAt.HasValue).FirstOrDefaultAsync(x => x.Id == id);

            if (o?.OrderItems?.Any() == true)
            {
                foreach (var oi in o.OrderItems)
                    CleanImages(oi.Product);
            }

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

        private void CleanImages(Product p)
        {
            var allImages = p.Images?.Split(',');
            //if (p.VendorId == Vendor.Custom)
            //{
            //    p.MainImage = $"https://api.trgatestore.com/api/Files/PreviewImage/{p.MainImage}?w=300&h=400";
            //    return;
            //}

            if (string.IsNullOrWhiteSpace(p.MainImage))
            {
                if (allImages?.Length > 0)
                {
                    for (int i = 0; i < (allImages?.Length ?? 0); i++)
                        allImages[i] = allImages[i].StartsWith("https") ? allImages[i] : $"https://cdn.dsmcdn.com/mnresize/{300}/{400}" + allImages[i];
                    p.Images = string.Join(",", allImages);
                    p.MainImage = allImages[0];
                }
            }
            else
            {
                p.MainImage = p.MainImage.StartsWith("https") ? p.MainImage : $"https://cdn.dsmcdn.com/mnresize/{1200}/{1000}" + p.MainImage;
            }
        }
    }
}
