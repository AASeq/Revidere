namespace Revidere;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Serilog;

internal static class App {
    private static void Main() {
        var config = new YamlConfig();

        // Config: Logging
        Logging.SetupConsole(config.GetLoggingProperties("console"));
        Logging.SetupFile(config.GetLoggingProperties("file"));
        Logging.SetupSeq(config.GetLoggingProperties("seq"));
        Logging.Init();

        // Config: Targets
        var targets = new List<Target>();
        var targetsConfigList = config.GetSequenceProperties("targets");
        foreach (var targetConfig in targetsConfigList) {
            targetConfig.TryGetValue("name", out var targetName);
            targetConfig.TryGetValue("title", out var targetTitle);
            targetConfig.TryGetValue("target", out var targetUri1);
            targetConfig.TryGetValue("", out var targetUri2);
            var targetText = targetUri1 ?? targetUri2 ?? "";
            if (!Uri.TryCreate(targetText, UriKind.Absolute, out var targetUri)) {
                Log.Warning("Invalid target URI: {target}", targetText);
                continue;
            }

            targetConfig.TryGetValue("period", out var targetPeriodText);
            double.TryParse(targetPeriodText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var targetPeriod);
            if (targetPeriod == 0) {
                targetPeriod = CheckProfile.Default.Period.TotalSeconds;  // default
                if (targetPeriodText != null) { Log.Information("Check period using default of {interval} seconds", targetPeriod); }
            } else if (targetPeriod < 1) {
                targetPeriod = 1;
                Log.Warning("Check period too low; adjusted to {interval} seconds", targetPeriod);
            } else if (targetPeriod > 600) {
                targetPeriod = 600;
                Log.Warning("Check period too high; adjusted to {interval} seconds", targetPeriod);
            }
            targetConfig.TryGetValue("timeout", out var targetTimeoutText);
            double.TryParse(targetTimeoutText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var targetTimeout);
            if (targetTimeout == 0) {
                targetTimeout = CheckProfile.Default.Timeout.TotalSeconds;  // default
                if (targetTimeoutText != null) { Log.Information("Check timeout using default of {interval} seconds", targetTimeout); }
            } else if (targetTimeout < 0.1) {
                targetTimeout = 1;
                Log.Warning("Check timeout too low; adjusted to {interval} seconds", targetTimeout);
            } else if (targetTimeout > 60) {
                targetTimeout = 60;
                Log.Warning("Check timeout too high; adjusted to {interval} seconds", targetTimeout);
            }
            targetConfig.TryGetValue("success", out var targetSuccessText);
            int.TryParse(targetSuccessText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var targetSuccess);
            if (targetSuccess == 0) {
                targetSuccess = CheckProfile.Default.SuccessCount;  // default
                if (targetSuccessText != null) { Log.Information("Check success count using default of {count}", targetSuccess); }
            } else if (targetSuccess < 1) {
                targetSuccess = 1;
                Log.Warning("Check success count too low; adjusted to {count}", targetSuccess);
            } else if (targetSuccess > 10) {
                targetSuccess = 10;
                Log.Warning("Check success count too high; adjusted to {count}", targetSuccess);
            }
            targetConfig.TryGetValue("failure", out var targetFailureText);
            int.TryParse(targetFailureText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var targetFailure);
            if (targetFailure == 0) {
                targetFailure = CheckProfile.Default.FailureCount;  // default
                if (targetFailureText != null) { Log.Information("Check success count using default of {count}", targetFailure); }
            } else if (targetFailure < 1) {
                targetFailure = 1;
                Log.Warning("Check failure count too low; adjusted to {count}", targetFailure);
            } else if (targetFailure > 10) {
                targetFailure = 10;
                Log.Warning("Check failure count too high; adjusted to {count}", targetFailure);
            }

            var target = new Target(
                                    targetName,
                                    targetTitle ?? targetName ?? "",
                                    targetUri,
                                    new CheckProfile(
                                                     TimeSpan.FromSeconds(targetPeriod),
                                                     TimeSpan.FromSeconds(targetTimeout),
                                                     targetSuccess,
                                                     targetFailure
                                                    )
                                   );
            targets.Add(target);
            Log.Information("Added target {target}", target);
        }

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


        var targetStates = new List<TargetState>();
        foreach (var target in targets) {
            targetStates.Add(new TargetState(target));
        }

        var source = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) => {
            Log.Information("Ctrl+C pressed");
            e.Cancel = true;
            source.Cancel();
        };

        HttpThread.Start(webPrefix, webTitle, webRefresh, targetStates, source.Token);
        CheckerThread.Start(targetStates, source.Token);

        source.Token.WaitHandle.WaitOne();
        CheckerThread.Stop();
        HttpThread.Stop();
    }
}
