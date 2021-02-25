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

        public static void LogInfo(string infoText)
        {
            Serilog.Log.Information(infoText);
        }

        public static void LogFatal(string errorText)
        {
            Serilog.Log.Fatal(errorText);
        }
    }
}
