namespace EcommerceAPI.Shared.Helpers
{
    public class MyFatoorahHelper
    {
        //static string token = "5MFjwTlr-7C2i8342cb-yV0g-D59eERyOrd5WgOjYTsPCEFKhYiVRgzP-VyauSOV9kaHnpvIk-CC9wjE6Wbl2YhHLoDHFNptfTazv-jMG6v0xx2tUwdSn0OszrRI4r7h3PwSrwi917L3rFopIV_FbLFJgpsvRQZHOS88clu3Zovi_k5AighBu01Uz2V-AZB6HNyju48JGsRI0VmvsKCF3YG2aEeWbJgg5BQhu0fGjlpSYQlLjjJ4TjpBUwg8QPVoIzqpQ0MCoFkJrFxC7FuUa1pA6_aAAY1MjlIAyW-81XJPFyM8v_4Ojv6H32SCY82AZRANqqaJwkUO26nQY4c91cWunoyP64NidkuggYOdwEIhpojO3CEi6AHWRVPTSzAHOO8l1YW5reDW9N4dzQ4eWZgOtfIk8DXY6PFp12jWz-vLnBKWnSbt5eUeQdeJ7TRdXqKplHQ046g2Ai_nVUuG30VHE2iw1XXrO4ZwW9avAS1fwjsoqwCb9yT27wbSZjULerxaqpuKN0A_CJ7EW7gShTmaTOvhZMdus5xf4fNMI7uFALsPg1yn6gxP3mupnC6oU9W1jJa9W_JdxyIZG6_1Tn2EtsqtiuxcNQth6eBL2ExMB6Rcx_AtEUbnPgz2hxVNADHNNVor_rfw9qoDB7LsynLQooojfUS2V5hRXuNJwGNwr2NZ";
        //static string baseURL = "https://api.myfatoorah.com";

        static string token = "TyYRRpESN8ZxvuwuGm10Ot9O5K33EAlHjEpdKGN2VasqiS0Z4HkzpYfZ_0RikqWlWFiqAczFjJA7DEpyPvrcFOCiGaMV8AEOlaGSZV5h4R1I6hK21B78dhPfJCDj8MzJ22oa8jHE1bEAWhrQDiD7A03_XVg9t4YgIDz2cr_X2RoEp_rJq_y9sP4Nj2Sk3MV57Yv1feg_JTwf3yhtJhGU6PMIfiXLOA3kcrVmsDpIQPEKR2T7ebuIBxjn41uolMGiVA-_-e1h65bpkgAHuWLDh6H0H_CxJZ3zqmO1MqXvxklkpSDULfUqEwAe7O68CD4zyinQ7ToUOiF9ye150aQknlMNOCdHNAH9dbBUW3xu2XjBhRd8SPQ0cbyPfb039BvOf9PnTW4BJbQfS0d0Xb6eRd2lbrwMq_Jo6WgKCCgC4LjbaLLAKUfRDvFYCy4vrZVwI2osJ0g06ZSbSoRESPfmu9S9LCQPpGuPNAFAvlV0gF4FW2e8bjK7hIWSW4Ryzgl3Fv7yFLmEtRSM4nQGEKwXV2q7TD76ie1KYCeOw36rPWkHc5RSgPcdc2D_kA7HqcXlTPzPLvL67BB9SHkfCDbFvVabOEko-6uXsJNTMi4n6MET3HMSXjZF_6w8aILgxLzVjXzAtHV5gpst3Z0lnVlPeAJzUUhcKXd4GhQpnag7_fCMuhYVKDCW-ES07GGlZDtRfufFQg";
        static string baseURL = "https://api.myfatoorah.com";

