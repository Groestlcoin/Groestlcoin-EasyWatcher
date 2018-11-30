using MVVMLibrary;
using System;
using System.Collections.ObjectModel;
using WatchOnlyGroestlcoinWallet.Models;
using WatchOnlyGroestlcoinWallet.Services;
using WatchOnlyGroestlcoinWallet.Services.BalanceServices;
using WatchOnlyGroestlcoinWallet.Services.PriceServices;

namespace WatchOnlyGroestlcoinWallet.ViewModels {
    public class SettingsViewModel : ViewModelBase {
        public SettingsViewModel() {
            BalanceApiList = new ObservableCollection<BalanceServiceNames>((BalanceServiceNames[]) Enum.GetValues(typeof(BalanceServiceNames)));
            PriceApiList = new ObservableCollection<PriceServiceNames>((PriceServiceNames[]) Enum.GetValues(typeof(PriceServiceNames)));
            SupportedCurrencyList = new ObservableCollection<SupportedCurrencies>((SupportedCurrencies[]) Enum.GetValues(typeof(SupportedCurrencies)));

            UpdatePriceCommand = new BindableCommand(UpdatePrice, () => !IsReceiving);

            RaisePropertyChanged("ConversionRateLabelText");
        }

        /// <summary>
        /// Indicating an active connection.
        /// <para/> Used to enable/disable buttons
        /// </summary>
        public bool IsReceiving {
            get { return isReceiving; }
            set {
                if (SetField(ref isReceiving, value)){
                    UpdatePriceCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool isReceiving;

        private string conversionRateLabelText;

        public string ConversionRateLabelText {
            get { return conversionRateLabelText; }
            set {
                conversionRateLabelText = value;
                RaisePropertyChanged("ConversionRateLabelText");
            }
        }

        public ObservableCollection<BalanceServiceNames> BalanceApiList { get; set; }

        public ObservableCollection<PriceServiceNames> PriceApiList { get; set; }
        public ObservableCollection<SupportedCurrencies> SupportedCurrencyList { get; set; }

        private SettingsModel settings;

        public SettingsModel Settings {
            get { return settings; }
            set { SetField(ref settings, value); }
        }

        public BalanceServiceNames SelectedBalanceApi {
            get { return Settings.SelectedBalanceApi; }
            set {
                if (Settings.SelectedBalanceApi != value) // Can't use SetField here because of "ref"
                {
                    Settings.SelectedBalanceApi = value;
                    RaisePropertyChanged("SelectedBalanceApi");
                }
            }
        }

        public PriceServiceNames SelectedPriceApi {
            get { return Settings.SelectedPriceApi; }
            set {
                if (Settings.SelectedPriceApi != value){
                    Settings.SelectedPriceApi = value;
                    RaisePropertyChanged("SelectedPriceApi");
                }
            }
        }

        public SupportedCurrencies SelectedCurrency {
            get {
                if (Settings?.SelectedCurrency != null){
                    return Settings.SelectedCurrency;
                }
                return SupportedCurrencies.EUR;
            }
            set {
                if (Settings.SelectedCurrency != value){
                    Settings.SelectedCurrency = value;
                    RaisePropertyChanged("SelectedCurrency");
                }
            }
        }

        public decimal GroestlcoinPrice {
            get { return Settings.GroestlcoinPriceInUSD; }
            set {
                if (Settings.GroestlcoinPriceInUSD != value){
                    Settings.GroestlcoinPriceInUSD = value;
                    RaisePropertyChanged("GroestlcoinPrice");
                }
            }
        }

        public decimal USDPrice {
            get { return Settings.DollarPriceInLocalCurrency; }
            set {
                if (Settings.DollarPriceInLocalCurrency != value){
                    Settings.DollarPriceInLocalCurrency = value;
                    RaisePropertyChanged("USDPrice");
                }
            }
        }

        public string LocalCurrencySymbol => Settings.SelectedCurrency.ToString();

        public BindableCommand UpdatePriceCommand { get; private set; }

        private async void UpdatePrice() {
            Status = "Fetching Groestlcoin Price...";
            Errors = string.Empty;
            IsReceiving = true;

            PriceApi api = null;
            switch (Settings.SelectedPriceApi){
                case PriceServiceNames.Chainz:
                    api = new WatchOnlyGroestlcoinWallet.Services.PriceServices.Chainz();
                    break;
                case PriceServiceNames.CoinMarketCap:
                    api = new WatchOnlyGroestlcoinWallet.Services.PriceServices.CoinMarketCap();
                    break;
                default:
                    api = new WatchOnlyGroestlcoinWallet.Services.PriceServices.Chainz();
                    break;
            }

            Response<decimal> resp = await api.UpdatePriceAsync();
            if (resp.Errors.Any()){
                Errors = resp.Errors.GetErrors();
                Status = "Encountered an error!";
            }
            else{
                Settings.GroestlcoinPriceInUSD = resp.Result;
                RaisePropertyChanged("GroestlcoinPrice");
                Status = "Price Update Success!";
            }
            IsReceiving = false;
        }
    }
}