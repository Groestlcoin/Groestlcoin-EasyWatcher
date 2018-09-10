using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WatchOnlyBitcoinWallet.Models;

namespace WatchOnlyBitcoinWallet.Services.BalanceServices {
    public class ChainzInfo : BalanceApi {
        public override async Task<Response> UpdateBalancesAsync(List<BitcoinAddress> addrList) {
            Response resp = new Response();
            foreach (var addr in addrList) {
                string url = "http://chainz.cryptoid.info/grs/api.dws?q=getbalance&a=" + addr.Address;

                using (var httpClient = new HttpClient()) {
                    var res = await httpClient.GetAsync(url);
                    if (res.IsSuccessStatusCode) {

                        var balance = decimal.Parse(res.Content.ReadAsStringAsync().Result);
                        addr.Difference = balance - addr.Balance;
                        addr.Balance = balance;
                    }
                    else {
                        resp.Errors.Add(res.ReasonPhrase);
                    }
                }

                //Response<JObject> apiResp = await SendApiRequestAsync(url);
                //if (apiResp.Errors.Any()) {
                //    resp.Errors.AddRange(apiResp.Errors);
                //    break;
                //}
                //decimal bal = (Int64)apiResp.Result["final_balance"] * Satoshi;
                //addr.Difference = bal - addr.Balance;
                //addr.Balance = bal;
            }
            return resp;
        }

        public override Task<Response> UpdateTransactionListAsync(List<BitcoinAddress> addrList) {
            throw new System.NotImplementedException();
        }

    }
}