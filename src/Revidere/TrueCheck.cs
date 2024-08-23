namespace Revidere;

using System.Collections.Generic;
using System.Threading;
using Serilog;

internal sealed class TrueCheck : Check {

    internal TrueCheck(CheckProperties checkProperties)
        : base(checkProperties) {
    }

    public override bool CheckIsHealthy(IReadOnlyList<CheckState> checkStates, CancellationToken cancellationToken) {
        Log.Verbose("{Check} status: {Status}", this, "Healthy");
        return true;
    }

}
