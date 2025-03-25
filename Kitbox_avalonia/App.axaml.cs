using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Kitbox_avalonia.Services;
using Kitbox_avalonia.ViewModels;
using Kitbox_avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kitbox_avalonia
{
    public partial class App : Application
    {
        public new static App Current => Application.Current as App;
        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // Register services
            services.AddSingleton<INavigationService>(provider => 
            {
                // We'll set this after window creation
                return null!;
            });
            
            return services.BuildServiceProvider();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow();
                desktop.MainWindow = mainWindow;
                
                // Now we can create the navigation service with the contentControl
                var navigationService = new NavigationService(mainWindow.MainContent);
                
                // Replace the service registration
                var serviceField = typeof(ServiceProvider).GetField("_defaultServiceProvider", 
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                
                var tempProvider = (ServiceProvider)Services;
                serviceField?.SetValue(null, new ServiceCollection()
                    .AddSingleton<INavigationService>(navigationService)
                    .BuildServiceProvider());
                
                // Navigate to the home view
                navigationService.Navigate<HomeViewModel>();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}