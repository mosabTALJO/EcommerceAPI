using EcommerceAPI.Data.Helpers;

namespace EcommerceAPI.Data.Models
{
    public class OrderPayment : TrackableEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string PaymentId { get; set; }
        public double Amount { get; set; }
        //public OrderPaymentType OrderPaymentType { get; set; }
        public int InvoiceId { get; set; }
        //public string UserDefinedField { get; set; }
        //public string CustomerReference { get; set; }
        public string InvoiceStatus { get; set; } //Pending, Paid, Canceled
        public string InvoiceURL { get; set; }
        //public InvoiceTransaction[] InvoiceTransactions { get; set; }
    }
}
