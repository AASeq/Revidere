namespace Revidere;

using System;
using System.Globalization;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

internal static class Logging {

    private static LoggerConfiguration? _configuration;
    private static LogEventLevel _minimumLevel = LogEventLevel.Error;

    private static LoggerConfiguration GetCurrentConfiguration() {
        _configuration ??= new LoggerConfiguration()
                           .Enrich.FromLogContext();
        return _configuration;
    }

    public static void SetupConsole(LogEventLevel minimumLevel = LogEventLevel.Information) {
        if ((int)_minimumLevel > (int)minimumLevel) { _minimumLevel = minimumLevel; };

        _configuration = GetCurrentConfiguration()
                         .WriteTo
                         .Console(
                             restrictedToMinimumLevel: minimumLevel,
                             standardErrorFromLevel: LogEventLevel.Error,
                             outputTemplate: "{Timestamp:HH:mm:ss.fff} {Level:u1} {Message:lj}{NewLine}{Exception}",
                             formatProvider: CultureInfo.InvariantCulture,
                             theme: AnsiConsoleTheme.Code
                         );
    }

    public static void SetupFile(string filePath, LogEventLevel minimumLevel = LogEventLevel.Debug) {
        if ((int)_minimumLevel > (int)minimumLevel) { _minimumLevel = minimumLevel; };

        _configuration = GetCurrentConfiguration()
                         .WriteTo
                         .File(
                             restrictedToMinimumLevel: minimumLevel,
                             path: filePath,
                             rollingInterval: RollingInterval.Day,
                             outputTemplate: "{Timestamp:HH:mm:ss.fff} {Level:u1} {Message:lj}{NewLine}{Exception}",
                             formatProvider: CultureInfo.InvariantCulture
                         );
    }

    public static void SetupSeq(Uri serverUrl, LogEventLevel minimumLevel = LogEventLevel.Debug) {
        if ((int)_minimumLevel > (int)minimumLevel) { _minimumLevel = minimumLevel; };

        _configuration = GetCurrentConfiguration()
                         .WriteTo
                         .Seq(
                             restrictedToMinimumLevel: minimumLevel,
                             serverUrl: serverUrl.ToString()
                         );
    }

    public static void Init() {
        Log.Logger = GetCurrentConfiguration()
                     .MinimumLevel.Is(_minimumLevel)
                     .CreateLogger();
    }

}
