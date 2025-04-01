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
using Kitbox_app;

namespace Kitbox_app.Pages
{
    public partial class OrdersPage : Page
    {
        public OrdersPage()
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

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }
    }
}
