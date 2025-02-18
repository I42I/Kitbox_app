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
    /// Logique d'interaction pour OrdersWindow.xaml
    /// </summary>
    public partial class OrdersWindow : Window
    {
        public OrdersWindow()
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
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

    }
}
