using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WatchOnlyGroestlcoinWallet
{
    /// <summary>
    /// Interaction logic for Titlebar.xaml
    /// </summary>
    public partial class Titlebar : UserControl
    {
        public Titlebar()
        {
            InitializeComponent();
        }
        private bool _isMainWindow = false;
        public bool IsMainWindow {
            get => _isMainWindow;
            set {
                if (value == false) {
                    uxMinimiseBtn.Visibility = Visibility.Hidden;
                    uxMinimiseBtn.IsEnabled = false;
                }
                _isMainWindow = value;
            }
        }
        private bool _isAbout = false;

        public bool IsAbout {
            get => _isAbout;
            set {
                if (value) {
                    uxMinimiseBtn.Visibility = Visibility.Hidden;
                    uxMinimiseBtn.IsEnabled = false;
                    uxAboutBtn.Visibility = Visibility.Hidden;
                    uxAboutBtn.IsEnabled = false;
                }
                _isAbout = value;
            }
        }

        private void UxCloseBtn_OnClick(object sender, RoutedEventArgs e) {
            if (IsMainWindow) {
                Window.GetWindow(this)?.Close();
            }
            else {
                Window.GetWindow(this)?.Hide();
            }
        }

        private void TitleBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Window.GetWindow(this)?.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var win = Window.GetWindow(this);
            if (win != null) {
                win.WindowState = WindowState.Minimized;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            
        }

        private void LnkGit_OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
