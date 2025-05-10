using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Kitbox_app.ViewModels;
using Kitbox_app.Services;

namespace Kitbox_app
{
    public partial class App : Application
    {
        // ✅ Propriété publique statique pour le service de navigation
        public static INavigationService NavigationService { get; private set; } = null!;

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // ✅ Création du MainWindowViewModel
                var mainViewModel = new MainWindowViewModel();

                // ✅ Assigner l'instance existante du NavigationService
                NavigationService = mainViewModel.Navigation;

                // ✅ Définir la fenêtre principale avec son DataContext
                desktop.MainWindow = new Views.MainWindow
                {
                    DataContext = mainViewModel
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
