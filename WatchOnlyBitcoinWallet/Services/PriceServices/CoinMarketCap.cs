using System;
using System.Threading;
using System.Threading.Tasks;
using CoinMarketCap;

namespace WatchOnlyGroestlcoinWallet.Services.PriceServices {
    public class CoinMarketCap : PriceApi {
        public static CoinMarketCapClient priceClient = new CoinMarketCapClient();

        public override async Task<Response<decimal>> UpdatePriceAsync() {
            Response<decimal> resp = new Response<decimal>();

            var ticker = await priceClient.GetTickerAsync(CancellationToken.None, 258);

            try {
                resp.Result = (decimal)ticker.Data.Quotes["USD"].Price;
            }
            catch (Exception e) {
                resp.Errors.Add("Unable to Fetch Price");
            }
            return resp;
        }
    }
}
