namespace Revidere;

using System;
using System.Collections.Generic;
using System.Threading;
using Serilog;
using Serilog.Events;

internal static class App {
    private static void Main() {
        Logging.SetupConsole(LogEventLevel.Verbose);  // TODO: Configurable
        Logging.SetupFile("revidere.log", LogEventLevel.Verbose);  // TODO: Configurable
        //Logging.SetupSeq(new Uri("http://127.0.0.1"), LogEventLevel.Debug);  // TODO: Configurable
        Logging.Init();

        var targets = new List<Target>();
        targets.Add(new Target("dummy", "Dummy", new Uri("dummy://dummy"), CheckProfile.Default));
        targets.Add(new Target(null, "Random 1", new Uri("random://localhost"), CheckProfile.Default));
        targets.Add(new Target(null, "Random 2", new Uri("random://localhost2"), CheckProfile.Default));
        targets.Add(new Target("random", "Random 3", new Uri("random://"), CheckProfile.Default));
        targets.Add(new Target("ping", "AASeq ping", new Uri("ping://aaseq.com"), CheckProfile.Default));
        targets.Add(new Target(null, "aaseq.com", new Uri("https://aaseq.com"), CheckProfile.Default));
        targets.Add(new Target(null, "Random health probe", new Uri("http://localhost:8089/healthz/random"), CheckProfile.Default));

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
