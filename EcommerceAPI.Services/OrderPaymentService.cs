using EcommerceAPI.Data.Models;
using EcommerceAPI.UnitOfWork.Abstractions;
using EcommerceAPI.UnitOfWork.Services;

namespace EcommerceAPI.Services
{
    public interface IOrderPaymentService : IService<OrderPayment>
    {
    }
    public class OrderPaymentService : Service<OrderPayment>, IOrderPaymentService
    {
        public OrderPaymentService(IRepository<OrderPayment> repo) : base(repo)
        {
        }
    }
}
