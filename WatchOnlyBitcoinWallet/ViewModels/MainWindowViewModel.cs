using GroestlcoinLibrary;
using MVVMLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using WatchOnlyGroestlcoinWallet.Models;
using WatchOnlyGroestlcoinWallet.Services;
using WatchOnlyGroestlcoinWallet.Services.BalanceServices;
using WatchOnlyGroestlcoinWallet.Services.ExchangeRateServices;

namespace WatchOnlyGroestlcoinWallet.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private DispatcherTimer refreshTimer;

        public MainWindowViewModel()
        {
            AddressList = new BindingList<GroestlcoinAddress>(DataManager.ReadFile<List<GroestlcoinAddress>>(DataManager.FileType.Wallet));
            AddressList.ListChanged += AddressList_ListChanged;

            SettingsInstance = DataManager.ReadFile<SettingsModel>(DataManager.FileType.Settings);

            if (string.IsNullOrEmpty(SettingsInstance.LocalCurrencySymbol))
            {
                SettingsInstance.SelectedCurrency = SettingsInstance.SelectedCurrency;
            }

            GetBalanceCommand = new BindableCommand(GetBalance, () => !IsReceiving);
            SettingsCommand = new BindableCommand(OpenSettings);

            ImportFromTextCommand = new BindableCommand(ImportFromText);
            ImportFromFileCommand = new BindableCommand(ImportFromFile);

            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = new TimeSpan(0, 0, 5);
            refreshTimer.Tick += RefreshBalances;
            refreshTimer.Start();

            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            VersionString = $"Version {ver.Major}.{ver.Minor}.{ver.Build}";

            AddressList.RaiseListChangedEvents = true;
            AddressList.ListChanged += (sender, args) =>
            {
                if (args.ListChangedType == ListChangedType.ItemChanged || args.ListChangedType == ListChangedType.ItemDeleted)
                    GetBalance();
            };
        }

        void RefreshBalances(object state, EventArgs e)
        {
            try
            {
                GetBalance();
                refreshTimer.Interval = new TimeSpan(0, 5, 0);
            }
            catch
            {
                //Do Nothing
            }
        }

        void AddressList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                GroestlcoinAddress addr = ((BindingList<GroestlcoinAddress>)sender)[e.NewIndex];
                if (addr.Address != null)
                {
                    addr.Validate(addr.Address);
                }
                if (!addr.HasErrors)
                {
                    DataManager.WriteFile(AddressList, DataManager.FileType.Wallet);
                }
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.ItemAdded)
            {
                DataManager.WriteFile(AddressList, DataManager.FileType.Wallet);
            }
        }

        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Indicating an active connection.
        /// <para/> Used to enable/disable buttons
        /// </summary>
        public bool IsReceiving {
            get { return isReceiving; }
            set {
                if (SetField(ref isReceiving, value))
                {
                    GetBalanceCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool isReceiving;

        public string VersionString { get; private set; }

        public BindingList<GroestlcoinAddress> AddressList { get; set; }



        private SettingsModel settingsInstance;

        public SettingsModel SettingsInstance {
            get { return settingsInstance; }
            set { SetField(ref settingsInstance, value); }
        }

        public string LocalCurrencySymbol => Enum.GetName(typeof(SupportedCurrencies), SettingsInstance.SelectedCurrency);

        public decimal GroestlcoinBalance {
            get { return AddressList.Sum(x => (decimal)x.Balance); }
        }

        [DependsOnProperty(new[] { "GroestlcoinBalance", "SettingsInstance" })]
        public decimal GroestlcoinBalanceUSD => GroestlcoinBalance * SettingsInstance.GroestlcoinPriceInUSD;

        private decimal? _groestlcoinBalanceLC;

        [DependsOnProperty(new[] { "GroestlcoinBalance", "SettingsInstance" })]
        public decimal? GroestlcoinBalanceLC {
            get {
                var conversion = CurrencyConverterApi.GetConversion(SettingsInstance.SelectedCurrency);
                _groestlcoinBalanceLC = conversion.Result * GroestlcoinBalanceUSD;
                return _groestlcoinBalanceLC;
            }
        }

        public BindableCommand SettingsCommand { get; private set; }

        private void OpenSettings()
        {
            IWindowManager winManager = new SettingsWindowManager();
            SettingsViewModel vm = new SettingsViewModel();
            vm.Settings = SettingsInstance;
            winManager.Show(vm);
            RaisePropertyChanged("SettingsInstance");
            DataManager.WriteFile(SettingsInstance, DataManager.FileType.Settings);
        }

        public BindableCommand ImportFromTextCommand { get; }

        private void ImportFromText()
        {
            IWindowManager winManager = new ImportWindowManager();
            ImportViewModel vm = new ImportViewModel();
            winManager.Show(vm);

            if (vm.AddressList != null && vm.AddressList.Count != 0)
            {
                vm.AddressList.ForEach(x => AddressList.Add(x));
                Status = $"Successfully added {vm.AddressList.Count} addresses.";
            }
        }

        public BindableCommand ImportFromFileCommand { get; private set; }

        private void ImportFromFile()
        {
            Response<string[]> resp = DataManager.OpenFileDialog();
            if (resp.Errors.Any())
            {
                Errors = resp.Errors.GetErrors();
                Status = "Encountered an error while reading from file!";
            }
            else if (resp.Result != null)
            {
                int addrCount = 0;
                foreach (var s in resp.Result)
                {
                    // remove possible white space
                    string addr = s.Replace(" ", "");

                    VerificationResult vr = ValidateAddr(addr);
                    if (vr.IsVerified)
                    {
                        AddressList.Add(new GroestlcoinAddress() { Address = addr });
                        addrCount++;
                    }
                    else
                    {
                        Errors += Environment.NewLine + vr.Error + ": " + addr;
                    }
                }
                Status = $"Successfully added {addrCount} addresses.";
            }
        }

        private VerificationResult ValidateAddr(string addr)
        {
            VerificationResult vr = new VerificationResult();
            if (addr.StartsWith("grs1"))
            {
                vr = SegWitAddress.Verify(addr, SegWitAddress.NetworkType.MainNet);
            }
            else
            {
                vr = Base58.Verify(addr);
            }
            return vr;
        }

        public BindableCommand GetBalanceCommand { get; private set; }

        private async void GetBalance()
        {
            if (!AddressList.ToList().TrueForAll(x => !x.HasErrors) || AddressList.Any(d => string.IsNullOrEmpty(d.Address)))
            {
                Errors = "Fix the errors in addresses first!";
                return;
            }
            Status = "Updating Balances...";
            Errors = string.Empty;
            IsReceiving = true;

            BalanceApi api = null;
            switch (SettingsInstance.SelectedBalanceApi)
            {
                case BalanceServiceNames.Chainz:
                    api = new Chainz();
                    break;
                case BalanceServiceNames.Insight:
                    api = new Insight();
                    break;
                default:
                    api = new Chainz();
                    break;
            }

            // Not all exchanges support Bech32 addresses!
            // The following "if" is to solve that.
            bool hasSegWit = AddressList.Any(x => x.Address.StartsWith("grs1", System.StringComparison.InvariantCultureIgnoreCase));
            if (hasSegWit)
            {
                BalanceApi segApi = new Chainz();
                List<GroestlcoinAddress> legacyAddrs = new List<GroestlcoinAddress>(AddressList.Where(x =>
                                                                                                          !x.Address.StartsWith("grs1", System.StringComparison.OrdinalIgnoreCase)));
                List<GroestlcoinAddress> segWitAddrs = new List<GroestlcoinAddress>(AddressList.Where(x =>
                                                                                                          x.Address.StartsWith("grs1", System.StringComparison.OrdinalIgnoreCase)));

                Response respSW = await segApi.UpdateBalancesAsync(segWitAddrs);
                if (respSW.Errors.Any())
                {
                    Errors = "SegWit API error: " + respSW.Errors.GetErrors();
                    Status = "Error in SegWit API! Continue updating legacy balances...";
                }
                Response resp = await api.UpdateBalancesAsync(legacyAddrs);
                if (resp.Errors.Any())
                {
                    Errors = resp.Errors.GetErrors();
                    Status = "Encountered an error!";
                }
                else
                {
                    DataManager.WriteFile(AddressList, DataManager.FileType.Wallet);
                    RaisePropertyChanged("GroestlcoinBalance");
                    LastUpdated = DateTime.Now;
                    Status = "Balance Update Success!";
                }
            }
            else
            {
                Response resp = await api.UpdateBalancesAsync(AddressList.ToList());
                if (resp.Errors.Any())
                {
                    Errors = resp.Errors.GetErrors();
                    Status = "Encountered an error!";
                }
                else
                {
                    DataManager.WriteFile(AddressList, DataManager.FileType.Wallet);
                    RaisePropertyChanged("GroestlcoinBalance");
                    LastUpdated = DateTime.Now;
                    Status = "Balance Update Success!";
                }
            }
            Status += $" - Last Updated: {LastUpdated.ToShortDateString()} {LastUpdated.ToShortTimeString()}";
            IsReceiving = false;
        }
    }
}