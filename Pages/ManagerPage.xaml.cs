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
            NavigationService?.Navigate(new OrdersManagementWindow());
        }

        private void ManageStock_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new StockManagementWindow());
        }

        private void ManageSuppliers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new SupplierManagementWindow());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }
    }
}
