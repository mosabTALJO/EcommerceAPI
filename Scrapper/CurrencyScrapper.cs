using EcommerceAPI.Data.Models;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace Scrapper
{
    public class CurrencyScrapper
    {
        public static async Task<List<CurrencyRate>> Scrap(string[] toCurrencies)
        {
            var items = new List<CurrencyRate>();

            foreach (var fromCurrency in toCurrencies)
            {
                ScrapingBrowser sbrow = new ScrapingBrowser();
                var webPage = await sbrow.NavigateToPageAsync(new Uri("https://www.xe.com/currencytables/?from=" + fromCurrency));
                var lst = webPage.Html.CssSelect("th").ToArray();
                foreach (var item in lst)
                {
                    if (string.IsNullOrEmpty(item?.InnerText))
                        continue;

                    if (toCurrencies.Contains(item.InnerText))
                    {
                        var toCurrency = toCurrencies.First(x => x == item.InnerText);
                        if (toCurrency == fromCurrency)
                            continue;

                        var rateTxt = item.NextSibling?.NextSibling?.InnerText.Replace(",", ".");
                        if (double.TryParse(rateTxt, out double rate))
                        {
                            items.Add(new CurrencyRate
                            {
                                FromCurrencyCode = fromCurrency,
                                ToCurrencyCode = toCurrency,
                                Rate = rate,
                                LastUpdatedDate = DateTime.UtcNow,
                            });
                        }
                    }
                }
            }

            return items;

            //ScrapingBrowser sbrow = new ScrapingBrowser();
            //var webPage = await sbrow.NavigateToPageAsync(new Uri("https://kur.doviz.com/"));
            //var lst = webPage.Html.CssSelect("table[id='currencies'] tbody tr").ToArray();
            //using (var db = new MyDbContext())
            //{
            //    var currencyRates = db.CurrencyRates.ToList();
            //    foreach (var item in lst)
            //    {
            //        var slst = item.CssSelect("td").ToArray();
            //        if (slst?.FirstOrDefault()?.InnerText?.Contains("KWD") == true)
            //        {
            //            var rate = slst.ElementAtOrDefault(2)?.InnerText.Replace(".", "").Replace(",", ".");
            //            var kdw = currencyRates.FirstOrDefault(x => x.Type == CurrencyRateTypes.KWD);
            //            kdw.Rate = 1 / double.Parse(rate);
            //            kdw.LastUpdatedDate = DateTime.UtcNow;
            //            db.CurrencyRates.Update(kdw);
            //        }
            //        else if (slst?.FirstOrDefault()?.InnerText?.Contains("USD") == true)
            //        {
            //            var rate = slst.ElementAtOrDefault(2)?.InnerText.Replace(".", "").Replace(",", ".");
            //            var usd = currencyRates.FirstOrDefault(x => x.Type == CurrencyRateTypes.USD);
            //            usd.Rate = 1 / double.Parse(rate);
            //            usd.LastUpdatedDate = DateTime.UtcNow;
            //            db.CurrencyRates.Update(usd);
            //        }
            //        else if (slst?.FirstOrDefault()?.InnerText?.Contains("SAR") == true)
            //        {
            //            var rate = slst.ElementAtOrDefault(2)?.InnerText.Replace(".", "").Replace(",", ".");
            //            var ksa = currencyRates.FirstOrDefault(x => x.Type == CurrencyRateTypes.SAR);
            //            ksa.Rate = 1 / double.Parse(rate);
            //            ksa.LastUpdatedDate = DateTime.UtcNow;
            //            db.CurrencyRates.Update(ksa);
            //        }
            //        else if (slst?.FirstOrDefault()?.InnerText?.Contains("AED") == true)
            //        {
            //            var rate = slst.ElementAtOrDefault(2)?.InnerText.Replace(".", "").Replace(",", ".");
            //            var aed = currencyRates.FirstOrDefault(x => x.Type == CurrencyRateTypes.AED);
            //            aed.Rate = 1 / double.Parse(rate);
            //            aed.LastUpdatedDate = DateTime.UtcNow;
            //            db.CurrencyRates.Update(aed);
            //        }
            //        else if (slst?.FirstOrDefault()?.InnerText?.Contains("OMR") == true)
            //        {
            //            var rate = slst.ElementAtOrDefault(2)?.InnerText.Replace(".", "").Replace(",", ".");
            //            var omr = currencyRates.FirstOrDefault(x => x.Type == CurrencyRateTypes.OMR);
            //            omr.Rate = 1 / double.Parse(rate);
            //            omr.LastUpdatedDate = DateTime.UtcNow;
            //            db.CurrencyRates.Update(omr);
            //        }
            //        else if (slst?.FirstOrDefault()?.InnerText?.Contains("QAR") == true)
            //        {
            //            var rate = slst.ElementAtOrDefault(2)?.InnerText.Replace(".", "").Replace(",", ".");
            //            var qar = currencyRates.FirstOrDefault(x => x.Type == CurrencyRateTypes.QAR);
            //            qar.Rate = 1 / double.Parse(rate);
            //            qar.LastUpdatedDate = DateTime.UtcNow;
            //            db.CurrencyRates.Update(qar);
            //        }
            //        else if (slst?.FirstOrDefault()?.InnerText?.Contains("EGP") == true)
            //        {
            //            var rate = slst.ElementAtOrDefault(2)?.InnerText.Replace(".", "").Replace(",", ".");
            //            var egp = currencyRates.FirstOrDefault(x => x.Type == CurrencyRateTypes.EGP);
            //            egp.Rate = 1 / double.Parse(rate);
            //            egp.LastUpdatedDate = DateTime.UtcNow;
            //            db.CurrencyRates.Update(egp);
            //        }
            //        else if (slst?.FirstOrDefault()?.InnerText?.Contains("IQD") == true)
            //        {
            //            var rate = slst.ElementAtOrDefault(2)?.InnerText.Replace(".", "").Replace(",", ".");
            //            var iqd = currencyRates.FirstOrDefault(x => x.Type == CurrencyRateTypes.IQD);
            //            iqd.Rate = 1 / double.Parse(rate);
            //            iqd.LastUpdatedDate = DateTime.UtcNow;
            //            db.CurrencyRates.Update(iqd);
            //        }
            //        else if (slst?.FirstOrDefault()?.InnerText?.Contains("MAD") == true)
            //        {
            //            var rate = slst.ElementAtOrDefault(2)?.InnerText.Replace(".", "").Replace(",", ".");
            //            var mad = currencyRates.FirstOrDefault(x => x.Type == CurrencyRateTypes.MAD);
            //            mad.Rate = 1 / double.Parse(rate);
            //            mad.LastUpdatedDate = DateTime.UtcNow;
            //            db.CurrencyRates.Update(mad);
            //        }
            //        else if (slst?.FirstOrDefault()?.InnerText?.Contains("JOD") == true)
            //        {
            //            var rate = slst.ElementAtOrDefault(2)?.InnerText.Replace(".", "").Replace(",", ".");
            //            var jod = currencyRates.FirstOrDefault(x => x.Type == CurrencyRateTypes.JOD);
            //            jod.Rate = 1 / double.Parse(rate);
            //            jod.LastUpdatedDate = DateTime.UtcNow;
            //            db.CurrencyRates.Update(jod);
            //        }
            //        else if (slst?.FirstOrDefault()?.InnerText?.Contains("DZD") == true)
            //        {
            //            var rate = slst.ElementAtOrDefault(2)?.InnerText.Replace(".", "").Replace(",", ".");
            //            var dzd = currencyRates.FirstOrDefault(x => x.Type == CurrencyRateTypes.DZD);
            //            dzd.Rate = 1 / double.Parse(rate);
            //            dzd.LastUpdatedDate = DateTime.UtcNow;
            //            db.CurrencyRates.Update(dzd);
            //        }
            //        else if (slst?.FirstOrDefault()?.InnerText?.Contains("BHD") == true)
            //        {
            //            var rate = slst.ElementAtOrDefault(2)?.InnerText.Replace(".", "").Replace(",", ".");
            //            var bhd = currencyRates.FirstOrDefault(x => x.Type == CurrencyRateTypes.BHD);
            //            bhd.Rate = 1 / double.Parse(rate);
            //            bhd.LastUpdatedDate = DateTime.UtcNow;
            //            db.CurrencyRates.Update(bhd);
            //        }
            //    }
            //  if (await db.SaveChangesAsync() > 0)
            //        return true;
            //    return false;
            //}
        }
    }
}