        //static string token = "9RZFVY4vOzwoGKR5WdDR-G1rYLKebOPTak6g35QbmMsGAS6mQCMUo65T_ZuG8IoXgJDnPnCsq4Dmx0RgmezRVAWMv5IcMY_oJ08qsgmMg6RKozpBSpMJQ1eulF_W6qVw2jFux6EAMZUJegn9k6EhchVg16826BMYfTFKBIgz4byAiUh8bKCuS6JI47GvFyMCmN9AmBFit4vNxVzdsMPWdK08NnHabdbAU6O6aqTmTedG-H5FEzcnmRFl5wzurDgv1Bl-Um4zmEJptExQBlj68b7tHE6MOwOWNgBG73fzU3rjO7NCWsEgcf-678PtmmHwV06-742Ime2syPKjouulVHD7Sdp6ljgUxkm5FjKuw1XpXb3ov_16rLBjhdXxdvTxYDyKzrAHNajnzDKeU6AD7D9D1jafDZ1t1qDXs56Q5xcYH5Albw89WGKQ2hSRzzneQcFevq0jr7h3B7QdFkLowukvwgxr3IZMMAhU2ucR7otRc1m7fgtdisCw9z1lk7nM--nAM5BhX4p5hwHhXfcAKvCcQNutNPkdeFZ6CB584M-1aZwdnEl2ntdzDBm2_CTbceHDE9EUQwv5TGnWkcz-6mDnCUmLNNyJhmJjsB_pSHCNfyxbH8ftuo5yNdOSgcCuyX_rBt4brOcsITAzeb-pD_Z-qmszWQs1Dg2_xqAn4KkLzePd";
        //static string baseURL = "https://apitest.myfatoorah.com";

        public static async Task<ApiResponse<SendPaymentModel>> SendPayment(object o)
        {
            string url = baseURL + $"/v2/SendPayment";
            return await HttpHelper.Post<ApiResponse<SendPaymentModel>>(url, o, token);
        }
        public static async Task<ApiResponse<PaymentDataModel>> GetPaymentStatusByInvoiceId(string InvoiceId)
        {
            string url = baseURL + $"/v2/GetPaymentStatus";
            var GetPaymentStatusRequest = new
            {
                Key = InvoiceId,
                KeyType = "invoiceId"
            };
            return await HttpHelper.Post<ApiResponse<PaymentDataModel>>(url, GetPaymentStatusRequest, token);
        }
        public static async Task<ApiResponse<PaymentDataModel>> GetPaymentStatusByPaymentId(string PaymentId)
        {
            string url = baseURL + $"/v2/GetPaymentStatus";
            var GetPaymentStatusRequest = new
            {
                Key = PaymentId,
                KeyType = "paymentId"
            };
            return await HttpHelper.Post<ApiResponse<PaymentDataModel>>(url, GetPaymentStatusRequest, token);
        }

        public static async Task<ApiResponse<ApiResponse<object>>> UpdatePaymentStatus(string PaymentId, double Amount)
        {
            string url = baseURL + $"/v2/UpdatePaymentStatus";
            var UpdatePaymentStatusRequest = new
            {
                Operation = "capture",
                Amount,
                Key = PaymentId,
                KeyType = "PaymentId"
            };
            return await HttpHelper.Post<ApiResponse<ApiResponse<object>>>(url, UpdatePaymentStatusRequest, token);
        }
    }

    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
    public class SendPaymentModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceURL { get; set; }
        public string CustomerReference { get; set; }
        public string UserDefinedField { get; set; }
    }
    public class PaymentDataModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceStatus { get; set; } //Pending, Paid, Canceled
        public string UserDefinedField { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ExpiryDate { get; set; }
        public string ExpiryTime { get; set; }
        public string CustomerReference { get; set; }
        public float InvoiceValue { get; set; }
        public string InvoiceDisplayValue { get; set; }
        public float DueDeposit { get; set; }
        public string DepositStatus { get; set; }
        public InvoiceTransaction[] InvoiceTransactions { get; set; }
        public string InvoiceReference { get; set; }
        public string Comments { get; set; }

        // public string CustomerName { get; set; }
        // public string CustomerMobile { get; set; }
        // public string CustomerEmail { get; set; }
        // public object[] InvoiceItems { get; set; }
        //public Supplier[] Suppliers { get; set; }
    }
    public class InvoiceTransaction
    {
        public DateTime TransactionDate { get; set; }
        public string PaymentGateway { get; set; }
        public string ReferenceId { get; set; }
        public string TrackId { get; set; }
        public string TransactionId { get; set; }
        public string PaymentId { get; set; }
        public string AuthorizationId { get; set; }
        public string TransactionStatus { get; set; }
        public string TransationValue { get; set; }
        public string CustomerServiceCharge { get; set; }
        public string DueValue { get; set; }
        public string PaidCurrency { get; set; }
        public string PaidCurrencyValue { get; set; }
        public string IpAddress { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public string Error { get; set; }
        public string ErrorCode { get; set; }
    }
}
