using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WatchOnlyGroestlcoinWallet.Models;

namespace WatchOnlyGroestlcoinWallet.Services {
    public enum PriceServiceNames {
        Chainz,
        CoinMarketCap
    }
    public enum BalanceServiceNames {
        Chainz
    }

    public abstract class Api {
        protected async Task<Response<JObject>> SendApiRequestAsync(string url) {
            Response<JObject> resp = new Response<JObject>();
            using (HttpClient client = new HttpClient()) {
                try {
                    string result = await client.GetStringAsync(url);
                    resp.Result = JObject.Parse(result);
                }
                catch (Exception ex) {
                    string errMsg = (ex.InnerException == null) ? ex.Message : ex.Message + " " + ex.InnerException;
                    resp.Errors.Add(errMsg);
                }
            }
            return resp;
        }
    }

    public abstract class PriceApi : Api {
        public abstract Task<Response<decimal>> UpdatePriceAsync();
    }

    public abstract class BalanceApi : Api {
        public abstract Task<Response> UpdateBalancesAsync(List<GroestlcoinAddress> addrList);

        public abstract Task<Response> UpdateTransactionListAsync(List<GroestlcoinAddress> addrList);

        internal decimal Satoshi = 0.00000001m;
    }
}
