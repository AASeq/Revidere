namespace Revidere;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Serilog;
using Serilog.Events;

internal static class App {
    private static void Main() {
        var config = new YamlConfig();
        Logging.SetupConsole(config.GetLoggingProperties("console"));
        Logging.SetupFile(config.GetLoggingProperties("file"));
        Logging.SetupSeq(config.GetLoggingProperties("seq"));
        Logging.Init();

        var targets = new List<Target>();

        var targetsConfigList = config.GetSequencedProperties("targets");
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

        HttpThread.Start(targetStates, source.Token);
        CheckerThread.Start(targetStates, source.Token);

        source.Token.WaitHandle.WaitOne();
        CheckerThread.Stop();
        HttpThread.Stop();
    }
}
