namespace Revidere;

using System;
using System.Collections.Generic;
using System.Threading;
using Serilog;

internal sealed class CompositeCheck : Check {

    internal CompositeCheck(CheckProperties checkProperties)
        : base(checkProperties) {

        CheckNames = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in checkProperties.Target.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) {
            CheckNames.TryAdd(name, null);
        }
    }

    private readonly Dictionary<string, object?> CheckNames;


    public override bool CheckIsHealthy(IReadOnlyList<CheckState> checkStates, CancellationToken cancellationToken) {
        var healthyCount = 0;
        var totalCount = 0;

        foreach (var checkState in checkStates) {
            if ((checkState.Check.Name == null) || !CheckNames.ContainsKey(checkState.Check.Name)) { continue; }

            totalCount++;
            if (checkState.IsHealthy == true) { healthyCount++; }
        }

        var isHelthy = (totalCount > 0) && ((healthyCount * 100 / totalCount) >= (Percent ?? 100));
        Log.Verbose("COMPOSITE status: {Status} ({Healthy}/{Total})", isHelthy ? "Healthy" : "Unhealthy", healthyCount, totalCount);

        return isHelthy;
    }

}
