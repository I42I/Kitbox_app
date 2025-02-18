using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using Kitbox_app;


namespace Kitbox_app
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnCreateCabinet_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers la fenêtre de configuration de l'armoire
            ConfigWindow configWindow = new ConfigWindow();
            configWindow.Show();
            this.Close();
        }

        private void BtnViewOrders_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers la fenêtre de gestion des commandes
            OrdersWindow ordersWindow = new OrdersWindow();
            ordersWindow.Show();
            this.Close();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers la fenêtre de connexion (si besoin)
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}