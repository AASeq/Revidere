namespace Revidere;

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

/// <summary>
/// Serilog logging setup.
/// </summary>
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
                            theme: new SystemConsoleTheme(
                                new Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle> {
                                    [ConsoleThemeStyle.Text] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White },
                                    [ConsoleThemeStyle.SecondaryText] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Gray },
                                    [ConsoleThemeStyle.TertiaryText] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.DarkGray },
                                    [ConsoleThemeStyle.Invalid] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Yellow },
                                    [ConsoleThemeStyle.Null] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Blue },
                                    [ConsoleThemeStyle.Name] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Gray },
                                    [ConsoleThemeStyle.String] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Cyan },
                                    [ConsoleThemeStyle.Number] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Magenta },
                                    [ConsoleThemeStyle.Boolean] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Blue },
                                    [ConsoleThemeStyle.Scalar] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.Magenta },

                                    [ConsoleThemeStyle.LevelVerbose] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.DarkGray },
                                    [ConsoleThemeStyle.LevelDebug] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.DarkGray  },
                                    [ConsoleThemeStyle.LevelInformation] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.DarkBlue },
                                    [ConsoleThemeStyle.LevelWarning] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.Yellow },
                                    [ConsoleThemeStyle.LevelError] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.Red },
                                    [ConsoleThemeStyle.LevelFatal] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.Red },
                                })
                         );
    }

    public static void SetupConsole(FrozenDictionary<string, string> properties) {
        // level: none, verbose, debug, information, warning, error, fatal      # log level (default: information)

        if (properties.TryGetValue("level", out string? levelProperty)) {
            if ("none".Equals(levelProperty, StringComparison.OrdinalIgnoreCase)) { return; }  // must be None if console is to be supressed
            if (!Enum.TryParse<LogEventLevel>(levelProperty, ignoreCase: true, out var level)) { throw new InvalidOperationException($"Unrecognized logging level '{levelProperty}'."); }
            SetupConsole(level);
        } else {  // use default init
            SetupConsole();
        }
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

    public static void SetupFile(FrozenDictionary<string, string> properties) {
        // level: none, verbose, debug, information, warning, error, fatal      # log level (default: debug)
        // path: /var/log/test.log                                              # file name (default: based on assembly name)
        // (if neither level nor file are defined then sink is not used)

        LogEventLevel? level = null;
        if (properties.TryGetValue("level", out string? levelProperty)) {
            if ("none".Equals(levelProperty, StringComparison.OrdinalIgnoreCase)) { return; }
            if (!Enum.TryParse<LogEventLevel>(levelProperty, ignoreCase: true, out var parsedLevel)) { throw new InvalidOperationException($"Unrecognized logging level '{levelProperty}'."); }
            level = parsedLevel;
        }

        if (properties.TryGetValue("path", out var filePath)) {  // file is defined
            SetupFile(filePath, level ?? LogEventLevel.Debug);
        } else if (level != null) {  // level is defined, use default file
            var assemblyName = (Assembly.GetEntryAssembly()?.GetName().Name) ?? throw new InvalidOperationException("Cannot determine logging file name.");
            SetupFile(assemblyName.ToLowerInvariant() + ".log", level.Value);
        }
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

    public static void SetupSeq(FrozenDictionary<string, string> properties) {
        // level: none, verbose, debug, information, warning, error, fatal      # log level (default: information)
        // url: http://localhost:5341                                           # server URL
        // (if neither level nor url are defined then sink is not used)

        LogEventLevel? level = null;
        if (properties.TryGetValue("level", out string? levelProperty)) {
            if ("none".Equals(levelProperty, StringComparison.OrdinalIgnoreCase)) { return; }
            if (!Enum.TryParse<LogEventLevel>(levelProperty, ignoreCase: true, out var parsedLevel)) { throw new InvalidOperationException($"Unrecognized logging level '{levelProperty}'."); }
            level = parsedLevel;
        }

        if (properties.TryGetValue("url", out var url)) {  // URL is defined
            SetupSeq(new Uri(url), level ?? LogEventLevel.Information);
        } else if (level != null) {  // level is defined, throw if no URL
            throw new InvalidOperationException("Cannot determine logging seq URL.");
        }
    }


    public static void Init() {
        Log.Logger = GetCurrentConfiguration()
                     .MinimumLevel.Is(_minimumLevel)
                     .CreateLogger();
    }

}
