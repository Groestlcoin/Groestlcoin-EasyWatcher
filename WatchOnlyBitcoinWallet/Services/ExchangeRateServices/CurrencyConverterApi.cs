using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WatchOnlyGroestlcoinWallet.Services.ExchangeRateServices
{
    public class CurrencyConverterApi
    {
        public static Response<decimal?> GetConversion(SupportedCurrencies ToCurrency)
        {
            //var str = Enum.GetName(typeof(Iso4217), ToCurrency);

            Response<decimal?> resp = new Response<decimal?>();

            var url = $"https://free.currencyconverterapi.com/api/v6/convert?q=USD_{Enum.GetName(typeof(SupportedCurrencies), ToCurrency)}&compact=ultra";

            using (var httpClient = new HttpClient())
            {
                var res = httpClient.GetAsync(url).Result;
                if (res.IsSuccessStatusCode)
                {
                    var str = res.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<RootObject>(res.Content.ReadAsStringAsync().Result);
                    resp.Result = (decimal)result.Values.Values.First();
                }
                else
                {
                    resp.Errors.Add(res.ReasonPhrase);
                    resp.Result = null;
                }
            }
            return resp;
        }

        public class RootObject
        {
            [JsonExtensionData]
            public IDictionary<string, JToken> Values;
        }
    }
}
