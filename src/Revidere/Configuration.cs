using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using Serilog;

namespace Revidere;

internal sealed class Configuration {

    private Configuration(WebConfiguration webConfiguration, IEnumerable<Check> checks) {
        Web = webConfiguration;
        Checks = new List<Check>(checks).AsReadOnly();
    }

    public WebConfiguration Web { get; }
    public IReadOnlyList<Check> Checks { get; }


    public static Configuration Load() {
        var config = YamlConfig.FromConfigFile();

        var checks = new List<Check>();
        WebConfiguration webConfiguration = WebConfiguration.Default;

        if (config != null) {

            // Config: Logging

            Logging.SetupConsole(config.GetLoggingProperties("console"));
            Logging.SetupFile(config.GetLoggingProperties("file"));
            Logging.SetupSeq(config.GetLoggingProperties("seq"));

            Logging.Init();


            // Config: Web

            var webConfig = config.GetProperties("web");

            var webPrefixText = ParseString(webConfig, "prefix", WebConfiguration.Default.Prefixes[0]);
            if (string.IsNullOrWhiteSpace(webPrefixText)) { webPrefixText = WebConfiguration.Default.Prefixes[0]; }
            var webPrefixes = webPrefixText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            for (var i = 0; i < webPrefixes.Length; i++) {
                if (!webPrefixes[i].EndsWith('/')) { webPrefixes[i] += '/'; }  // prefix must end with a trailing slash
            }

            var webTitle = ParseString(webConfig, "title", WebConfiguration.Default.Title);
            var webRefresh = ParseInteger(webConfig, "refresh", 1, 60, WebConfiguration.Default.RefreshInterval);

            webConfiguration = new WebConfiguration(webPrefixes, webTitle, webRefresh);


            // Config: Checks

            var checksConfigList = config.GetSequenceProperties("checks");
            foreach (var checkConfig in checksConfigList) {
                checkConfig.TryGetValue("kind", out var checkKind);
                checkConfig.TryGetValue("target", out var checkTargetText);  // either just number target (default) or allow URL-based target
                checkConfig.TryGetValue("name", out var checkName);
                checkConfig.TryGetValue("title", out var checkTitle);
                checkConfig.TryGetValue("", out var checkTargetText2);  // URL-based target (only if proper target is not set)
                checkTargetText ??= checkTargetText2 ?? "";
                string? checkTarget = checkTargetText;
                if (checkKind == null) {
                    (checkKind, checkTarget) = GetCheckKindAndTarget(checkTargetText);
                }

                if (checkKind == null) {
                    Log.Warning("Check kind not set; skipping check");
                    continue;
                }

                var checkPeriod = TimeSpan.FromSeconds(ParseDouble(checkConfig, "period", 1, 60, CheckProfile.Default.Timeout.TotalSeconds));
                var checkTimeout = TimeSpan.FromSeconds(ParseDouble(checkConfig, "timeout", 0.01, 10, CheckProfile.Default.Timeout.TotalSeconds));
                var checkSuccess = ParseInteger(checkConfig, "success", 1, 10, CheckProfile.Default.SuccessCount);
                var checkFailure = ParseInteger(checkConfig, "failure", 1, 10, CheckProfile.Default.FailureCount);
                var isVisible = ParseBool(checkConfig, "visible", true);
                var isBreak = ParseBool(checkConfig, "break", false);

                var check = Check.FromConfigData(
                    checkKind,
                    checkTarget ?? "",
                    checkTitle ?? checkName ?? checkKind,
                    checkName,
                    isVisible,
                    isBreak,
                new CheckProfile(checkPeriod, checkTimeout, checkSuccess, checkFailure));
                if (check != null) { checks.Add(check); }

                Log.Information("Configured check {check}", check);
            }

        } else {  // default console
            Logging.SetupConsole();
            Logging.Init();
            Log.Warning("Configuration file not found; using defaults");
        }


        // Environment: Checks

        var envCheckTargetUrls = Environment.GetEnvironmentVariable("CHECKS")?.Split(new char[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
        foreach (var targetUrl in envCheckTargetUrls) {
            (var checkKind, var checkTarget) = GetCheckKindAndTarget(targetUrl);
            if (checkKind == null) {
                Log.Warning($"Check kind not set for '{targetUrl}'; skipping check");
                continue;
            }
            var check = Check.FromConfigData(
                checkKind,
                checkTarget ?? "",
                title: null,
                name: null,
                isVisible: true,
                isBreak: false,
                CheckProfile.Default);
            if (check != null) { checks.Add(check); }
            Log.Information("Configured check {check}", check);
        }


        return new Configuration(webConfiguration, checks);
    }

    private static (string? checkKind, string? checkTarget) GetCheckKindAndTarget(string targetUri) {
        var parts = targetUri.Split("://", 2);
        if (parts.Length < 1) { return (null, null); }

        var scheme = parts[0];
        var rest = parts[1] ?? "";

        if (scheme.Equals("http", StringComparison.OrdinalIgnoreCase)) {
            return ("get", "http://" + rest);
        } else if (scheme.Equals("https", StringComparison.OrdinalIgnoreCase)) {
            return ("get", "https://" + rest);
        } else {
            return (scheme, rest);
        }
    }

    private static bool ParseBool(FrozenDictionary<string, string> checkConfig, string key, bool defaultValue) {
        checkConfig.TryGetValue(key, out var valueText);
        if (bool.TryParse(valueText, out var value)) {
            return value;
        } else {
            return defaultValue;
        }
    }

    private static int ParseInteger(FrozenDictionary<string, string> checkConfig, string key, int minValue, int maxValue, int defaultValue) {
        checkConfig.TryGetValue(key, out var valueText);
        int.TryParse(valueText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value);
        if (value == 0) {
            if (valueText != null) { Log.Information("Check " + key + " count using default of {count}", defaultValue); }
            return defaultValue;
        } else if (value < minValue) {
            value = minValue;
            Log.Warning("Check " + key + " count too low; adjusted to {count}", value);
        } else if (value > maxValue) {
            value = maxValue;
            Log.Warning("Check " + key + " count too high; adjusted to {count}", value);
        }
        return value;
    }

    private static double ParseDouble(FrozenDictionary<string, string> checkConfig, string key, double minValue, double maxValue, double defaultValue) {
        checkConfig.TryGetValue(key, out var valueText);
        double.TryParse(valueText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value);
        if (value == 0) {
            if (valueText != null) { Log.Information("Check " + key + " count using default of {count}", defaultValue); }
            value = defaultValue;
        } else if (value < minValue) {
            value = minValue;
            Log.Warning("Check " + key + " count too low; adjusted to {count}", value);
        } else if (value > maxValue) {
            value = maxValue;
            Log.Warning("Check " + key + " count too high; adjusted to {count}", value);
        }
        return value;
    }

    private static string ParseString(FrozenDictionary<string, string> checkConfig, string key, string defaultValue) {
        checkConfig.TryGetValue(key, out var valueText);
        return valueText ?? defaultValue;
    }
}


internal record WebConfiguration(string[] Prefixes, string Title, int RefreshInterval) {
    public static WebConfiguration Default => new(["http://*:8089/"], "Revidere", 10);
}
