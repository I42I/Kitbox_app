using Avalonia.Controls;
using Kitbox_avalonia.Views;

namespace Kitbox_avalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CurrentPage.Content = new HomeView(); // page par défaut au lancement
        }

        public void NavigateTo(UserControl page)
        {
            CurrentPage.Content = page;
        }
    }
}