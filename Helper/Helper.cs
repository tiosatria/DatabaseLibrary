
using System.Globalization;

namespace DatabaseLibrary.Helper
{
    public static class Helper
    {
        private static void ShowErrorOnConsole(ErrorLogs logs)
        {
            Console.WriteLine($"(ERROR) [{DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}] System Error: {logs.Message}. Trigger: {logs.Trigger ?? "Unknown"} | Module: {logs.Module ?? "Unknown"}");
        }

        public static void HandleError(Exception ex, string module)
        {
            var error = new ErrorLogs { Message = ex.Message, Module = module, Trigger = ex.Source };
            error.AddToCollection();
            ShowErrorOnConsole(error);
        }

        public static void HandleError(Exception ex, string module, string? trigger)
        {
            var error = new ErrorLogs { Message = ex.Message, Module = module, Trigger = trigger };
            error.AddToCollection();
            ShowErrorOnConsole(error);
        }

        public static void HandleError(Exception ex)
        {
            var error = new ErrorLogs { Message = ex.Message, Module = ex.TargetSite?.Module.Name, Trigger = ex.Source };
            error.AddToCollection();
            ShowErrorOnConsole(error);
        }
    }
}
