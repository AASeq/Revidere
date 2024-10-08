﻿namespace Revidere;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Serilog;

internal static class App {
    private static void Main() {
        var config = Configuration.Load();

        var version = Assembly.GetEntryAssembly()?.GetName().Version;
        Log.Information("Starting" + (version != null ? $" (version {version.Major:0}.{version.Minor:0}.{version.Build:0})" : ""));

        if (config.Checks.Count == 0) {
            Log.Fatal("No checks configured.");
            return;
        }

        var checkStates = new List<CheckState>();
        foreach (var check in config.Checks) {
            checkStates.Add(new CheckState(check));
        }

        var source = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) => {
            Log.Information("Ctrl+C pressed");
            e.Cancel = true;
            source.Cancel();
        };

        WebThread.Start(config.Web, checkStates, source.Token);
        CheckerThread.Start(checkStates, source.Token);

        source.Token.WaitHandle.WaitOne();
        CheckerThread.Stop();
        WebThread.Stop();

        Log.Information("Done");
    }
}
