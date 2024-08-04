namespace Revidere;

using System;
using System.Collections.Generic;
using System.Threading;
using Serilog;

internal static class CheckerThread {

    public static void Start(IReadOnlyList<TargetState> targetStates, CancellationToken cancellationToken) {
        Log.Verbose("Starting CheckerThread");
        TargetStates = targetStates;
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


    private static Thread? Thread;
    private static CancellationToken? CancellationToken;
    private static IReadOnlyList<TargetState>? TargetStates;


    private static void Run() {
        Log.Verbose("Started CheckerThread");

        var cancellationToken = CancellationToken!.Value;
        var targetStates = TargetStates!;

        var sleepInterval = Math.Min(Math.Max(1000 / targetStates.Count, 1), 100);  // 1-100ms
        while (!cancellationToken.IsCancellationRequested) {
            foreach (var targetState in targetStates) {
                var target = targetState.Target;
                var checker = targetState.Target.Checker;
                var profile = targetState.Target.CheckProfile;

                var lastUpdate = targetState?.LastUpdated ?? DateTimeOffset.MinValue;
                var shouldCheck = (DateTimeOffset.Now - lastUpdate).TotalSeconds > profile.Period.TotalSeconds;

                if (shouldCheck) {
                    var isHealthy = checker.CheckIsHealthy(cancellationToken, profile.Timeout);
                    Log.Verbose("Check for {Target}: {Status}", target, isHealthy ? "healthy" : "unhealthy");
                    targetState!.UpdateCheck(isHealthy);
                    if (cancellationToken.IsCancellationRequested) { break; }
                }

                Thread.Sleep(sleepInterval);
            }
        }

        Log.Verbose("Stopped CheckerThread");
    }


}
