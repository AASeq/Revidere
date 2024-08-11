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


    /// <summary>
    /// Configure logging to console.
    /// </summary>
    /// <param name="minimumLevel">Minimum level to use.</param>
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
                                    [ConsoleThemeStyle.LevelDebug] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.DarkGray },
                                    [ConsoleThemeStyle.LevelInformation] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.DarkBlue },
                                    [ConsoleThemeStyle.LevelWarning] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.Yellow },
                                    [ConsoleThemeStyle.LevelError] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.Red },
                                    [ConsoleThemeStyle.LevelFatal] = new SystemConsoleThemeStyle { Foreground = ConsoleColor.White, Background = ConsoleColor.Red },
                                })
                         );
    }

    /// <summary>
    /// Configure logging to console.
    ///
    /// The following properties are expected:
    /// * level: none, verbose, debug, information, warning, error, fatal       # log level (default: information)
    ///
    /// If neither level nor path are defined then sink is not used.
    /// </summary>
    /// <param name="properties">Parsed logging properties.</param>
    public static void SetupConsole(FrozenDictionary<string, string> properties) {
        var minimumLevel = ParseNullableMinimumLevel(properties, "level", LogEventLevel.Information);
        if (minimumLevel == null) { return; }  // value was None; skip setup

        SetupConsole(minimumLevel.Value);
    }


    /// <summary>
    /// Configure logging to file.
    /// </summary>
    /// <param name="filePath">File path</param>
    /// <param name="rollingInterval">Rolling interval.</param>
    /// <param name="retainCount">Number of files to keep after rolling.</param>
    /// <param name="useBuffering">If output should be buffered.</param>
    /// <param name="minimumLevel">Minimum level to use.</param>
    public static void SetupFile(string filePath, RollingInterval rollingInterval = RollingInterval.Day, int retainCount = 7, bool useBuffering = true, LogEventLevel minimumLevel = LogEventLevel.Debug) {
        if ((int)_minimumLevel > (int)minimumLevel) { _minimumLevel = minimumLevel; };

        if (retainCount == 0) {
            retainCount = 7;
        } else if (retainCount < 0) {
            retainCount = -retainCount;
        }

        _configuration = GetCurrentConfiguration()
                         .WriteTo
                         .File(
                            restrictedToMinimumLevel: minimumLevel,
                            path: filePath,
                            rollingInterval: rollingInterval,
                            retainedFileCountLimit: retainCount,
                            buffered: useBuffering,
                            outputTemplate: "{Timestamp:HH:mm:ss.fff} {Level:u1} {Message:lj}{NewLine}{Exception}",
                            formatProvider: CultureInfo.InvariantCulture
                         );
    }

    /// <summary>
    /// Configure logging to file.
    ///
    /// The following properties are expected:
    /// * level:    none, verbose, debug, information, warning, error, fatal    # log level (default: debug)
    /// * path:     /var/log/test.log                                           # file name (default: based on assembly name)
    /// * interval: infinite year month day hour minute                         # interval for rolling (Valid values are  default: day)
    /// * retain:   7                                                           # number of log files to keep (default: 7)
    /// * buffered: true                                                        # if output will be buffered (default: true)
    ///
    /// If neither level nor path are defined then sink is not used.
    /// </summary>
    /// <param name="properties">Parsed logging properties.</param>
    public static void SetupFile(FrozenDictionary<string, string> properties) {
        if (properties.Count == 0) { return; }  // ignore if no properties are defined

        var minimumLevel = ParseNullableMinimumLevel(properties, "level", LogEventLevel.Information);
        if (minimumLevel == null) { return; }  // value was None; skip setup

        var filePath = ParseNullablePath(properties, "path", null);
        if (filePath == null) {
            var assemblyName = (Assembly.GetEntryAssembly()?.GetName().Name) ?? throw new InvalidOperationException("Cannot determine logging file name.");
            filePath = assemblyName.ToLowerInvariant() + ".log";
        }

        var rollingInterval = ParseInterval(properties, "interval", RollingInterval.Day);
        var retainCount = ParseRetainCount(properties, "retain", 7);
        var useBuffering = ParseUseBuffering(properties, "buffered", true);

        SetupFile(filePath, rollingInterval, retainCount, useBuffering, minimumLevel.Value);
    }


    /// <summary>
    /// Configure logging to seq server.
    /// </summary>
    /// <param name="serverUrl">Seq server URL.</param>
    /// <param name="minimumLevel">Minimum level to use.</param>
    public static void SetupSeq(Uri serverUrl, LogEventLevel minimumLevel = LogEventLevel.Information) {
        if ((int)_minimumLevel > (int)minimumLevel) { _minimumLevel = minimumLevel; };

        _configuration = GetCurrentConfiguration()
                         .WriteTo
                         .Seq(
                             restrictedToMinimumLevel: minimumLevel,
                             serverUrl: serverUrl.ToString()
                         );
    }

    /// <summary>
    /// Configure logging to seq server.
    ///
    /// The following properties are expected:
    /// * level: none, verbose, debug, information, warning, error, fatal       # log level (default: information)
    /// * url:   http://localhost:5341                                          # server URL
    ///
    /// If neither level nor url are defined then sink is not used.
    /// </summary>
    /// <param name="properties">Parsed logging properties.</param>
    public static void SetupSeq(FrozenDictionary<string, string> properties) {
        if (properties.Count == 0) { return; }  // ignore if no properties are defined

        var minimumLevel = ParseNullableMinimumLevel(properties, "level", LogEventLevel.Information);
        if (minimumLevel == null) { return; }  // value was None; skip setup

        var serverUrl = ParseNullableServerUrl(properties, "url", null);
        if (serverUrl == null) { throw new InvalidOperationException("Cannot determine logging seq URL."); }

        SetupSeq(serverUrl, minimumLevel.Value);
    }


    /// <summary>
    /// Initialize logging based on previously supplied configuration.
    /// Setup* methods must be called before this method.
    /// </summary>
    public static void Init() {
        Log.Logger = GetCurrentConfiguration()
                     .MinimumLevel.Is(_minimumLevel)
                     .CreateLogger();
    }


    #region Helpers

    private static string? GetPropertyValue(FrozenDictionary<string, string> properties, string key) {
        return properties.TryGetValue(key, out string? value) ? value.Trim() : null;
    }

    private static LogEventLevel? ParseNullableMinimumLevel(FrozenDictionary<string, string> properties, string key, LogEventLevel defaultMinimumLevel) {
        var text = GetPropertyValue(properties, key);
        if ("none".Equals(text, StringComparison.OrdinalIgnoreCase)) { return null; }
        if ("info".Equals(text, StringComparison.OrdinalIgnoreCase)) { return LogEventLevel.Information; }
        if (Enum.TryParse<LogEventLevel>(text, ignoreCase: true, out var parsedLevel)) {
            return parsedLevel;
        } else {
            return defaultMinimumLevel;
        }
    }

    private static string? ParseNullablePath(FrozenDictionary<string, string> properties, string key, string? defaultPath) {
        var text = GetPropertyValue(properties, key);
        if (text != null) {
            return text;
        } else {
            return defaultPath;
        }
    }

    private static RollingInterval ParseInterval(FrozenDictionary<string, string> properties, string key, RollingInterval defaultRollingInterval) {
        var text = GetPropertyValue(properties, key);
        if (Enum.TryParse<RollingInterval>(text, ignoreCase: true, out var parsedInterval)) {
            return parsedInterval;
        } else {
            return defaultRollingInterval;
        }
    }

    private static int ParseRetainCount(FrozenDictionary<string, string> properties, string key, int defaultRetainCount) {
        var text = GetPropertyValue(properties, key);
        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var retainCount)) {
            return retainCount;
        } else {
            return defaultRetainCount;
        }
    }

    private static bool ParseUseBuffering(FrozenDictionary<string, string> properties, string key, bool defaultUseBuffering) {
        var text = GetPropertyValue(properties, key);
        if (bool.TryParse(text, out var useBuffering)) {
            return useBuffering;
        } else if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var useBufferingInt)) {
            return (useBufferingInt != 0);
        } else if ("yes".Equals(text, StringComparison.OrdinalIgnoreCase)) {
            return true;
        } else if ("no".Equals(text, StringComparison.OrdinalIgnoreCase)) {
            return false;
        } else {
            return defaultUseBuffering;
        }
    }

    private static Uri? ParseNullableServerUrl(FrozenDictionary<string, string> properties, string key, Uri? defaultServerUrl) {
        var text = GetPropertyValue(properties, key);
        if (Uri.TryCreate(text, UriKind.Absolute, out var serverUrl)) {
            return serverUrl;
        } else {
            return defaultServerUrl;
        }
    }

    #endregion Helpers
}
