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
        public static void AddCollectionDiscounts(ICollectionService _collectionService, Order order)
        {
            var collections = new Dictionary<int, List<OrderItem>>(); // collection ID, order items in that collection
            var pIds = order.OrderItems.Select(x => x.ProductId).ToArray();

            var collectionsIds = _collectionService.GetCollectionsByProducts(pIds);
            foreach (var id in collectionsIds)
            {
                collections.Add(id.Key, order.OrderItems.Where(x => id.Value.Select(x => x.ProductId).Contains(x.ProductId)).OrderBy(oi => oi.SalePrice).ToList());
            }
            foreach (var pair in collections)
            {
                while (pair.Value.Sum(x => x.RemainingQtyForDiscount - x.Qty) > 1)
                {
                    var first = pair.Value.First(x => x.RemainingQtyForDiscount < x.Qty);
                    var last = pair.Value.Last(x => x.RemainingQtyForDiscount < x.Qty);

                    first.RemainingQtyForDiscount++;
                    last.RemainingQtyForDiscount++;
                    first.DiscountAmount++;
                }
            }
        }
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
            order.CurrencyRate = _currencyRateService.GetCurrencyRate(order.Currency);
            order.PaymentGateway = PaymentGateway.MyFatoorah;

            double totalWithoutCuponDiscount = 0;
            double totalWithCuponDiscount = 0;
            bool couponExists = false;

            //AddCollectionDiscounts(_collectionService, order);

            foreach (var oi in order.OrderItems ?? [])
            {
                var pv = _productVariantService.Queryable().Include(x => x.Product).ThenInclude(x => x.Category).AsNoTracking()
                .FirstOrDefault(x => x.Id == oi.ProductVariantId);

                bool pvValid = pv != null && pv.Product?.Category?.IsActive == true;
                if (pvValid && pv!.ProductVariants?.Count > 0)
                    pvValid = pv.ProductVariants?.Any(x => x.Id == oi.ProductVariantId && x.StockLevel > 0) == true;
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
                    || coupon.Type == CouponType.Product && p.Id == coupon.TypeId
                    || coupon.Type == CouponType.Brand && p.BrandId == coupon.TypeId
                    || coupon.Type == CouponType.Category && p.CategoryId == coupon.TypeId)
                    {
                        oi.CoupunDiscount = coupon.Rate;
                        couponExists = true;
                    }
                }

                oi.ProductVariant.MainImage = string.IsNullOrWhiteSpace(oi.ProductVariant.MainImage) || oi.ProductVariant.MainImage.StartsWith("https") ? oi.ProductVariant.MainImage : $"https://cdn.dsmcdn.com/mnresize/{600}/{500}" + oi.ProductVariant.MainImage;

                //Calcualte Prices
                oi.ProductSalesPrice = p.CompanyPrice.Value * (p.Category.ProfitRatio ?? 1.5) * order.CurrencyRate;
                oi.ProductDiscount = p.Discount > 0 ? p.Discount : p.Category.Discount > 0 ? p.Category.Discount : null;
                oi.SalePrice = oi.ProductSalesPrice - oi.ProductSalesPrice * (oi.ProductDiscount ?? 0) / 100;

                totalWithoutCuponDiscount += oi.SalePrice * oi.Qty;
                oi.SalePrice = oi.SalePrice - oi.SalePrice * (oi.CoupunDiscount ?? 0) / 100;
                oi.DiscountAmount = Math.Round(oi.DiscountAmount * oi.SalePrice, 3); //convert the DiscountAmount from Qty to money
                oi.TotalSalePrice = Math.Round(oi.Qty * oi.SalePrice - oi.DiscountAmount, 3); //**********************
                totalWithCuponDiscount += oi.TotalSalePrice;
                order.DiscountAmount += oi.DiscountAmount;

                //oi.ProductVariant.CompanyPrice = p.Category.ProfitRatio; //look here
                //oi.ProductVariant.CompanyProductID = null;
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
            IProductService _productService,
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

            order.CurrencyRate = _currencyRateService.GetCurrencyRate(order.Currency);
            order.PaymentGateway = PaymentGateway.MyFatoorah;

            order.RecipientFirstName = address.FirstName;
            order.RecipientLastName = address.LastName;
            order.PhoneNumber = address.PhoneNumber;
            order.Country = address.Country;
            order.Lat = address.Lat;
            order.Lng = address.Lng;
            order.FullAddress = address.FullAddress;
            order.Country = c.NameEn;
            order.City = c.Cities?.FirstOrDefault(x => c.Code == address.City)?.NameEn ?? address.City;
            order.PaymentGateway = PaymentGateway.MyFatoorah;

            bool couponExists = false;
            double totalWithoutCuponDiscount = 0;
            double totalWithCuponDiscount = 0;
            bool areProductsValid = true;

            //AddCollectionDiscounts(_collectionService, order);

            foreach (var oi in order.OrderItems)
            {
                var p = _productService.Queryable().Include(x => x.ProductVariants).Include(x => x.Category).AsNoTracking()
                .FirstOrDefault(x => x.Id == oi.ProductId);

                bool productValid = p != null && p.Category?.IsActive == true;
                if (productValid && p!.ProductVariants?.Count > 0)
                    productValid = p.ProductVariants?.Any(x => x.Id == oi.ProductVariantId && x.StockLevel > 0) == true;
                if (!productValid)
                {
                    oi.Qty = 0;
                    areProductsValid = false;
                    continue;
                }

                if (coupon != null)
                {
                    if (coupon.Type == CouponType.General
                    || coupon.Type == CouponType.Product && p.Id == coupon.TypeId
                    || coupon.Type == CouponType.Brand && p.BrandId == coupon.TypeId
                    || coupon.Type == CouponType.Category && p.CategoryId == coupon.TypeId)
                    {
                        oi.CoupunDiscount = coupon.Rate;
                        couponExists = true;
                    }
                }
                //Calcualte Prices
                oi.ProductSalesPrice = p.CompanyPrice.Value * p.Category.ProfitRatio.Value * order.CurrencyRate;
                oi.ProductDiscount = p.Discount > 0 ? p.Discount : p.Category.Discount > 0 ? p.Category.Discount : null;
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

            order.TotalTLPrice = order.TotalSalePrice / order.CurrencyRate;

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
                ExpiryDate = DateTime.Now.AddYears(1),
            };
        }
        public static bool IsPhoneNumber(string number) =>
            Regex.Match(number, @"^(\+[0-9]{5,12})$").Success;
    }
}