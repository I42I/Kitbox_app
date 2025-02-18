using System.Windows;
using System.Windows.Controls;

namespace Kitbox_app
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void BtnCreateCabinet_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ConfigPage());
        }

        private void BtnViewOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new OrdersPage());
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginPage());
        }
    }
}
