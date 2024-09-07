namespace EcommerceAPI.Shared.Helpers
{
    public enum Policies
    {
        //Admin Users
        EditAdmins = 0,
        //Product
        GetProducts = 1,
        CreateProduct = 2,
        UpdateProduct = 3,
        DeleteProduct = 4,
        //Category
        EditCategories = 5,
        //Customer
        EditCustomers = 6,
        //CustomerCommissions
        EditCustomerCommissions = 7,
        //Order
        GetOrders = 8,
        UpdateOrder = 9,
        UpdateOrder_AcceptItem = 9001,
        UpdateOrder_RejectItem = 9002,
        UpdateOrder_EditItem = 9003,
        UpdateOrder_ShipItem = 9004,
        UpdateOrder_ShowPrices = 9005,
        UpdateOrder_ShowItems = 9006,
        UpdateOrder_ClickLinks = 9007,
        UpdateOrder_GenerateLink = 9008,
        UpdateOrder_ArchiveOrders = 9009,
        UpdateOrder_TogglePayment = 9010,
        UpdateOrder_GetLogs = 9011,
        UpdateOrder_EditRecipientInfo = 9012,
        UpdateOrder_EditDeliveryPrice = 9013,
        UpdateOrder_SpecialDiscount = 9014,
        UpdateOrder_ShowInvoice = 9015,
        UpdateOrder_CaptureInvoice = 9016,

        DeleteOrder = 10,
        //Brand
        EditBrands = 11,
        //Coupons
        EditCoupons = 12,
        //BaseItem
        EditBaseItem = 13,
        EditScraping = 14,

        //ExportData
        ExportData = 15,

        //Notifications
        EditNotifications = 17,
        //AppMainScreenViewItem
        EditAppMainScreenViewItem = 18,


        //Currency
        EditCurrencyRate = 22,
        //Collections
        EditCollections = 24,
        //Country
        EditCountries = 25,
        //EmailPromotion
        EmailPromotion = 26,

        //CustomerCarts
        EditCustomerCarts = 27,
        EditCustomerCarts_CreateOrder = 2701,
        EditCustomerCarts_DeleteItem = 2702,
        EditCustomerCarts_ShowPrices = 2703,
        EditCustomerCarts_ClickLinks = 2704,
        EditCustomerCarts_ShowItems = 2705,

        //Cleaning Database
        CleaningDatabase = 100,

        //Memberships
        EditMemberships = 28,

        //Shipments
        EditShipments = 29,

        //Rating
        EditRatings = 31,

        //Frontend Setting
        EditFrontendSettings = 32,

        //Statistics
        Statistics = 33,

        //Shipping Companies
        EditShippingCompanies = 34,

        //PricingDiscount
        PricingDiscount = 99,

        //Permissions
        AdminPermissions = 35,

        DeleteAdmin = 36,

        UsersPage = 37,
        BlockUnBlockUsers = 38,
    }

    public static class PoliciesText
    {
        //Admin Users
        public const string EditAdmins = "0";
        //Product
        public const string GetProducts = "1";
        public const string CreateProduct = "2";
        public const string UpdateProduct = "3";
        public const string DeleteProduct = "4";
        //Category
        public const string EditCategories = "5";
        //Customer
        public const string EditCustomers = "6";
        //CustomerCommissions
        public const string EditCustomerCommissions = "7";
        //Order
        public const string GetOrders = "8";
        public const string UpdateOrder = "9";
        public const string UpdateOrder_AcceptItem = "9001";
        public const string UpdateOrder_RejectItem = "9002";
        public const string UpdateOrder_EditItem = "9003";
        public const string UpdateOrder_ShipItem = "9004";
        public const string UpdateOrder_ShowPrices = "9005";
        public const string UpdateOrder_ShowItems = "9006";
        public const string UpdateOrder_ClickLinks = "9007";
        public const string UpdateOrder_GenerateLink = "9008";
        public const string UpdateOrder_ArchiveOrders = "9009";
        public const string UpdateOrder_TogglePayment = "9010";
        public const string UpdateOrder_GetLogs = "9011";
        public const string UpdateOrder_EditRecipientInfo = "9012";
        public const string UpdateOrder_EditDeliveryPrice = "9013";
        public const string UpdateOrder_SpecialDiscount = "9014";
        public const string UpdateOrder_ShowInvoice = "9015";
        public const string UpdateOrder_CaptureInvoice = "9016";
        public const string DeleteOrder = "10";
        //Brand
        public const string EditBrands = "11";
        //Coupons
        public const string EditCoupons = "12";
        //BaseItem
        public const string EditBaseItem = "13";
        //Scraping
        public const string EditScraping = "14";
        //ExportData
        public const string ExportData = "15";


        //Notifications
        public const string EditNotifications = "17";
        //AppMainScreenViewItem
        public const string EditAppMainScreenViewItem = "18";

        //Currency
        public const string EditCurrencyRate = "22";
        //Collections
        public const string EditCollections = "24";
        //Country
        public const string EditCountries = "25";
        //EmailPromotion
        public const string EmailPromotion = "26";
        //CustomerCarts
        public const string EditCustomerCarts = "27";
        public const string EditCustomerCarts_CreateOrder = "2701";
        public const string EditCustomerCarts_DeleteItem = "2702";
        public const string EditCustomerCarts_ShowPrices = "2703";
        public const string EditCustomerCarts_ClickLinks = "2704";
        public const string EditCustomerCarts_ShowItems = "2705";

        //Cleaning Database
        public const string CleaningDatabase = "100";

        //Memberships
        public const string EditMemberships = "28";

        //Shipments
        public const string EditShipments = "29";

        //Rating
        public const string EditRatings = "31";

        //Frontend Setting
        public const string EditFrontendSettings = "32";

        //Statistics
        public const string Statistics = "33";

        //Shipping Companies
        public const string EditShippingCompanies = "34";

        //PricingDiscount
        public const string PricingDiscount = "99";

        //Permissions
        public const string AdminPermissions = "35";

        public const string DeleteAdmin = "36";

        public const string UsersPage = "37";
        public const string BlockUnBlockUsers = "37";
    }
}
