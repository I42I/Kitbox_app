using System.Windows;
using System.Windows.Controls;

namespace Kitbox_app
{
    public partial class ManagerPage : Page
    {
        public ManagerPage()
        {
            InitializeComponent();
        }

        private void ManageOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new OrdersManagementPage());
        }

        private void ManageStock_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StockManagementPage());
        }

        private void ManageSuppliers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new SupplierManagementPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }
    }
}
