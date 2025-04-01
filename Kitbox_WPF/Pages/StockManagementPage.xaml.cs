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

namespace Kitbox_app.Pages
{
    /// <summary>
    /// Logique d'interaction pour StockManagementPage.xaml
    /// </summary>
    public partial class StockManagementPage : Page
    {
        public StockManagementPage()
        {
            InitializeComponent();
        }

        private void RefreshStock_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Stock mis à jour !");
        }

        private void BackToManager_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ManagerPage());
        }
    }
}
