namespace Revidere;

using System.Collections.Generic;
using System.Threading;
using Serilog;

internal sealed class FalseCheck : Check {

    internal FalseCheck(CheckProperties checkProperties)
        : base(checkProperties) {
    }

    public override bool CheckIsHealthy(IReadOnlyList<CheckState> checkStates, CancellationToken cancellationToken) {
        Log.Verbose("{Check} status: {Status}", this, "Unhealthy");
        return true;
    }

}
