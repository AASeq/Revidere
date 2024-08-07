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
            targetConfig.TryGetValue("name", out var name);
            targetConfig.TryGetValue("title", out var title);
            targetConfig.TryGetValue("target", out var targetUri1);
            targetConfig.TryGetValue("", out var targetUri2);
            var targetText = targetUri1 ?? targetUri2 ?? "";
            if (!Uri.TryCreate(targetText, UriKind.Absolute, out var targetUri)) {
                Log.Warning("Invalid target URI: {target}", targetText);
                continue;
            }
            var target = new Target(name, title ?? name ?? "", targetUri, CheckProfile.Default);
            targets.Add(target);
            Log.Information("Added target {target}", target);
        }

        // Config: Web
        var webConfig = config.GetProperties("web");
        webConfig.TryGetValue("title", out var webTitle);
        webTitle ??= "Revidere";
        webConfig.TryGetValue("refresh", out var webRefreshText);
        int.TryParse(webRefreshText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var webRefresh);
        if (webRefresh == 0) {
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

        HttpThread.Start(webTitle, targetStates, source.Token);
        CheckerThread.Start(targetStates, source.Token);

        source.Token.WaitHandle.WaitOne();
        CheckerThread.Stop();
        HttpThread.Stop();
    }
}
