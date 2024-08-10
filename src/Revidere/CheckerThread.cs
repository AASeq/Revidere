namespace Revidere;

using System;
using System.Collections.Generic;
using System.Threading;
using Serilog;

internal static class CheckerThread {

    public static void Start(IReadOnlyList<CheckState> checkStates, CancellationToken cancellationToken) {
        Log.Verbose("Starting CheckerThread");
        CheckStates = checkStates;
        CancellationToken = cancellationToken;

        Thread = new Thread(Run) {
            IsBackground = true,
            Name = "Checker",
        };
        Thread.Start();
    }

    public static void Stop() {
        Log.Verbose("Stopping CheckerThread");
        Thread?.Join();
    }


    private static IReadOnlyList<CheckState>? CheckStates;

    private static Thread? Thread;
    private static CancellationToken? CancellationToken;


    private static void Run() {
        Log.Verbose("Started CheckerThread");

        var cancellationToken = CancellationToken!.Value;
        var checkStates = CheckStates!;

        var sleepInterval = Math.Min(Math.Max(1000 / checkStates.Count, 1), 100);  // 1-100ms
        while (!cancellationToken.IsCancellationRequested) {
            foreach (var checkState in checkStates) {
                var check = checkState.Check;
                var checker = checkState.Check.Checker;
                var profile = checkState.Check.CheckProfile;

                var lastUpdate = checkState?.LastUpdated ?? DateTimeOffset.MinValue;
                var shouldCheck = (DateTimeOffset.Now - lastUpdate).TotalSeconds > profile.Period.TotalSeconds;

                if (shouldCheck) {
                    var isHealthy = checker.CheckIsHealthy(cancellationToken, profile.Timeout);
                    Log.Verbose("Check for {Check}: {Status}", check, isHealthy ? "healthy" : "unhealthy");
                    checkState!.UpdateCheck(isHealthy);
                    if (cancellationToken.IsCancellationRequested) { break; }
                }

                Thread.Sleep(sleepInterval);
            }
        }

        Log.Verbose("Stopped CheckerThread");
    }


}
