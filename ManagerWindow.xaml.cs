using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Kitbox;

namespace KitBox
{
    /// <summary>
    /// Logique d'interaction pour ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        public ManagerWindow()
        {
            InitializeComponent();
        }
        private void ManageOrders_Click(object sender, RoutedEventArgs e)
        {
            OrdersManagementWindow ordersWindow = new OrdersManagementWindow();
            ordersWindow.Show();
            this.Close();
        }

        private void ManageStock_Click(object sender, RoutedEventArgs e)
        {
            StockManagementWindow stockWindow = new StockManagementWindow();
            stockWindow.Show();
            this.Close();
        }

        private void ManageSuppliers_Click(object sender, RoutedEventArgs e)
        {
            SupplierManagementWindow suppliersWindow = new SupplierManagementWindow();
            suppliersWindow.Show();
            this.Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
