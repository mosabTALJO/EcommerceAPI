namespace EcommerceAPI.Shared.ViewModels
{
    public class OrderPaymentVM
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string PaymentId { get; set; }
        public double Amount { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceStatus { get; set; } //Pending, Paid, Canceled
        public string InvoiceURL { get; set; }
    }
}
