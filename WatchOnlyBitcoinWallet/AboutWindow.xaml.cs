using System.Windows;

using System.Diagnostics;
using System.Windows.Navigation;
using System.Reflection;

namespace WatchOnlyGroestlcoinWallet
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        private const string DonationAddress = "FYoKoGrSXGpTavNFVbvW18UYxo6JVbUDDa";

        public AboutWindow()
        {
            InitializeComponent();
            DonationHyperLink.NavigateUri = new System.Uri($"groestlcoin:{DonationAddress}");
            txtDonate.Text = DonationAddress;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
