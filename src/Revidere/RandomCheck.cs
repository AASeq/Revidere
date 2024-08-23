namespace Revidere;

using System;
using System.Collections.Generic;
using System.Threading;
using Serilog;

internal sealed class RandomCheck : Check {

    internal RandomCheck(CheckProperties checkProperties)
        : base(checkProperties) {

        var seed = 0;
        foreach (var ch in checkProperties.Target) {
            seed = (seed << 5) ^ (seed << 3) ^ (seed << 1) ^ ch;
        }
        Random = (seed == 0) ? new Random((int)DateTime.Now.Ticks) : new Random(seed);
    }

    private readonly Random Random;


    public override bool CheckIsHealthy(IReadOnlyList<CheckState> checkStates, CancellationToken cancellationToken) {
        var isHealthy = (Random.Next() & 0x01) != 0;
        Log.Verbose("RANDOM {Target} status: {Status}", Target, isHealthy ? "Healthy" : "Unhealthy");
        return isHealthy;
    }

}
