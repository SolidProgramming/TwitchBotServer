using System;
using System.IO;
using Serilog;
using Serilog.Events;

namespace Logger
{
    public static class Log
    {
        private static readonly string SolidProgrammingLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SolidProgramming", "Twitch Bot Server", "Log");

        public static void Init()
        {
          
            Serilog.Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Debug()
             .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
             .Enrich.FromLogContext()
             .WriteTo.File(SolidProgrammingLogPath)
             .CreateLogger();

        }
        public static void Info(string infoText, string moduleName)
        {
            Serilog.Log.Information(infoText);
            Console.WriteLine($"[INFO|{moduleName}] {infoText}");
        }
        public static void Fatal(string errorText, string moduleName)
        {
            Serilog.Log.Fatal($"[{moduleName}] {errorText}");
            Console.WriteLine($"[FATAL|{moduleName}] {errorText}");
        }
    }
}
