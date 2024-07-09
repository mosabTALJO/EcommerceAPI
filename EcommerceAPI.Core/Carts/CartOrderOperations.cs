using AutoMapper;
using EcommerceAPI.Core.Orders;
using EcommerceAPI.Data.Models;
using EcommerceAPI.Services;
using EcommerceAPI.UnitOfWork.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EcommerceAPI.Core.Carts
{
    public class CartOrderOperations
    {
        public static async Task CalcualteCartItems(
            ICountryService _countryService,
            IProductVariantService _productVariantService,
            IOrderService _orderService,
            IUnitOfWork _unitOfWork,
            ICollectionService _collectionService,
            ICurrencyRateService _currencyRateService,
            Order order,
            Coupon? coupon = null
            )
        {
            var c = _countryService.Queryable().AsNoTracking().FirstOrDefault(x => x.CurrencyCode.ToLower() == order.Currency.ToLower()) ??
                _countryService.Queryable().AsNoTracking().FirstOrDefault(x => x.CurrencyCode.ToLower() == "kwd");
            order.Country = c.Code;
            order.PaymentGateway = PaymentGateway.MyFatoorah;

            double totalWithoutCuponDiscount = 0;
            double totalWithCuponDiscount = 0;
            bool couponExists = false;

            //AddCollectionDiscounts(_collectionService, order);

            foreach (var oi in order.OrderItems ?? [])
            {
                var pv = await _productVariantService.Queryable().Where(x => x.Id == oi.ProductVariantId)
                    .Select(x => new
                    {
                        x.Id,
                        x.ProductId,
                        x.Product!.IsActive,
                        x.StockLevel,
                        x.Product.BrandId,
                        x.Product.CategoryId,
                        x.Product.SalesPrice,
                        x.Discount,
                        CategoryDiscount = x.Product.Category.Discount,
                    }).FirstOrDefaultAsync();

                bool pvValid = pv != null && pv.IsActive == true && pv.StockLevel > 0;
                if (!pvValid)
                {
                    oi.Qty = 0;
                    _orderService.DeleteOrderItem(oi);
                    await _unitOfWork.SaveChangesAsync();
                    continue;
                }
                if (coupon != null)
                {
                    if (coupon.Type == CouponType.General
                    || coupon.Type == CouponType.Product && pv!.ProductId == coupon.TypeId
                    || coupon.Type == CouponType.Brand && pv!.BrandId == coupon.TypeId
                    || coupon.Type == CouponType.Category && pv!.CategoryId == coupon.TypeId)
                    {
                        oi.CoupunDiscount = coupon.Rate;
                        couponExists = true;
                    }
                }

                //Calcualte Prices
                oi.ProductSalesPrice = pv.SalesPrice;// * order.CurrencyRate;
                oi.ProductDiscount = pv.Discount > 0 ? pv.Discount : pv.CategoryDiscount > 0 ? pv.CategoryDiscount : null;
                oi.SalePrice = oi.ProductSalesPrice - oi.ProductSalesPrice * (oi.ProductDiscount ?? 0) / 100;

                totalWithoutCuponDiscount += oi.SalePrice * oi.Qty;
                oi.SalePrice = oi.SalePrice - oi.SalePrice * (oi.CoupunDiscount ?? 0) / 100;
                oi.DiscountAmount = Math.Round(oi.DiscountAmount * oi.SalePrice, 3); //convert the DiscountAmount from Qty to money
                oi.TotalSalePrice = Math.Round(oi.Qty * oi.SalePrice - oi.DiscountAmount, 3); //**********************
                totalWithCuponDiscount += oi.TotalSalePrice;
                order.DiscountAmount += oi.DiscountAmount;

                //oi.ProductVariant.CompanyPrice = p.Category.ProfitRatio; //look here
            }

            order.SubTotalSalePrice = totalWithoutCuponDiscount;
            order.DeliveryPrice = OrderOperations.CalculateOrderDeliveryPrice(totalWithCuponDiscount, c);
            order.TotalSalePrice = totalWithCuponDiscount + order.DeliveryPrice;
            order.OrderItems = order.OrderItems.Where(x => x.Qty > 0).ToArray();

            if (couponExists)
            {
                order.CoupunDiscountRate = coupon.Rate;
                order.DiscountCoupunId = coupon.Id;
            }
        }
        public static async Task<bool> CalcualteAndValidateOrderItems(
            ICountryService _countryService,
            IProductVariantService _productVariantService,
            ICouponService _couponService,
            ICollectionService _collectionService,
            ICurrencyRateService _currencyRateService,
            Order order,
            UserAddress address,
            Coupon coupon,
            string Currency)
        {
            //await CheckProductsCompanyData(order);

            order.Currency = Currency;
            var c = await _countryService.Queryable().AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode.ToLower() == order.Currency.ToLower()) ??
                await _countryService.Queryable().AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode.ToLower() == "kwd");

            order.RecipientFirstName = address.FirstName;
            order.RecipientLastName = address.LastName;
            order.PhoneNumber = address.PhoneNumber;
            order.Lat = address.Lat;
            order.Lng = address.Lng;
            order.FullAddress = address.FullAddress;
            order.PaymentGateway = PaymentGateway.MyFatoorah;
            order.Country = c.NameEn;
            order.City = c.Cities?.FirstOrDefault(x => c.Id == address.CityId)?.NameEn ?? "";

            bool couponExists = false;
            double totalWithoutCuponDiscount = 0;
            double totalWithCuponDiscount = 0;
            bool areProductsValid = true;

            //AddCollectionDiscounts(_collectionService, order);

            foreach (var oi in order.OrderItems)
            {
                var pv = await _productVariantService.Queryable().Where(x => x.Id == oi.ProductVariantId)
                    .Select(x => new
                    {
                        x.Id,
                        x.ProductId,
                        x.Product.Category.IsActive,
                        x.StockLevel,
                        x.Discount,
                        x.SalesPrice,
                        x.Product.CategoryId,
                        x.Product.BrandId,
                        CategoryDiscount = x.Product.Category.Discount,
                    }).FirstOrDefaultAsync();

                bool productValid = pv != null && pv.IsActive == true && pv.StockLevel > 0;
                if (!productValid)
                {
                    oi.Qty = 0;
                    areProductsValid = false;
                    continue;
                }

                if (coupon != null)
                {
                    if (coupon.Type == CouponType.General
                    || coupon.Type == CouponType.Product && pv!.Id == coupon.TypeId
                    || coupon.Type == CouponType.Brand && pv!.BrandId == coupon.TypeId
                    || coupon.Type == CouponType.Category && pv!.CategoryId == coupon.TypeId)
                    {
                        oi.CoupunDiscount = coupon.Rate;
                        couponExists = true;
                    }
                }
                //Calcualte Prices
                oi.ProductSalesPrice = pv.SalesPrice;// * order.CurrencyRate;
                oi.ProductDiscount = pv.Discount > 0 ? pv.Discount : pv.CategoryDiscount > 0 ? pv.CategoryDiscount : null;
                oi.SalePrice = oi.ProductSalesPrice - oi.ProductSalesPrice * (oi.ProductDiscount ?? 0) / 100;

                totalWithoutCuponDiscount += oi.SalePrice * oi.Qty;
                oi.SalePrice -= oi.SalePrice * (oi.CoupunDiscount ?? 0) / 100;
                oi.DiscountAmount = Math.Round(oi.DiscountAmount * oi.SalePrice, 3); //convert the DiscountAmount from Qty to money
                oi.TotalSalePrice = Math.Round(oi.Qty * oi.SalePrice - oi.DiscountAmount, 3); //**********************
                totalWithCuponDiscount += oi.TotalSalePrice;
                order.DiscountAmount += oi.DiscountAmount;
            }
            order.SubTotalSalePrice = totalWithoutCuponDiscount;
            order.DeliveryPrice = OrderOperations.CalculateOrderDeliveryPrice(totalWithCuponDiscount, c);
            order.TotalSalePrice = totalWithCuponDiscount + order.DeliveryPrice;

            if (areProductsValid && couponExists)
            {
                order.CoupunDiscountRate = coupon.Rate;
                order.DiscountCoupunId = coupon.Id;

                coupon.RemainingCount--;
                _couponService.Update(coupon);
            }

            return areProductsValid;
        }
        public static object GeneratePaymentRequest(Order order, AppUser appUser, UserAddress userAddress, double totalKwdPrice)
        {
            var mobileCountryCode = userAddress.PhoneNumber.StartsWith("+20") || userAddress.PhoneNumber.StartsWith("+90") ? userAddress.PhoneNumber.Remove(3) : userAddress.PhoneNumber.Remove(4);
            return new
            {
                //required fields
                InvoiceValue = totalKwdPrice,
                CustomerName = userAddress.FirstName + " " + userAddress.LastName,
                NotificationOption = "LNK",
                DisplayCurrencyIso = "KWD",
                CallBackUrl = "https://api.trgatestore.com/api/Payment/myfatoorah/success",
                ErrorUrl = "https://api.trgatestore.com/api/Payment/myfatoorah/failed",
                Language = "En",
                UserDefinedField = "OrderId", //The custom field that you have passed in the request
                CustomerReference = order.Id.ToString(), //Refers to the order or transaction ID in your system that you have sent in the request earlier
                                                         //optional fields 
                MobileCountryCode = mobileCountryCode,
                CustomerMobile = userAddress.PhoneNumber.Substring(mobileCountryCode.Length),
                CustomerEmail = appUser.Email,
                //CustomerCivilId = "",
                ExpiryDate = DateTime.UtcNow.AddYears(1),
            };
        }
        public static bool IsPhoneNumber(string number) =>
            Regex.Match(number, @"^(\+[0-9]{5,12})$").Success;
    }
}