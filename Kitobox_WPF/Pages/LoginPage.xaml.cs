using System.Windows;
using System.Windows.Controls;

namespace Kitbox_app.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameInput.Text;
            string password = PasswordInput.Password;

            if (username == "admin" && password == "admin")
            {
                MessageBox.Show("Connexion réussie !", "Bienvenue", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.Navigate(new ManagerPage());
            }
            else
            {
                MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }
    }
}
