using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System;
using System.Linq;
using System.IO;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using KitBoxDesigner.ViewModels;
using KitBoxDesigner.Views;
using KitBoxDesigner.Services;

namespace KitBoxDesigner;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Register global exception handler
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        
        try
        {
            // Setup dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                DisableAvaloniaDataAnnotationValidation();
                
                var mainWindowViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel,
                };
                
                // Add exit handler to prevent immediate closing
                desktop.ShutdownRequested += Desktop_ShutdownRequested;
            }

            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception ex)
        {
            LogException(ex, "Error during application initialization");
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // 🔄 Utiliser KitboxApiService pour les deux interfaces
        services.AddSingleton<KitboxApiService>();
        services.AddSingleton<IPartService>(provider => provider.GetRequiredService<KitboxApiService>());
        services.AddSingleton<IStockService>(provider => provider.GetRequiredService<KitboxApiService>());
        
        // Register AuthenticationService
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        
        // Autres services
        services.AddSingleton<IPriceCalculatorService, PriceCalculatorService>();
        services.AddSingleton<ConfigurationStorageService>();
        
        // Register ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ConfiguratorViewModel>();
        services.AddTransient<InventoryViewModel>();
        services.AddTransient<StockCheckerViewModel>();
        services.AddTransient<PriceCalculatorViewModel>();
    }

    private void Desktop_ShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        // Uncomment to prevent immediate closing and show dialog
        // e.Cancel = true;
    }
    
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        LogException(e.ExceptionObject as Exception, "Unhandled exception");
    }
    
    private void LogException(Exception? exception, string context)
    {
        try
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "KitBoxDesigner_error.log");
            string errorMessage = $"[{DateTime.Now}] {context}: {exception?.ToString() ?? "Unknown error"}";
            File.AppendAllText(logPath, errorMessage + Environment.NewLine + Environment.NewLine);
        }
        catch
        {
            // If logging fails, we can't do much about it
        }
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}