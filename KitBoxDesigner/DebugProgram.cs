using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;

namespace KitBoxDesigner
{
    class DebugProgram
    {
        // For debugging only
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting KitBoxDesigner in debug mode...");
                
                // Register global exception handlers
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    var ex = e.ExceptionObject as Exception;
                    LogError("Unhandled exception", ex);
                };
                
                TaskScheduler.UnobservedTaskException += (s, e) =>
                {
                    LogError("Unobserved task exception", e.Exception);
                };
                
                // Run the application
                Program.BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                LogError("Critical startup error", ex);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
        
        private static void LogError(string context, Exception? ex)
        {
            string errorMessage = $"[{DateTime.Now}] {context}: {ex?.ToString() ?? "Unknown error"}";
            
            // Write to console
            Console.WriteLine(errorMessage);
            
            // Write to file
            try
            {
                string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "KitBoxDesigner_debug.log");
                File.AppendAllText(logPath, errorMessage + Environment.NewLine + Environment.NewLine);
                Console.WriteLine($"Error logged to: {logPath}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error logging to file: {e.Message}");
            }
        }
    }
}
