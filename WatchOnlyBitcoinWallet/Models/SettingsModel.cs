using System;
using System.Globalization;
using System.Threading.Tasks;
using MVVMLibrary;
using WatchOnlyGroestlcoinWallet.Properties;
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
        public SupportedCurrencies? SelectedCurrency { get; set; }
    }

    /// <summary>
    /// This is used to remove the reliance of SettingsViewModel on retrieving fiat prices. If no Settings.json file existed, USD and secondary currency would show zero. This removes that reliance
    /// and gives us the ability to call update prices from anywhere that uses the settings model.
    /// </summary>
    public class SettingsModelContainer : CommonBase {
        public SettingsModel SettingsModel { get; set; }
        public ErrorCollection Errors { get; set; }
        public string Status { get; set; }
        public SupportedCurrencies? SelectedCurrency {
            get {
                if (SettingsModel?.SelectedCurrency != null)
                {
                    return SettingsModel.SelectedCurrency;
                }
                var local = NumberFormatInfo.CurrentInfo.CurrencySymbol;
                switch (local)
                {
                    case ("£"):
                        return SupportedCurrencies.GBP;
                    case ("₩"):
                        return SupportedCurrencies.KRW;
                    case ("¥"):
                        return SupportedCurrencies.CNY;
                    case ("€"):
                    default:
                        return SupportedCurrencies.EUR;
                }
            }
            set {
                if (SettingsModel.SelectedCurrency != value)
                {
                    SettingsModel.SelectedCurrency = value;
                    RaisePropertyChanged("SelectedCurrency");
                }
                SettingsModel.LocalCurrencySymbol = Enum.GetName(typeof(SupportedCurrencies), value);
            }
        }
        public string SelectedCurrencySymbol {
            get {
                if (SettingsModel?.SelectedCurrency != null)
                {
                    return SettingsModel.SelectedCurrency.ToString();
                }
                var local = NumberFormatInfo.CurrentInfo.CurrencySymbol;
                switch (local)
                {
                    case ("£"):
                        return SupportedCurrencies.GBP.ToString();
                    case ("€"):
                        return SupportedCurrencies.EUR.ToString();
                    case ("₩"):
                        return SupportedCurrencies.KRW.ToString();
                    case ("¥"):
                        return SupportedCurrencies.CNY.ToString();
                    default:
                        return SupportedCurrencies.EUR.ToString();
                }
            }
            set {
                if (SettingsModel.LocalCurrencySymbol != value)
                {
                    SettingsModel.LocalCurrencySymbol = value;
                    RaisePropertyChanged("SelectedCurrencySymbol");
                }
            }
        }



        public static async Task<SettingsModelContainer> UpdatePrice(SettingsModel settingsModel)
        {
            var settingsContainer = new SettingsModelContainer();
            settingsContainer.SettingsModel = settingsModel;
            settingsContainer.Status = "Fetching Groestlcoin Price...";
            settingsContainer.Errors = new ErrorCollection();
            
            PriceApi api = null;
            switch (settingsContainer.SettingsModel.SelectedPriceApi)
            {
                case PriceServiceNames.Chainz:
                    api = new Services.PriceServices.Chainz();
                    break;
                case PriceServiceNames.CoinMarketCap:
                    api = new Services.PriceServices.CoinMarketCap();
                    break;
                default:
                    api = new Services.PriceServices.Chainz();
                    break;
            }
            settingsContainer.SettingsModel.SelectedCurrency = settingsContainer.SelectedCurrency;
            settingsContainer.SettingsModel.LocalCurrencySymbol = settingsContainer.SelectedCurrencySymbol;
            Response<decimal> resp = await api.UpdatePriceAsync();
            if (resp.Errors.Any()){
                settingsContainer.Errors = resp.Errors;
                settingsContainer.Status = "Encountered an error!";
            }
            else
            {
                settingsContainer.SettingsModel.GroestlcoinPriceInUSD = resp.Result;
                settingsContainer.Status = "Price Update Success!";
            }
            return settingsContainer;
        }
    }
}
