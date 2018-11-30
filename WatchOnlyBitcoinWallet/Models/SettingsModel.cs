using MVVMLibrary;
using WatchOnlyGroestlcoinWallet.Services;

namespace WatchOnlyGroestlcoinWallet.Models
{
    public class SettingsModel : CommonBase
    {
        public decimal GroestlcoinPriceInUSD { get; set; }
        public decimal DollarPriceInLocalCurrency { get; set; }
        public string LocalCurrencySymbol { get; set; }
        public BalanceServiceNames SelectedBalanceApi { get; set; }
        public PriceServiceNames SelectedPriceApi { get; set; }
        public SupportedCurrencies SelectedCurrency { get; set; }
    }
}
