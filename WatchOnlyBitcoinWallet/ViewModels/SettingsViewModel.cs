using MVVMLibrary;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using WatchOnlyGroestlcoinWallet.Models;
using WatchOnlyGroestlcoinWallet.Services;
using WatchOnlyGroestlcoinWallet.Services.BalanceServices;
using WatchOnlyGroestlcoinWallet.Services.PriceServices;

namespace WatchOnlyGroestlcoinWallet.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            BalanceApiList = new ObservableCollection<BalanceServiceNames>((BalanceServiceNames[])Enum.GetValues(typeof(BalanceServiceNames)));
            PriceApiList = new ObservableCollection<PriceServiceNames>((PriceServiceNames[])Enum.GetValues(typeof(PriceServiceNames)));
            SupportedCurrencyList = new ObservableCollection<SupportedCurrencies>((SupportedCurrencies[])Enum.GetValues(typeof(SupportedCurrencies)));

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
                if (SetField(ref isReceiving, value))
                {
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
                if (Settings.SelectedPriceApi != value)
                {
                    Settings.SelectedPriceApi = value;
                    RaisePropertyChanged("SelectedPriceApi");
                }
            }
        }

        public SupportedCurrencies? SelectedCurrency {
            get {
                if (Settings?.SelectedCurrency != null)
                {
                    return Settings.SelectedCurrency;
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
                if (Settings.SelectedCurrency != value)
                {
                    Settings.SelectedCurrency = value;
                    RaisePropertyChanged("SelectedCurrency");
                }
                Settings.LocalCurrencySymbol = Enum.GetName(typeof(SupportedCurrencies), value);
            }
        }

        public string SelectedCurrencySymbol {
            get {
                if (Settings?.SelectedCurrency != null)
                {
                    return LocalCurrencySymbol;
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
                if (Settings.LocalCurrencySymbol != value)
                {
                    Settings.LocalCurrencySymbol = value;
                    RaisePropertyChanged("SelectedCurrencySymbol");
                }
            }
        }

        public decimal GroestlcoinPrice {
            get { return Settings.GroestlcoinPriceInUSD; }
            set {
                if (Settings.GroestlcoinPriceInUSD != value)
                {
                    Settings.GroestlcoinPriceInUSD = value;
                    RaisePropertyChanged("GroestlcoinPrice");
                }
            }
        }

        public decimal USDPrice {
            get { return Settings.DollarPriceInLocalCurrency; }
            set {
                if (Settings.DollarPriceInLocalCurrency != value)
                {
                    Settings.DollarPriceInLocalCurrency = value;
                    RaisePropertyChanged("USDPrice");
                }
            }
        }

        public string LocalCurrencySymbol => Settings.SelectedCurrency.ToString();

        public BindableCommand UpdatePriceCommand { get; private set; }

        public async void UpdatePrice()
        {
            IsReceiving = true;
            Status = "Fetching Groestlcoin Price...";
            Settings.SelectedCurrency = SelectedCurrency;
            Settings.LocalCurrencySymbol = SelectedCurrencySymbol;

            var prices = await SettingsModelContainer.UpdatePrice(Settings);
            Settings = prices.SettingsModel;
            RaisePropertyChanged("SelectedCurrency");
            RaisePropertyChanged("SelectedCurrencySymbol");
            Status = prices.Status;
            Errors = prices.Errors.GetErrors();
            IsReceiving = false;
        }
    }
}