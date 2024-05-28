namespace EcommerceAPI.Core.Orders
{
    public enum GetOrderStatus
    {
        All,
        Pending,
        Accepted,
        Rejected,
        Shipped,
        Delivered,
        Paid,
        ConfirmedForOnDoorPayment,
        Completed,
        Cart,
    }
}
