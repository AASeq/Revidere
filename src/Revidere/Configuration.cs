using System;
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
        var config = new YamlConfig();

        // Config: Logging

        Logging.SetupConsole(config.GetLoggingProperties("console"));
        Logging.SetupFile(config.GetLoggingProperties("file"));
        Logging.SetupSeq(config.GetLoggingProperties("seq"));

        Logging.Init();


        // Config: Web

        var webConfig = config.GetProperties("web");

        webConfig.TryGetValue("prefix", out var webPrefix);
        if (string.IsNullOrEmpty(webPrefix)) { webPrefix = "http://*:8089/"; }
        if (!webPrefix.EndsWith('/')) { webPrefix += "/"; }  // prefix must end with a trailing slash

        webConfig.TryGetValue("title", out var webTitle);
        webTitle ??= "Revidere";

        webConfig.TryGetValue("refresh", out var webRefreshText);
        int.TryParse(webRefreshText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var webRefresh);
        if (webRefresh == 0) {
            webRefresh = 5;  // default
            if (webRefreshText != null) { Log.Warning("Web refresh interval too low; adjusted to {interval} seconds", webRefresh); }
        } else if (webRefresh < 5) {
            webRefresh = 5;
            Log.Warning("Web refresh interval too low; adjusted to {interval} seconds", webRefresh);
        } else if (webRefresh > 60) {
            webRefresh = 60;
            Log.Warning("Web refresh interval too high; adjusted to {interval} seconds", webRefresh);
        }

        var webConfiguration = new WebConfiguration(webPrefix, webTitle, webRefresh);


        // Config: Checks

        var checks = new List<Check>();

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

            checkConfig.TryGetValue("period", out var checkPeriodText);
            double.TryParse(checkPeriodText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var checkPeriod);
            if (checkPeriod == 0) {
                checkPeriod = CheckProfile.Default.Period.TotalSeconds;  // default
                if (checkPeriodText != null) { Log.Information("Check period using default of {interval} seconds", checkPeriod); }
            } else if (checkPeriod < 1) {
                checkPeriod = 1;
                Log.Warning("Check period too low; adjusted to {interval} seconds", checkPeriod);
            } else if (checkPeriod > 600) {
                checkPeriod = 600;
                Log.Warning("Check period too high; adjusted to {interval} seconds", checkPeriod);
            }

            checkConfig.TryGetValue("timeout", out var checkTimeoutText);
            double.TryParse(checkTimeoutText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var checkTimeout);
            if (checkTimeout == 0) {
                checkTimeout = CheckProfile.Default.Timeout.TotalSeconds;  // default
                if (checkTimeoutText != null) { Log.Information("Check timeout using default of {interval} seconds", checkTimeout); }
            } else if (checkTimeout < 0.1) {
                checkTimeout = 1;
                Log.Warning("Check timeout too low; adjusted to {interval} seconds", checkTimeout);
            } else if (checkTimeout > 60) {
                checkTimeout = 60;
                Log.Warning("Check timeout too high; adjusted to {interval} seconds", checkTimeout);
            }

            checkConfig.TryGetValue("success", out var checkSuccessText);
            int.TryParse(checkSuccessText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var checkSuccess);
            if (checkSuccess == 0) {
                checkSuccess = CheckProfile.Default.SuccessCount;  // default
                if (checkSuccessText != null) { Log.Information("Check success count using default of {count}", checkSuccess); }
            } else if (checkSuccess < 1) {
                checkSuccess = 1;
                Log.Warning("Check success count too low; adjusted to {count}", checkSuccess);
            } else if (checkSuccess > 10) {
                checkSuccess = 10;
                Log.Warning("Check success count too high; adjusted to {count}", checkSuccess);
            }

            checkConfig.TryGetValue("failure", out var checkFailureText);
            int.TryParse(checkFailureText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var checkFailure);
            if (checkFailure == 0) {
                checkFailure = CheckProfile.Default.FailureCount;  // default
                if (checkFailureText != null) { Log.Information("Check success count using default of {count}", checkFailure); }
            } else if (checkFailure < 1) {
                checkFailure = 1;
                Log.Warning("Check failure count too low; adjusted to {count}", checkFailure);
            } else if (checkFailure > 10) {
                checkFailure = 10;
                Log.Warning("Check failure count too high; adjusted to {count}", checkFailure);
            }

            var check = Check.FromConfigData(
                checkKind,
                checkTarget ?? "",
                checkTitle ?? checkName ?? checkKind,
                checkName,
                new CheckProfile(TimeSpan.FromSeconds(checkPeriod), TimeSpan.FromSeconds(checkTimeout), checkSuccess, checkFailure));
            if (check != null) { checks.Add(check); }

            Log.Information("Added check {check}", check);
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
}


internal record WebConfiguration(string Prefix, string Title, int RefreshInterval);