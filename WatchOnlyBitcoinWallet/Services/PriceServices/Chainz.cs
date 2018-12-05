using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WatchOnlyGroestlcoinWallet.Services.PriceServices {
    public class Chainz : PriceApi {
        public override async Task<Response<decimal>> UpdatePriceAsync() {
            Response<decimal> resp = new Response<decimal>();
            var url = "https://chainz.cryptoid.info/grs/api.dws?q=ticker.usd";

            using (var httpClient = new HttpClient()) {
                var res = await httpClient.GetAsync(url);
                if (res.IsSuccessStatusCode) {
                    resp.Result = decimal.Parse(res.Content.ReadAsStringAsync().Result, CultureInfo.InvariantCulture);
                }
                else {
                    resp.Errors.Add(res.ReasonPhrase);
                }
            }
            return resp;
        }
    }
}
