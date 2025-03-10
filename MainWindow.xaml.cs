using System.Windows;
using System.Windows.Controls;
using Kitbox_app.Pages;

namespace Kitbox_app
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new HomePage());
        }
    }
}