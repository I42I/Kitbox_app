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

namespace Kitbox_app
{
    /// <summary>
    /// Logique d'interaction pour OrdersManagementWindow.xaml
    /// </summary>
    public partial class OrdersManagementWindow : Window
    {
        public OrdersManagementWindow()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            OrdersList.Items.Add("Commande #001 - En cours");
            OrdersList.Items.Add("Commande #002 - Prête pour retrait");
        }

        private void RefreshOrders_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Liste des commandes mise à jour !");
        }

        private void BackToManager_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow?.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new ManagerPage());
            }
            this.Close();
        }
    }
}
