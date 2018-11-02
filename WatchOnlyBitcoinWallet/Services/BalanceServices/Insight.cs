using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WatchOnlyGroestlcoinWallet.Models;

namespace WatchOnlyGroestlcoinWallet.Services.BalanceServices {
    public class Insight : BalanceApi {
        public override async Task<Response> UpdateBalancesAsync(List<GroestlcoinAddress> addrList) {
            Response resp = new Response();
            foreach (var addr in addrList) {
                string url = "https://groestlsight.groestlcoin.org/api/addr/" + addr.Address + "/balance";

                using (var httpClient = new HttpClient()) {
                    var res = await httpClient.GetAsync(url);
                    if (res.IsSuccessStatusCode) {

                        var balance = decimal.Parse(res.Content.ReadAsStringAsync().Result) / 100000000;
                        addr.Difference = balance - addr.Balance;
                        addr.Balance = balance;
                    }
                    else {
                        resp.Errors.Add(res.ReasonPhrase);
                    }
                }
            }
            return resp;
        }

        public override Task<Response> UpdateTransactionListAsync(List<GroestlcoinAddress> addrList) {
            throw new System.NotImplementedException();
        }

        private class InsightResponse {
            public string addrStr { get; set; }
            public decimal balance { get; set; }
            public decimal balanceSat { get; set; }
            public double totalReceived { get; set; }
            public long totalReceivedSat { get; set; }
            public decimal totalSent { get; set; }
            public decimal totalSentSat { get; set; }
            public decimal unconfirmedBalance { get; set; }
            public decimal unconfirmedBalanceSat { get; set; }
            public decimal unconfirmedTxApperances { get; set; }
            public decimal txApperances { get; set; }
            public List<string> transactions { get; set; }
        }

    }
}