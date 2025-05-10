using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace Kitbox_app
{
    sealed class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("🚨 Erreur non gérée :");
                Console.WriteLine(e.ExceptionObject.ToString());
                Console.ResetColor();
                Console.WriteLine("\nAppuie sur Entrée pour quitter...");
                Console.ReadLine();
            };

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .WithInterFont()
                         .LogToTrace()
                         .UseReactiveUI();
    }
}
