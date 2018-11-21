using System.Windows;
using System.Windows.Input;

namespace WatchOnlyGroestlcoinWallet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow myWin = new AboutWindow();
            myWin.Owner = this;
            myWin.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MainWindowCopy_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e){
            try {
                this?.DragMove();
            }
            catch {
                //Ignored
            }
        }
    }
}
