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
    /// Logique d'interaction pour LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
         private void LoginButton_Click(object sender, RoutedEventArgs e)
            {
                string username = UsernameInput.Text;
                string password = PasswordInput.Password;

                // Vérification des identifiants
                if (username == "admin" && password == "admin")
                {
                    MessageBox.Show("Connexion réussie !", "Bienvenue", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Ouvrir l'interface de gestionnaire (Manager UI)
                    ManagerWindow managerWindow = new ManagerWindow();
                    managerWindow.Show();

                    // Fermer la fenêtre de login
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
            {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
            }

    }
}
