using Avalonia.Controls;
using Kitbox_app.ViewModels;

namespace Kitbox_app.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
