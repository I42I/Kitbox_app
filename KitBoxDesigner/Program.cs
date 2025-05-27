using Avalonia;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KitBoxDesigner;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            LogError("Main method exception", ex);
            Console.WriteLine($"Critical error: {ex}");
        }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        LogError("Unhandled exception", e.ExceptionObject as Exception);
    }

    private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogError("Unobserved task exception", e.Exception);
        e.SetObserved();
    }
    
    private static void LogError(string context, Exception? ex)
    {
        try
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "KitBoxDesigner_error_program.log");
            string errorMessage = $"[{DateTime.Now}] {context}: {ex?.ToString() ?? "Unknown error"}";
            File.AppendAllText(logPath, errorMessage + Environment.NewLine + Environment.NewLine);
            
            // Also write to console for debugging
            Console.WriteLine(errorMessage);
        }
        catch
        {
            // If logging fails, we can't do much about it
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
