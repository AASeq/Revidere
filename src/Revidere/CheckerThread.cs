namespace Revidere;

using System;
using System.Collections.Generic;
using System.Threading;
using Serilog;

internal static class CheckerThread {

    public static void Start(IReadOnlyList<CheckState> checkStates, CancellationToken cancellationToken) {
        Log.Verbose("Starting Checker thread");
        CancellationToken = cancellationToken;
        CheckStates = checkStates;

        var index = 0;
        foreach (var checkState in checkStates) {
            index++;
            var thread = new Thread(Run) {
                IsBackground = true,
                Name = $"Checker#{index}",
            };
            thread.Start(checkState);
            Thread.Sleep(50);  // slow it down a bit
        }
    }

    public static void Stop() {
        Log.Verbose("Stopping Checker thread");
        foreach (var thread in Threads) {
            thread.Join();
        }
    }


    private static readonly IList<Thread> Threads = [];
    private static CancellationToken? CancellationToken;
    private static IReadOnlyList<CheckState> CheckStates = [];


    private static void Run(object? state) {
        var checkState = (CheckState)state!;
        var cancellationToken = CancellationToken!.Value;

        Log.Debug("Started Checker thread for {Check}", checkState.Check);

        while (!cancellationToken.IsCancellationRequested) {
            var check = checkState!.Check;
            var profile = check.Properties.CheckProfile;

            var period = (check is CompositeCheck) ? 1 : profile.Period.TotalSeconds;  // composite checks are always 1 second
            var lastUpdate = checkState?.LastUpdated ?? DateTimeOffset.MinValue;
            var shouldCheck = (DateTimeOffset.Now - lastUpdate).TotalSeconds >= period;

            if (shouldCheck) {
                var isHealthy = check.CheckIsHealthy(CheckStates, cancellationToken);
                Log.Debug("Check for {Check}: {Status}", check, isHealthy ? "healthy" : "unhealthy");
                checkState!.UpdateCheck(isHealthy);
            }

            if (cancellationToken.IsCancellationRequested) { break; }
            Thread.Sleep(100);
        }

        Log.Verbose("Stopped Checker thread for {Check}", checkState!.Check);
    }

}
